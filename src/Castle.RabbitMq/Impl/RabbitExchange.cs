namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;


	[System.Diagnostics.DebuggerDisplay("Exchange '{Name}' {_options}", Name = "Exchange")]
	public class RabbitExchange : IRabbitExchange
	{
		private readonly IModel _model;
		private readonly IRabbitSerializer _defaultSerializer;
		private readonly bool _canDestroy;
		private readonly ExchangeOptions _options;
		private readonly RpcHelper _rpcHelper;
		private readonly bool _isDefaultExchange;

		public RabbitExchange(IModel model, IRabbitSerializer serializer,
			string name, bool canDestroy, ExchangeOptions options)
		{
			this.Name = name;

			_model = model;
			_defaultSerializer = serializer;
			_canDestroy = canDestroy;
			_options = options;
			_isDefaultExchange = name == string.Empty;

			_rpcHelper = new RpcHelper(_model, this.Name, serializer);
		}

		#region IRabbitExchange

		public string Name { get; private set; }

		public IRabbitQueueBinding Bind(IRabbitQueue queue, string routingKeyOrFilter)
		{
			lock(_model)
				_model.QueueBind(queue.Name, this.Name, routingKeyOrFilter);

			return new RabbitQueueBinding(_model, queue.Name, this.Name, routingKeyOrFilter);
		}

		public IRabbitQueueBinding BindNoWait(IRabbitQueue queue, string routingKeyOrFilter)
		{
			lock(_model)
				_model.QueueBindNoWait(queue.Name, this.Name, routingKeyOrFilter, null);

			return new RabbitQueueBinding(_model, queue.Name, this.Name, routingKeyOrFilter);
		}

		public void Delete()
		{
			if (!_canDestroy) return;

			lock(_model)
				_model.ExchangeDelete(this.Name);
		}

		public void Delete(bool ifUnused)
		{
			if (!_canDestroy) return;

			lock(_model)
				_model.ExchangeDelete(this.Name, ifUnused);
		}

		#endregion

		#region IRabbitSender

		public IRabbitSerializer DefaultSerializer
		{
			get { return _defaultSerializer; }
		}

		public MessageInfo SendRaw(byte[] body, string routingKey = "",
			IBasicProperties properties = null,
			SendOptions options = null)
		{
			Argument.NotNull(routingKey, "routingKey");

			options = options ?? SendOptions.Default;
			var prop = properties ?? _model.CreateBasicProperties();
			if (options.Persist)
			{
				prop.DeliveryMode = 2; // persistent
			}

			lock(_model)
			{
				var id = _model.NextPublishSeqNo;
				_model.BasicPublish(this.Name, routingKey,
					mandatory: options.Mandatory,
					basicProperties: prop,
					body: body);
				return new MessageInfo() {Tag = id};
			}
		}

		public MessageInfo Send<T>(T message, string routingKey = "",
			IBasicProperties properties = null,
			SendOptions options = null)
			where T : class
		{
			options = options ?? SendOptions.Default;
			var serializer = options.Serializer ?? _defaultSerializer;
			var prop = properties ?? _model.CreateBasicProperties();
			var data = serializer.TypedSerialize(message, prop);

			return SendRaw(data, routingKey, prop, options);
		}

		public MessageEnvelope CallRaw(byte[] data, string routingKey = "",
			IBasicProperties properties = null,
			RpcSendOptions options = null)
		{
			Argument.NotNull(routingKey, "routingKey");
			properties = properties ?? _model.CreateBasicProperties();

			return _rpcHelper.CallRaw(data, routingKey, properties, options);
		}

		public TResponse Call<TRequest, TResponse>(TRequest request, string routingKey = "",
			IBasicProperties properties = null,
			RpcSendOptions options = null)
			where TRequest : class
			where TResponse : class
		{
			Argument.NotNull(routingKey, "routingKey");
			properties = properties ?? _model.CreateBasicProperties();

			return _rpcHelper.CallTyped<TRequest, TResponse>(request, routingKey, properties, options);
		}

		#endregion

		#region IRabbitQueueDeclarer

		public IRabbitQueue DeclareQueue(string name, QueueOptions options)
		{
			return DeclareQueueInternal(false, name, options);
		}

		public IRabbitQueue DeclareQueueNoWait(string name, QueueOptions options)
		{
			return DeclareQueueInternal(true, name, options);
		}

		#endregion

		internal IRabbitQueue DeclareQueueInternal(bool nowait, string name, QueueOptions options)
		{
			Argument.NotNull(name, "name");
			if (nowait && name == String.Empty) throw new ArgumentException("Auto gen queues cannot be declared with no-wait");

			options = options ?? QueueOptions.Default;

			var serializer = options.Serializer ?? _defaultSerializer;

			lock(_model)
			{
				QueueDeclareOk result;

				if (nowait)
				{
					_model.QueueDeclareNoWait(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
					result = new QueueDeclareOk(name, 0, 0);
				}
				else
				{
					result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
				}

				if (!this._isDefaultExchange)
				{
					// binds it to "this" exchange
					_model.QueueBind(result.QueueName, this.Name, "");
				}

				return new RabbitQueue(_model, serializer, result, options);
			}
		}
	}
}