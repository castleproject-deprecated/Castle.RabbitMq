namespace Castle.RabbitMq.WindsorIntegration.Impl
{
    using System;
    using Castle.RabbitMq.Messaging;

    public abstract class MessageHandlerInvoker
    {
        public abstract void Invoke(Type msgType, IMessage message, IMessageHandler handler);
    }

    public class DefaultMessageHandlerInvoker : MessageHandlerInvoker
    {
        public override void Invoke(Type msgType, IMessage message, IMessageHandler handler)
        {
            var method = handler.GetType().GetMethod("Handle", new[] { msgType });

            method.Invoke(handler, new object[] { message });
        }
    }

    public class TransactionalMessageHandlerInvoker : MessageHandlerInvoker
    {
        public override void Invoke(Type msgType, IMessage message, IMessageHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
