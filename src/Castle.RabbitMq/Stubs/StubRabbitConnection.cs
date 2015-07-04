namespace Castle.RabbitMq.Stubs
{
	public class StubRabbitConnection : IRabbitConnection
	{
		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public IRabbitChannel CreateChannel(ChannelOptions options = null)
		{
			throw new System.NotImplementedException();
		}

		public IRabbitConsole Console
		{
			get { throw new System.NotImplementedException(); }
		}

		public IRabbitConnection NewConnection()
		{
			throw new System.NotImplementedException();
		}
	}
}