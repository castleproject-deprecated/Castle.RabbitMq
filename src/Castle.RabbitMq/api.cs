namespace Castle.RabbitMq
{
    using System;

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

    public interface IRabbitChannel : IRabbitQueueDeclarer, IDisposable
    {
        // OnException?
        // OnMessageNotDelivered --> OnMessageUnrouted

        IRabbitExchange DefaultExchange { get; }

        IRabbitExchange DeclareExchange(string name, ExchangeOptions options);

        IRabbitQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null);
    }

    public interface IRabbitExchange : IRabbitSender, IRabbitQueueDeclarer, IDestroyable
    {
        string Name { get; }
    }

    public interface IRabbitQueue : IRabbitSender, IRabbitQueueConsumer, IDestroyable
    {
        string Name { get; }
        uint ConsumerCount { get; }
        uint MessageCount { get; }
    }

    public class MessageInfo
    {
        public ulong Tag { get; set; }
    }
}
