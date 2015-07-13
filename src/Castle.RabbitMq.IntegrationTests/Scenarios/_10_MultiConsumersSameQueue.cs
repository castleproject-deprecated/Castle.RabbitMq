namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
	using System.Text;
	using System.Threading;
	using FluentAssertions;
	using Xunit;

	/// <summary>
	/// tests that the round robin of consumers is actually happening
	/// </summary>
	public class _10_MultiConsumersSameQueue : IClassFixture<ConnectorFixture>
	{
		public _10_MultiConsumersSameQueue(ConnectorFixture connectorFixture)
		{
			this.Connection = connectorFixture.Connection;
		}

		public IRabbitConnection Connection { get; set; }

		[Fact]
		public void BasicRequestReply()
		{
			var channelClient = this.Connection.CreateChannel();
			var channelServer = this.Connection.CreateChannel();

			var consumer1 = 0;
			var consumer2 = 0;

			var queue = channelServer.DeclareQueue("multicon_1");
			queue.ConsumeRaw((env, ack) =>
			{
				consumer1++;
			}, new ConsumerOptions());
			queue.ConsumeRaw((env, ack) =>
			{
				consumer2++;
			}, new ConsumerOptions());


			channelClient.DefaultExchange.SendRaw(Encoding.UTF8.GetBytes("a"), queue.Name);
			channelClient.DefaultExchange.SendRaw(Encoding.UTF8.GetBytes("b"), queue.Name);
			channelClient.DefaultExchange.SendRaw(Encoding.UTF8.GetBytes("c"), queue.Name);
			channelClient.DefaultExchange.SendRaw(Encoding.UTF8.GetBytes("d"), queue.Name);

			Thread.Sleep(100);

			consumer1.Should().Be(2);
			consumer2.Should().Be(2);
		}
	}
}