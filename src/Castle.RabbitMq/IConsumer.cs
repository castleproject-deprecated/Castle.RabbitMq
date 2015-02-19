namespace Castle.RabbitMq
{
    using System;

    public interface IConsumer
    {
        MessageEnvelope<T> Receive<T>() where T : class;
        MessageEnvelope<T> Peek<T>() where T : class;

        // TODO: move this one to Extension method
        void Consume<T>(Action<MessageEnvelope<T>> onMsgReceived, ConsumeOptions options) where T : class;

        // Designated Consume overload
        void Consume<T>(Action<MessageEnvelope<T>, MessageAction> onMsgReceived, ConsumeOptions options) where T : class;

        // Designated Consume overload
        void ConsumeRaw(Action<MessageEnvelope, MessageAction> onMsgReceived, ConsumeOptions options);
    }
}