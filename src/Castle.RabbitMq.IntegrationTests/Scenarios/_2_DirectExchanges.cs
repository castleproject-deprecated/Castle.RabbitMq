namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using Xunit;

    // Using direct queues
    public class _2_DirectExchanges : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareAndUseIt()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("my2exchange", new RabbitExchangeOptions()
            {
                // using default options = ephemeral
            });
            var queue = exchange.DeclareQueue("namedqueue", new RabbitQueueOptions()
            {

            });

            queue.Consume(new Action<MessageEnvelope<MyDumbMessage>>(env =>
            {
                Console.WriteLine("Received dumb message");

            }), new ConsumeOptions());

            exchange.Send(new MyDumbMessage());
        }
    }
}