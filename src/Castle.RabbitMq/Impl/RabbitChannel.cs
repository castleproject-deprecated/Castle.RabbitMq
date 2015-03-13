namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

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
        }

        #region IRabbitChannel

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
            EnsureNotDisposed();

            lock (_model)
            {
                _model.ExchangeDeclare(name, options.ExchangeType.ToStr());
            }
            return new RabbitExchange(_model, _defaultSerializer, name, true, options);
        }

        public IRabbitQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
            EnsureNotDisposed();

            lock (_model)
            {
                _model.QueueBind(queue.Name, exchange.Name, routingKeyOrFilter);
            }
            return new RabbitQueueBinding(_model, queue.Name, exchange.Name, routingKeyOrFilter);
        }

        #endregion

        #region IRabbitQueueDeclarer

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            EnsureNotDisposed();

            options = options ?? QueueOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            lock (_model)
            {
                var result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
                return new RabbitQueue(_model, _defaultExchange, serializer, result, options);
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

        private void EnsureNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("RabbitConnection");
        }
    }
}