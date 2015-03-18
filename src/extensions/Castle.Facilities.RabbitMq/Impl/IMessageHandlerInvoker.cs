namespace Castle.RabbitMq.Extensions.MessageHandler
{
    using Messaging;

    public interface IMessageHandlerInvoker
    {
//        Type TargetMessage { get; }
        bool Invoke(IMessage message);
    }

    public class DefaultMessageHandlerInvoker : IMessageHandlerInvoker
    {
        public bool Invoke(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TransactionalMessageHandlerInvoker : IMessageHandlerInvoker
    {
        public bool Invoke(IMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}