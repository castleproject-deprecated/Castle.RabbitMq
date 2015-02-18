namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRabbitChannel : IDisposable
    {
        // OnException?
        // OnMessageNotDelivered?

        IRabbitExchange DeclareExchange(string name, RabbitExchangeOptions options);

        IRabbitQueue DeclareQueue(IRabbitExchange exchange, string name, RabbitQueueOptions options);

        /// <summary>
        /// Uses the default exchange
        /// </summary>
        IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options);

        IRabbitQueue DeclareEphemeralQueue(IRabbitExchange exchange, RabbitQueueOptions options);

        void Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter);
    }

    

    public class MessageInfo
    {
        public int Tag { get; set; }
    }

    public interface ISender
    {
        MessageInfo Send<T>(T message, bool persist = false) where T : class;
//        Task<MessageInfo> SendAsync<T>(T message, bool persist = false) where T : class;

        MessageInfo SendRaw(byte[] body, bool persist = false);
    }

    public interface IConsumer
    {
        void Receive();
        void Peek();
        void Consume(Action<RabbitMessage, MessageAction> onMsgReceived);
    }

    public interface IRabbitExchange : ISender
    {
//        void Publish();
//        Task PublishAsync();
    }

    public interface IRabbitQueue : ISender, IConsumer
    {
//        void Send();
//        Task SendAsync();

        // void Receive();

//        void Consume(Action<RabbitMessage, MessageAction> onMsgReceived);
    }

    public class RabbitMessage
    {
    }

    public class MessageAction
    {
        public void Ack()
        {
        }

        public void Reject()
        {
        }
    }


    public class RabbitQueueOptions
    {
        public bool IsExclusive { get; set; }

        IRabbitSerializer Serializer { get; set; }
    }

    


    public class RabbitExchangeOptions
    {
        public RabbitExchangeType ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }

    public enum RabbitExchangeType
    {
        Direct, Fanout, Headers, Topic
    }
}
