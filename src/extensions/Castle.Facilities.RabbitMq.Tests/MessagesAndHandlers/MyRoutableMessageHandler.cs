namespace Castle.Facilities.RabbitMq.Tests
{
    using Castle.RabbitMq.Messaging;

    public class MyRoutableMessageHandler : IMessageHandler<MyRoutableMessage>
    {
        public void Handle(MyRoutableMessage message)
        {
            Handled = true;
        }

        public static bool Handled { get; set; }
    }
}