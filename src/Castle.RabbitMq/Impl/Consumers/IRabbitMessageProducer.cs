namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    /// <summary>
    /// Union of two behaviors, just to make our life easier
    /// </summary>
    interface IRabbitMessageProducer<T> : IBasicConsumer, IMessageProducer<T>
    {
    }
}