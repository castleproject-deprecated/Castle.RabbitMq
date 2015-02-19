namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
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
            var exchange = channel.DeclareExchange("myfirstexchange", new ExchangeOptions()
            {
                // using default options = ephemeral
            });

            exchange.Send(new MyDumbMessage()); // message will be dropped
        }
    }


    // Orthogonal:
    //  Using a custom logger
    //  Using a custom serializer
    //  Using a custom type resolver


    class MyDumbMessage
    {
    }
}
