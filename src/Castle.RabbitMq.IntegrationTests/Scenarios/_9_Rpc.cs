namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    public class _9_Rpc : IClassFixture<ConnectorFixture>
    {
        public _9_Rpc(ConnectorFixture connectorFixture)
        {
            this.Connection = connectorFixture.Connection;
        }

        public IRabbitConnection Connection { get; set; }

        [Fact]
        public void BasicRequestReply()
        {
            var channelClient = this.Connection.CreateChannel();
            var channelServer = this.Connection.CreateChannel();

            var msgs = 0;
            var @event = new AutoResetEvent(false);

            channelServer.DeclareQueue("rpc_1")
                .Respond<MyRequest, MyResponse>((env, ack) =>
                {
                    msgs++;
                    @event.Set();
                    return new MyResponse();

                }, new ConsumerOptions());


            var reply = channelClient.DefaultExchange
                .SendRequest<MyRequest, MyResponse>(new MyRequest(), "rpc_1");

            @event.WaitOne(TimeSpan.FromSeconds(2));

            msgs.Should().Be(1);
        }        
    }
}