namespace Castle.RabbitMq
{
    public interface IExchangeQueueBinding
    {
        void Unbind(string routingKey = null);
    }
}