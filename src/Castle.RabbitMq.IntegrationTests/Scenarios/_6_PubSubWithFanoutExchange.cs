namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    /// Pub sub with fanout exchanges
    public class _6_PubSubWithFanoutExchange : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareAndUseIt()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("my6exchange", new ExchangeOptions()
            {
                // using default options = ephemeral
                ExchangeType = RabbitExchangeType.Fanout
            });

            var queue1 = exchange.DeclareQueue("q1", new QueueOptions());
            var queue2 = exchange.DeclareQueue("q2", new QueueOptions());
            var wait = new SemaphoreSlim(2, 2);
            var receivedCount = 0;

            queue1.Consume(new Action<MessageEnvelope<MyDumbMessage>>(env =>
            {
                wait.Release();
                Interlocked.Increment(ref receivedCount);
            }), new ConsumeOptions());
            queue2.Consume(new Action<MessageEnvelope<MyDumbMessage>>(env =>
            {
                wait.Release();
                Interlocked.Increment(ref receivedCount);
            }), new ConsumeOptions());

            exchange.Send(new MyDumbMessage()); // message will be dropped

            wait.Wait(TimeSpan.FromSeconds(2.0));

            receivedCount.Should().Be(2);
        }
    }
}