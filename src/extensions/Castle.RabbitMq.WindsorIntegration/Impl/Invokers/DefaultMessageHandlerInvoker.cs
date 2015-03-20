namespace Castle.RabbitMq.WindsorIntegration.Impl
{
    using System;
    using Messaging;

    public class DefaultMessageHandlerInvoker : MessageHandlerInvoker
    {
        public override void Invoke(Type msgType, IMessage message, IMessageHandler handler)
        {
            // PERF: cache or create compiled Lambda to invoke this
            var method = handler.GetType().GetMethod("Handle", new[] { msgType });

            method.Invoke(handler, new object[] { message });
        }
    }
}