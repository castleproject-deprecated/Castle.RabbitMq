namespace Castle.RabbitMq.WindsorIntegration.Impl
{
    using System;
    using Castle.RabbitMq.Messaging;

    public abstract class MessageHandlerInvoker
    {
        public abstract void Invoke(Type msgType, IMessage message, IMessageHandler handler);
    }
}
