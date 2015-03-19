namespace Castle.RabbitMq
{
    public interface IRabbitQueue : /*IRabbitSender,*/ IRabbitQueueConsumer
    {
        string Name { get; }
        uint ConsumerCount { get; }
        uint MessageCount { get; }

        void Purge();

        void Delete();
        void Delete(bool ifUnused, bool ifEmpty);
    }
}