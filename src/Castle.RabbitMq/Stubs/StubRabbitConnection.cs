namespace Castle.RabbitMq.Stubs
{
	using System.Collections.Generic;

	public class StubRabbitConnection : IRabbitConnection
	{
		private readonly List<StubRabbitChannel> _channelCreated = new List<StubRabbitChannel>();

		public StubRabbitConnection()
		{
		}

		// Stub helpers

		public bool Disposed { get; private set; }

		public List<StubRabbitChannel> ChannelCreated
		{
			get { return _channelCreated; }
		}

		// end stub helpers

		public IRabbitChannel CreateChannel(ChannelOptions options = null)
		{
			var channel = new StubRabbitChannel(options);
			_channelCreated.Add(channel);
			return channel;
		}

		public IRabbitConsole Console
		{
			get { throw new System.NotImplementedException(); }
		}

		public IRabbitConnection NewConnection()
		{
			return new StubRabbitConnection();
		}

		public void Dispose()
		{
			this.Disposed = true;
		}

	}
}