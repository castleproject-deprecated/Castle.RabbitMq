namespace Castle.RabbitMq
{
    public interface IQueueDeclarer
    {
        IRabbitQueue DeclareQueue(string name, QueueOptions options);

        IRabbitQueue DeclareEphemeralQueue(QueueOptions options);
    }
}