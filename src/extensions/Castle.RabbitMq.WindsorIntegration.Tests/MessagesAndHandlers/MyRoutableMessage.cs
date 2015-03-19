namespace Castle.RabbitMq.WindsorIntegration.Tests
{
    using Castle.RabbitMq.Messaging;

    public class MyRoutableMessage : IMessage, IRoutable
    {
        public string RoutingKey { get; set; }
    }
}