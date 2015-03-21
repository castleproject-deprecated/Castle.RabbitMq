namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    /// handling unrouted messages
    public class _4_HandlingUnroutedMessages : IClassFixture<ChannelFixture>
    {
        public _4_HandlingUnroutedMessages(ChannelFixture channelFix)
        {
            this.Channel = channelFix.Channel;
        }

        public IRabbitChannel Channel { get; set; }

        [Fact]
        public void DeclareExchangeAndSendMessage()
        {
            var eventTriggered = false;

            this.Channel.MessageUnrouted += args =>
            {
                eventTriggered = true;
            };

            var exchange = this.Channel.DeclareExchange("unboundexchange1", new ExchangeOptions()
            {
                // using default options = ephemeral
            });

            exchange.Send(new MyDumbMessage(), options: new SendOptions() { Mandatory = true }); // message will be dropped


            Thread.Sleep(100);

            eventTriggered.Should().BeTrue("event should have been triggered");
        }
    }
}