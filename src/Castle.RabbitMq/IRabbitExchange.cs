namespace Castle.RabbitMq
{
    public interface IRabbitExchange : IRabbitSender, IRabbitQueueDeclarer, IDestroyable
    {
        string Name { get; }
    }
}