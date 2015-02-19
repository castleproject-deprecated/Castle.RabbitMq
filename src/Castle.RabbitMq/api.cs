namespace Castle.RabbitMq
{
    using System;

    public static class RabbitChannelExtensions
    {
        public static IRabbitExchange DeclareExchange(this IRabbitChannel source, RabbitExchangeOptions options)
        {
            return source.DeclareExchange(string.Empty, options);
        }
    }

    public interface IRabbitChannel : IQueueDeclarer, IDisposable
    {
        // OnException?
        // OnMessageNotDelivered --> OnMessageUnrouted

        IRabbitExchange DeclareExchange(string name, RabbitExchangeOptions options);

//        IRabbitQueue DeclareQueue(IRabbitExchange exchange, string name, RabbitQueueOptions options);

        /// <summary>
        /// Uses the default exchange
        /// </summary>
//        IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options);
//        IRabbitQueue DeclareEphemeralQueue(IRabbitExchange exchange, RabbitQueueOptions options);

        IExchangeQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null);
    }

    public interface IRabbitExchange : ISender, IQueueDeclarer, IDestroyable
    {
//        void Publish();
//        Task PublishAsync();
    }

    public interface IRabbitQueue : ISender, IConsumer, IDestroyable
    {
//        void Send();
//        Task SendAsync();

        // void Receive();

//        void Consume(Action<RabbitMessage, MessageAction> onMsgReceived);
    }

    public class MessageInfo
    {
        public int Tag { get; set; }
    }
}
