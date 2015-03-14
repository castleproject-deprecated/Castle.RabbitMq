namespace Castle.RabbitMq
{
    using System;

    public interface IRabbitSender
    {
        MessageInfo Send(byte[] body, string routingKey = "", 
                         MessageProperties properties = null, 
                         SendOptions options = null);

        MessageInfo Send<T>(T message, string routingKey = "", 
                            MessageProperties properties = null, 
                            SendOptions options = null) where T : class;

        MessageEnvelope SendRequest(byte[] data, string routingKey = "",
                                    MessageProperties properties = null,
                                    RpcSendOptions options = null);

        TResponse SendRequest<TRequest, TResponse>(TRequest request, 
                                                   string routingKey = "", 
                                                   MessageProperties properties = null,
                                                   RpcSendOptions options = null) 
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