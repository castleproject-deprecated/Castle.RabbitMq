namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
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

        public IRabbitExchange DeclareExchange(string name, RabbitExchangeOptions options)
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareQueue(IRabbitExchange exchange, string name, RabbitQueueOptions options)
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options)
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareEphemeralQueue(IRabbitExchange exchange, RabbitQueueOptions options)
        {
            throw new NotImplementedException();
        }

        public void Bind(IRabbitExchange exchange, IRabbitQueue queue)
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
}