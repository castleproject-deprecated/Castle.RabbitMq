namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    public class ConnectorFixture : IDisposable
    {
        public ConnectorFixture()
        {
            this.Connection = RabbitConnector.Connect("localhost", vhost: "/castle");
        }

        public IRabbitConnection Connection { get; set; }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                this.Connection = null;
            }
        }
    }

    public abstract class ConnectorFixtureBase : Xunit.IUseFixture<ConnectorFixture>
    {
        public IRabbitConnection Connection { get; set; }

        public void SetFixture(ConnectorFixture data)
        {
            Connection = data.Connection;
        }
    }

    public class _1_DeclareAndUseExchange : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareExchangeAndUseIt()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("myfirstexchange", new RabbitExchangeOptions()
            {
                // using default options = ephemeral
            });

            exchange.Send(new MyDumbMessage()); // message will be dropped
        }
    }


    /// handling unrouted messages
    public class _4_HandlingUnroutedMessages : ConnectorFixtureBase
    {
        
    }


    /// using custom properties (userid, timestamp, persistent, priority, headers)
    public class _5_UsingMessageCustomProperties : ConnectorFixtureBase
    {

    }

    /// Pub sub with fanout exchanges
    public class _6_PubSubWithFanoutExchange : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareAndUseIt()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("my6exchange", new RabbitExchangeOptions()
            {
                // using default options = ephemeral
                ExchangeType = RabbitExchangeType.Fanout
            });

            var queue1 = exchange.DeclareQueue("q1", new RabbitQueueOptions());
            var queue2 = exchange.DeclareQueue("q2", new RabbitQueueOptions());
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

    // Using expirations on message


    // Using message transactions



    // Orthogonal:
    //  Using a custom logger
    //  Using a custom serializer
    //  Using a custom type resolver


    class MyDumbMessage
    {
    }
}
