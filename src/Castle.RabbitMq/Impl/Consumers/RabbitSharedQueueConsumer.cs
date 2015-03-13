namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    class RabbitSharedQueueConsumer : QueueingBasicConsumer
    {
        public RabbitSharedQueueConsumer(IModel model, IRabbitSerializer serializer)
            : base(model)
        {
        }
    }
}