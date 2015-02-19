namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RabbitMQ.Client;

    public class RabbitChannel : IRabbitChannel
    {
        private readonly IModel _model;

        private volatile bool _disposed;

        public RabbitChannel(IModel model)
        {
            _model = model;

            
        }

        // _model.ExchangeDeclare(name, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);

        public IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options)
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareEphemeralQueue(RabbitQueueOptions options)
        {
            // _model.ExchangeDeclare(string.Empty, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);
        }

        public IRabbitExchange DeclareExchange(string name, RabbitExchangeOptions options)
        {
            var exchange = new RabbitExchange(name, options);

            _model.ExchangeDeclare(name, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);

            return exchange;
        }

        public IExchangeQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            if (_disposed) return;
        }

        private static string ToExchangeType(RabbitExchangeType exchangeType)
        {
            return exchangeType.ToString().ToLowerInvariant();
        }
    }

    public class RabbitExchange : IRabbitExchange
    {
        private readonly string _name;
        private readonly RabbitExchangeOptions _options;
        private readonly IModel _channel;

        public RabbitExchange(string name, RabbitExchangeOptions options, IModel channel)
        {
            _name = name;
            _options = options;
            _channel = channel;
        }

        public MessageInfo Send<T>(T message, string routing = "", bool persist = false, bool mandatory = false,
            MessageProperties properties = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options)
        {
            var result = _channel.QueueDeclare(name, options.Durable, options.IsExclusive, options.AutoDelete, options.Arguments);
        }

        public IRabbitQueue DeclareEphemeralQueue(RabbitQueueOptions options)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}