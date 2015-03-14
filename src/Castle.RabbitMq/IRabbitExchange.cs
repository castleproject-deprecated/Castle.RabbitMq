namespace Castle.RabbitMq
{
    public interface IRabbitExchange : IRabbitSender, IRabbitQueueDeclarer
    {
        string Name { get; }

        IRabbitQueueBinding Bind(IRabbitQueue queue, string routingKeyOrFilter);

        void Delete();
        void Delete(bool ifUnused);
    }
}