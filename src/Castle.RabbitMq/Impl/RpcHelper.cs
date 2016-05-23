namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Threading;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;


	internal class RpcHelper : IBasicConsumer
	{
		private readonly IModel _model;
		private readonly IRabbitSerializer _serializer;
		private readonly string _exchange;

		private readonly Dictionary<string, string> _routing2RetQueue;
		private readonly ConcurrentDictionary<string, AutoResetEvent> _waits;
		private readonly ConcurrentDictionary<string, MessageEnvelope> _replyData;

		public RpcHelper(IModel model, string exchange, IRabbitSerializer serializer)
		{
			_model = model;
			_exchange = exchange;
			_serializer = serializer;

			_routing2RetQueue = new Dictionary<string, string>(StringComparer.Ordinal);
			_waits = new ConcurrentDictionary<string, AutoResetEvent>(StringComparer.Ordinal);
			_replyData = new ConcurrentDictionary<string, MessageEnvelope>(StringComparer.Ordinal);

			this.ConsumerCancelled += (sender, args) =>
			{
				LogAdapter.LogDebug("RpcHelper", "Consumer cancelled: " + args.ConsumerTag);
			};
		}

		public MessageEnvelope CallRaw(byte[] data,
			string routingKey,
			IBasicProperties messageProperties,
			RpcSendOptions options)
		{
			// CreateBasicProperties doesnt need the lock
			var prop = messageProperties ?? _model.CreateBasicProperties();

			using(var @event = new AutoResetEvent(false))
			{
				prop.CorrelationId = Guid.NewGuid().ToString();
				prop.Expiration = options.Timeout.TotalMilliseconds.ToString();
				_waits[prop.CorrelationId] = @event;

				lock (_model)
				{
					var returnQueue = GetOrCreateReturnQueue(routingKey);
					prop.ReplyTo = returnQueue;
				}

				lock(_model) 
				{
					_model.BasicPublish(_exchange, routingKey, prop, data);
				}

				if (!@event.WaitOne(options.Timeout))
				{
					MessageEnvelope val;
					_replyData.TryRemove(prop.CorrelationId, out val);

					LogAdapter.LogDebug("RpcHelper", "Timeout'ed correlation id " + prop.CorrelationId + " for " + routingKey);

					throw new TimeoutException("Timeout waiting for reply.");
				}

				MessageEnvelope reply;
				_replyData.TryRemove(prop.CorrelationId, out reply);
				return reply;
			}
		}

		public TResponse CallTyped<TRequest, TResponse>(TRequest request,
			string routingKey,
			IBasicProperties properties,
			RpcSendOptions options)
		{
			options = options ?? RpcSendOptions.Default;

			try
			{
				var data = _serializer.TypedSerialize(request, properties);
				var reply = this.CallRaw(data, routingKey, properties, options);

				if (ErrorResponse.IsHeaderErrorFlag(reply.Properties))
				{
					HandleError(reply);
				}

				return _serializer.TypedDeserialize<TResponse>(reply.Body, reply.Properties);
			}
			catch(TimeoutException)
			{
				throw new TimeoutException("Timeout waiting for reply for Rpc call: " + typeof(TRequest).FullName);
			}
		}

		private void HandleError(MessageEnvelope reply)
		{
			if (reply.Body == null || reply.Body.Length == 0)
			{
				throw new Exception("Call failed");
			}

			// Note: ErrorResponse infra does not add the 
			//       type name to properties, so we cannot call .TypedDeserialize()
			var response = (ErrorResponse) _serializer.Deserialize(reply.Body, typeof(ErrorResponse), reply.Properties);
			throw response.Exception;
		}

		private string GetOrCreateReturnQueue(string routingKey)
		{
			string queueName;
			if (_routing2RetQueue.TryGetValue(routingKey, out queueName)) return queueName;

			queueName = _model.QueueDeclare();
			_routing2RetQueue[routingKey] = queueName;

			// starts a bare metal consumer with no acks
			var consumerTag = _model.BasicConsume(queueName, noAck: true, consumer: this);

			LogAdapter.LogDebug("RpcHelper", "Started consumer " + consumerTag + " temporary queue " + queueName + " for routing " + routingKey);

			return queueName;
		}

		//
		// IBasicConsumer implementation
		//

		public void HandleBasicDeliver(string consumerTag,
			ulong deliveryTag,
			bool redelivered,
			string exchange, string routingKey,
			IBasicProperties properties,
			byte[] body)
		{
			var correlationId = properties.CorrelationId;
			if (string.IsNullOrEmpty(correlationId))
			{
				throw new RabbitException("Invalid correlationId:   got a null or empty one");
			}

			AutoResetEvent @event;
			if (!_waits.TryRemove(correlationId, out @event))
			{
				// timeout'd - no need to move further
				LogAdapter.LogDebug("RpcHelper", "Could not find wait for correlation " + correlationId +
				                                 " either it was timeout'ed, or the message was consumed by a outlier subscriber to " +
				                                 routingKey);
				return;
			}

			// hold reply
			_replyData[correlationId] = new MessageEnvelope(properties, body)
			{
				ConsumerTag = consumerTag,
				DeliveryTag = deliveryTag,
				ExchangeName = exchange,
				IsRedelivery = redelivered,
				RoutingKey = routingKey
			};

			try
			{
				@event.Set(); // may have been disposed
			}
			catch(Exception)
			{
				// potential object disposed

				MessageEnvelope val;
				_replyData.TryRemove(correlationId, out val);
			}
		}

		public IModel Model
		{
			get { return _model; }
		}

		public void HandleModelShutdown(object model, ShutdownEventArgs reason)
		{
			LogAdapter.LogWarn("RpcHelper", "Model Shutdown. Reason: " + reason);

			ResetQueueCache();
		}

		public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

		public void HandleBasicCancel(string consumerTag)
		{
		}

		public void HandleBasicCancelOk(string consumerTag)
		{
		}

		public void HandleBasicConsumeOk(string consumerTag)
		{
		}

		private void ResetQueueCache()
		{
			LogAdapter.LogWarn("RpcHelper", "Reseting return queue cache.");

			_routing2RetQueue.Clear();
		}
	}
}