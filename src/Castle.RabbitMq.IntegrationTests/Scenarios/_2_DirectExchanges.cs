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
            var exchange = channel.DeclareExchange("my2exchange", new ExchangeOptions()
            {
                // using default options = ephemeral
            });
            var queue = exchange.DeclareQueue("namedqueue", new QueueOptions()
            {

            });

            queue.Consume<MyDumbMessage>((env, ack) =>
            {
                Console.WriteLine("Received dumb message");

            }, new ConsumerOptions() {  });

            exchange.Send(new MyDumbMessage());
        }
    }
}