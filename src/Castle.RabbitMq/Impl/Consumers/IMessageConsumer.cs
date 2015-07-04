namespace Castle.RabbitMq
{
    using System;

    interface IMessageConsumer
    {
        void OnNext(MessageEnvelope message);
    }

//  interface IMessageConsumer<T>
//  {
//      void OnNext(MessageEnvelope<T> message);
//  }

    class ActionAdapter : IMessageConsumer
    {
        private readonly Action<MessageEnvelope>    _action;

        public ActionAdapter(Action<MessageEnvelope>    action)
        {
            _action = action;
        }

        public void OnNext(MessageEnvelope message)
        {
            _action(message);
        }
    }
}