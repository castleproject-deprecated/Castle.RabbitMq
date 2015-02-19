namespace Castle.RabbitMq
{
    using System;

    public static class RabbitChannelExtensions
    {
        public static IRabbitExchange DeclareExchange(this IRabbitChannel source, ExchangeOptions options)
        {
            return source.DeclareExchange(string.Empty, options);
        }
    }

    public interface IRabbitChannel : IQueueDeclarer, IDisposable
    {
        // OnException?
        // OnMessageNotDelivered --> OnMessageUnrouted

        IRabbitExchange DeclareExchange(string name, ExchangeOptions options);

//        IRabbitQueue DeclareQueue(IRabbitExchange exchange, string name, QueueOptions options);

        /// <summary>
        /// Uses the default exchange
        /// </summary>
//        IRabbitQueue DeclareQueue(string name, QueueOptions options);
//        IRabbitQueue DeclareEphemeralQueue(IRabbitExchange exchange, QueueOptions options);

        IExchangeQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null);
    }

    public interface IRabbitExchange : ISender, IQueueDeclarer, IDestroyable
    {
//        void Publish();
//        Task PublishAsync();
    }

    public interface IRabbitQueue : ISender, IConsumer, IDestroyable
    {
        string Name { get; }
        uint ConsumerCount { get; }
        uint MessageCount { get; }

//        void Send();
//        Task SendAsync();

        // void Receive();

//        void Consume(Action<RabbitMessage, MessageAck> onMsgReceived);
    }

    public class MessageInfo
    {
        public int Tag { get; set; }
    }
}
