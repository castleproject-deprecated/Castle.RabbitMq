namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using Xunit;

    /// Using message transactions
    public class _8_UsingTransactions : ConnectorFixtureBase
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

    /// Using message transactions
    public class _9_Confirmation : ConnectorFixtureBase
    {
        [Fact]
        public void DeclareExchangeAndUseIt()
        {
            var channel = this.Connection.CreateChannel(withConfirmation: true);

            var exchange = channel.DeclareExchange("my9exchange", new ExchangeOptions()
            {
                // using default options = ephemeral
            });

            var msg = new MyDumbMessage();
            exchange.Send(msg, confirmationCallback: () =>
            {
                
            }); // message will be dropped
        }
    }
}