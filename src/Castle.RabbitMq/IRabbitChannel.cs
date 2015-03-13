namespace Castle.RabbitMq
{
    using System;

    public interface IRabbitChannel : IRabbitQueueDeclarer, IDisposable
    {
        // OnException?
        // OnMessageNotDelivered --> OnMessageUnrouted

        IRabbitExchange DefaultExchange { get; }

        IRabbitExchange DeclareExchange(string name, ExchangeOptions options);

        IRabbitQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null);
    }

    public static class RabbitChannelExtensions
    {
        public static IRabbitExchange DeclareExchange(this IRabbitChannel source, string name, RabbitExchangeType exchangeType)
        {
            return source.DeclareExchange(string.Empty, new ExchangeOptions()
            {
                ExchangeType = exchangeType,
                // defaults from the original api:
                Durable = false,
                AutoDelete = false
            });
        }
    }

}