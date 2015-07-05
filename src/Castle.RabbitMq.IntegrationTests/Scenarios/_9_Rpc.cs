namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
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

            channelServer.DeclareQueue("rpc_1")
                .Respond<MyRequest, MyResponse>((env, ack) =>
                {
                    msgs++;
                    return new MyResponse();

                }, new ConsumerOptions());


            var reply = channelClient.DefaultExchange
                .Call<MyRequest, MyResponse>(new MyRequest(), "rpc_1");

            msgs.Should().Be(1);
	        reply.Should().BeOfType<MyResponse>();
        }

		[Fact]
		public void CalleeThrowsExceptions()
		{
			var channelClient = this.Connection.CreateChannel();
			var channelServer = this.Connection.CreateChannel();

			channelServer.DeclareQueue("rpc_2")
				.Respond<MyRequest, MyResponse>((env, ack) =>
				{
					throw new Exception("fake exception");

				}, new ConsumerOptions());

			Xunit.Assert.Throws<Exception>(() => channelClient.DefaultExchange
				.Call<MyRequest, MyResponse>(new MyRequest(), "rpc_2"))
			.Message.Should().Be("fake exception");
		}  
    }
}