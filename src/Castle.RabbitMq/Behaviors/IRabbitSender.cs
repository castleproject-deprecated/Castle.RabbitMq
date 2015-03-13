namespace Castle.RabbitMq
{
    using System;

    public class SendOptions : TransportOptions
    {
        internal static SendOptions Default = new SendOptions()
        {
            Mandatory = false,
            Persist = false,
            Immediate = false
        }; 

        public bool Persist { get; set; }
        public bool Immediate { get; set; }
        public bool Mandatory { get; set; }
    }

    public interface IRabbitSender
    {
        MessageInfo Send(byte[] body, string routingKey = "", 
                         MessageProperties properties = null, 
                         SendOptions options = null);

        MessageInfo Send<T>(T message, string routingKey = "", 
                            MessageProperties properties = null, 
                            SendOptions options = null) where T : class;

        TResponse SendRequest<TRequest, TResponse>(TRequest request, 
                                                   string routingKey = "", 
                                                   MessageProperties properties = null, 
                                                   SendOptions options = null) 
            where TRequest : class 
            where TResponse : class;

//        Action<int> ConfirmationCallback { get; set; }
    }

    public static class RabbitSenderExtensions
    {
        public static MessageInfo Send(this IRabbitSender source, byte[] body, string routingKey = "")
        {
            return null;
        }

        public static MessageInfo Send<T>(this IRabbitSender source, T message, string routingKey = "") where T : class
        {
            return null;
        }

        public static MessageInfo SendPersistent(this IRabbitSender source, byte[] body, string routingKey = "")
        {
            return null;
        }

        public static MessageInfo SendPersistent<T>(this IRabbitSender source, T message, string routingKey = "") where T : class
        {
            return null;
        }
    }
}