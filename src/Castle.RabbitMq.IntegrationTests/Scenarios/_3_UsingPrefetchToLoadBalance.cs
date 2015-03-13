namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using System.Threading;
    using Xunit;

    // Using prefetch = 1 to load balance
    public class _3_UsingPrefetchToLoadBalance : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareAndUseIt()
        {
            var channel = this.Connection.CreateChannel(prefetchCount: 1);

            var exchange = channel.DeclareExchange("my3exchange", new ExchangeOptions()
            {
                // using default options = ephemeral
            });
            var queue = exchange.DeclareQueue("namedqueue3", new QueueOptions()
            {
            });

            queue.Consume(new Action<MessageEnvelope<MyDumbMessage>>(env =>
            {
                Console.WriteLine("Received dumb message 1");
                Thread.Sleep(1000);

            }), new ConsumerOptions());
            queue.Consume(new Action<MessageEnvelope<MyDumbMessage>>(env =>
            {
                Console.WriteLine("Received dumb message 2");
                Thread.Sleep(1000);

            }), new ConsumerOptions());

            exchange.Send(new MyDumbMessage());
            exchange.Send(new MyDumbMessage());
            exchange.Send(new MyDumbMessage());
            exchange.Send(new MyDumbMessage());
        }
    }
}