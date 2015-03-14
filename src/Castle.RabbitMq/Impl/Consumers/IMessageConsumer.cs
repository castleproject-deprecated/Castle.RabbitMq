namespace Castle.RabbitMq
{
    using System;

    interface IMessageConsumer<T>
    {
        void OnNext(MessageEnvelope<T> message);
    }

    class ActionAdapter<T> : IMessageConsumer<T>
    {
        private readonly Action<MessageEnvelope<T>> _action;

        public ActionAdapter(Action<MessageEnvelope<T>> action)
        {
            _action = action;
        }

        public void OnNext(MessageEnvelope<T> message)
        {
            _action(message);
        }
    }
}