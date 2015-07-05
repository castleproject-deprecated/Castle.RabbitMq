namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Framing;

	[System.Diagnostics.DebuggerDisplay("Queue '{Name}' {_queueOptions}", Name = "Queue")]
	public class RabbitQueue : IRabbitQueue
	{
		private readonly IModel _model;
		private readonly IRabbitSerializer _defaultSerializer;
		private readonly QueueOptions _queueOptions;

		public RabbitQueue(IModel model, 
						   IRabbitSerializer serializer,
						   QueueDeclareOk result, 
						   QueueOptions queueOptions)
		{
			_model = model;
			_defaultSerializer = serializer ?? queueOptions.Serializer;
			_queueOptions = queueOptions;

			this.Name = result.QueueName;
			this.ConsumerCount = result.ConsumerCount;
			this.MessageCount = result.MessageCount;
		}

		public string Name { get; private set; }
		public uint ConsumerCount { get; private set; }
		public uint MessageCount { get; private set; }

		public void Purge()
		{
			lock(_model)
				_model.QueuePurge(this.Name);
		}

		public void Delete()
		{
			lock(_model)
				_model.QueueDelete(this.Name);
		}

		public void Delete(bool ifUnused, bool ifEmpty)
		{
			lock(_model)
				_model.QueueDelete(this.Name, ifUnused, ifEmpty);
		}

		#region IRabbitQueueConsumer

		public Subscription RespondRaw(Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond, 
									   ConsumerOptions options)
		{
			Argument.NotNull(onRespond, "onRespond");

			return InternalRespondRaw(onRespond, options, shouldSerializeExceptions: false);
		}

		public Subscription ConsumeRaw(Action<MessageEnvelope, IMessageAck> onReceived, 
									   ConsumerOptions options)
		{
			Argument.NotNull(onReceived, "onReceived");
			options = options ?? ConsumerOptions.Default;

			var consumer = CreateConsumer(options);

			consumer.Subscribe(new ActionAdapter(env =>
			{
				var msgAcker = new MessageAck(() => { lock(_model) _model.BasicAck(env.DeliveryTag, false); },
					(requeue) => { lock(_model) _model.BasicNack(env.DeliveryTag, false, requeue); });

				onReceived(env, msgAcker);
			}));

			lock(_model)
			{
				var consumerTag = _model.BasicConsume(this.Name, options.NoAck, consumer);

				return new Subscription(_model, consumerTag);
			}
		}

		[Obsolete]
		public Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond,
			ConsumerOptions options)
		{
			Argument.NotNull(onRespond, "onRespond");
			
			var serializer = options.Serializer ?? _defaultSerializer;

			var typedOnRespond = new Func<MessageEnvelope, IMessageAck, MessageEnvelope>((envelope, ack) =>
			{
				var typedMessage = serializer.Deserialize<TRequest>(envelope.Body, envelope.Properties);

				var replyInstance = onRespond(new MessageEnvelope<TRequest>(envelope, typedMessage), ack);

				var replyProperties = new BasicProperties();
				var replyData = serializer.Serialize(replyInstance, replyProperties);

				return new MessageEnvelope(replyProperties, replyData);
			});

			return InternalRespondRaw(typedOnRespond, options, shouldSerializeExceptions: true);
		}

		[Obsolete]
		public Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived,
			ConsumerOptions options)
		{
			Argument.NotNull(onReceived, "onReceived");

			options = options ?? ConsumerOptions.Default;

			var serializer = options.Serializer ?? _defaultSerializer;

			var typedReceived = new Action<MessageEnvelope, IMessageAck>((envelope, ack) =>
			{
				var message = serializer.Deserialize<T>(envelope.Body, envelope.Properties);

				onReceived(new MessageEnvelope<T>(envelope, message), ack);
			});

			return this.ConsumeRaw(typedReceived, options);
		}

		#endregion

		private Subscription InternalRespondRaw(Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond, 
			ConsumerOptions options, 
			bool shouldSerializeExceptions)
		{
			options = options ?? ConsumerOptions.Default;
			var serializer = options.Serializer ?? _defaultSerializer;

			var consumer = CreateConsumer(options);

			consumer.Subscribe(new RpcResponder(_model, serializer, onRespond, shouldSerializeExceptions));

			lock (_model)
			{
				var consumerTag = _model.BasicConsume(this.Name, options.NoAck, consumer);

				return new Subscription(_model, consumerTag);
			}
		}

		private IRabbitMessageProducer CreateConsumer(ConsumerOptions options)
		{
			if (options.ConsumerStrategy == ConsumerStrategy.Default)
			{
				return new StreamerConsumer(_model);
			}
			// else if (options.ConsumerStrategy == ConsumerStrategy.Queue)
			{
				return new SharedQueueConsumer(_model);
			}
		}
	}
}