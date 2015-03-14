namespace Castle.RabbitMq
{
    using System;

    interface IMessageProducer<T>
    {
        void Subscribe(IMessageConsumer<T> consumer);
    }
}