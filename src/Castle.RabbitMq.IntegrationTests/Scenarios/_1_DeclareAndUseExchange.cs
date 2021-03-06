﻿namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using Xunit;

    public class _1_DeclareAndUseExchange : IClassFixture<ConnectorFixture>
    {
        public _1_DeclareAndUseExchange(ConnectorFixture connectorFixture)
        {
            this.Connection = connectorFixture.Connection;
        }

        public IRabbitConnection Connection { get; set; }

        [Fact]
        public void DeclareExchangeAndUseIt()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("myfirstexchange", new ExchangeOptions()
            {
                // using default options = ephemeral
            });

            exchange.Send(new MyDumbMessage()); // message will be dropped

            exchange.Delete();
        }

        [Fact]
        public void UseDefaultExchange()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DefaultExchange;

            exchange.Send(new MyDumbMessage()); // message will be dropped
        }
    }

    // Orthogonal:
    //  Using a custom logger
    //  Using a custom serializer
    //  Using a custom type resolver
}
