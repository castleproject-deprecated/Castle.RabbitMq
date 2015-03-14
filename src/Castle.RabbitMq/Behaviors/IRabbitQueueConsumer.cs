namespace Castle.RabbitMq
{
    using System;

    public class TransportOptions
    {
        public IRabbitSerializer Serializer { get; set; }
    }

    public class ConsumerOptions : TransportOptions
    {
        internal static ConsumerOptions Default = new ConsumerOptions();

        // bool exclusive
        // IDictionary<string, object> arguments

        public ConsumerOptions()
        {
            this.NoAck = true;
        }

        /// <summary>
        /// Note that if the "noAck" option is enabled (which it is by default), 
        /// then received deliveries are automatically acked within the 
        /// server before they are even transmitted across the network to us. 
        /// </summary>
        public bool NoAck { get; set; }
    }

    public interface IRabbitQueueConsumer
    {
        /// <summary>
        /// For Rpc
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="onRespond"></param>
        /// <param name="options"></param>
        Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond, 
                                                  ConsumerOptions options) 
            where TRequest : class 
            where TResponse : class;

        /// <summary>
        /// For pure message consumption
        /// </summary>
        /// <param name="onReceived"></param>
        /// <param name="options"></param>
        Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived, 
                                ConsumerOptions options);

        // MessageEnvelope<T> Receive<T>() where T : class;
        // MessageEnvelope<T> Peek<T>() where T : class;
    }

    public static class RabbitQueueConsumerExtensions
    {
        // TODO: move this one to Extension method
        // void Consume<T>(Action<MessageEnvelope<T>> onMsgReceived, ConsumerOptions options) where T : class;

        public static Subscription Consume<T>(this IRabbitQueueConsumer source,
                                              Action<MessageEnvelope<T>, IMessageAck> onReceived)
        {
            return source.Consume<T>(onReceived, null);
        }
    }
}