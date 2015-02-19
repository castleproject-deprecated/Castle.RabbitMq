namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using Xunit;

    /// Using expirations on message
    public class _7_UsingMessageExpiration : ConnectorFixtureBase
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
}