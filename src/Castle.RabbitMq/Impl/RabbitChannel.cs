namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitChannel : IRabbitChannel
    {
        private readonly IModel _model;
        private readonly IRabbitSerializer _defaultSerializer;
        private readonly IRabbitExchange _defaultExchange;
        private volatile bool _isDisposed;

        public RabbitChannel(IModel model, IRabbitSerializer defaultSerializer)
        {
            _model = model;
            _defaultSerializer = defaultSerializer;
            _defaultExchange = new RabbitExchange(_model, _defaultSerializer, 
                name: string.Empty, canDestroy: false, 
                options: new ExchangeOptions());

            _model.BasicReturn += ModelOnBasicReturn;
        }

        #region IRabbitChannel

        public event Action<MessageUnroutedEventArgs> MessageUnrouted;

        public IRabbitExchange DefaultExchange
        {
            get
            {
                EnsureNotDisposed();
                return _defaultExchange;
            }
        }

        public IRabbitExchange DeclareExchange(string name, ExchangeOptions options)
        {
            Argument.NotNullOrEmpty(name, "name");

            EnsureNotDisposed();

            lock (_model)
            {
                _model.ExchangeDeclare(name, options.ExchangeType.ToStr(), options.Durable, options.AutoDelete, options.Arguments);
            }

            return new RabbitExchange(_model, _defaultSerializer, name, true, options);
        }

		public IRabbitExchange DeclareExchangeNoWait(string name, ExchangeOptions options)
		{
			Argument.NotNullOrEmpty(name, "name");

			EnsureNotDisposed();

			lock (_model)
			{
				_model.ExchangeDeclareNoWait(name, options.ExchangeType.ToStr(), options.Durable, options.AutoDelete, options.Arguments);
			}

			return new RabbitExchange(_model, _defaultSerializer, name, true, options);
		}

        public IRabbitQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
	        return BindInternal(queue.Name, exchange.Name, routingKeyOrFilter);
        }

        public void UnBind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
            EnsureNotDisposed();

            lock (_model)
                _model.QueueUnbind(queue.Name, exchange.Name, routingKeyOrFilter, null);
        }

		public IModel Model { get { return _model; } }

        #endregion

        #region IRabbitQueueDeclarer

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            Argument.NotNullOrEmpty(name, "name");

            EnsureNotDisposed();

            options = options ?? QueueOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            lock (_model)
            {
                var result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
                return new RabbitQueue(_model, serializer, result, options);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            lock (_model)
            {
                _model.Abort();
                _model.Dispose();
            }
        }

		internal RabbitQueueBinding BindInternal(string queue, string exchange, string routingKeyOrFilter)
	    {
			EnsureNotDisposed();

			lock (_model)
				_model.QueueBind(queue, exchange, routingKeyOrFilter);

			return new RabbitQueueBinding(_model, queue, exchange, routingKeyOrFilter);
		}

        private void ModelOnBasicReturn(object sender, BasicReturnEventArgs args)
        {
            if (LogAdapter.LogEnabled)
            {
                LogAdapter.LogDebug("RabbitChannel", 
                    "Message dropped. Message sent to exchange " + args.Exchange + " with routing key " + args.RoutingKey, 
                    null);
            }

            var ev = this.MessageUnrouted;
            if (ev == null) return;

            var envelope = new MessageEnvelope(args.BasicProperties, args.Body);
            var eventArgs = new MessageUnroutedEventArgs()
            {
                MessageEnvelope = envelope,
                Exchange = args.Exchange,
                ReplyCode = args.ReplyCode,
                ReplyText = args.ReplyText,
                RoutingKey = args.RoutingKey
            };
                
            ev(eventArgs);
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("RabbitChannel");
        }
    }
}