namespace Castle.RabbitMq
{
    public interface IQueueDeclarer
    {
        IRabbitQueue DeclareQueue(string name, RabbitQueueOptions options);

        IRabbitQueue DeclareEphemeralQueue(RabbitQueueOptions options);
    }
}