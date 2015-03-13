namespace Castle.RabbitMq
{
    public interface IRabbitQueue : IRabbitSender, IRabbitQueueConsumer, IDestroyable
    {
        string Name { get; }
        uint ConsumerCount { get; }
        uint MessageCount { get; }
    }
}