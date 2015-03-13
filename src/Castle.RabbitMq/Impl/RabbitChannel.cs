namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    public class RabbitChannel : IRabbitChannel
    {
        private readonly IModel _model;
        private readonly IRabbitExchange _defaultExchange;

        public RabbitChannel(IModel model)
        {
            _model = model;
            _defaultExchange = new RabbitExchange(_model, name: string.Empty, canDestroy: false, options: new ExchangeOptions());
        }

        // IRabbitChannel : IRabbitQueueDeclarer, IDisposable

        #region IRabbitChannel

        public IRabbitExchange DefaultExchange
        {
            get { return _defaultExchange; }
        }

        public IRabbitExchange DeclareExchange(string name, ExchangeOptions options)
        {
            lock (_model)
            {
                _model.ExchangeDeclare(name, options.ExchangeType.ToStr());
            }
            return new RabbitExchange(_model, name, true, options);
        }

        public IRabbitQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
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
            options = options ?? QueueOptions.Default;

            lock (_model)
            {
                var result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);
                return new RabbitQueue(_model, _defaultExchange, result, options);
            }
        }

        public IRabbitQueue DeclareEphemeralQueue(QueueOptions options)
        {
            options = options ?? new QueueOptions();
            options.AutoDelete = true;
            options.Exclusive = true;

            lock (_model)
            {
                var result = _model.QueueDeclare();

                return new RabbitQueue(_model, _defaultExchange, result, options);
            }
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
}