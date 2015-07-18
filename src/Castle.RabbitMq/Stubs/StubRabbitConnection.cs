namespace Castle.RabbitMq.Stubs
{
	using System;
	using System.Collections.Generic;

	public class StubRabbitConnection : IRabbitConnection
	{
		private readonly List<StubRabbitChannel> _channelCreated = new List<StubRabbitChannel>();
		private readonly List<StubRabbitConnection> _connectionsCreated = new List<StubRabbitConnection>();
		
		private volatile bool _disposed;

		public StubRabbitConnection()
		{
		}

		// Stub helpers

		public bool Disposed { get { return _disposed; } }

		public List<StubRabbitConnection> ConnectionsCreated
		{
			get { return _connectionsCreated; }
		}

		public List<StubRabbitChannel> ChannelCreated
		{
			get { return _channelCreated; }
		}

		// end stub helpers

		public IRabbitChannel CreateChannel(ChannelOptions options = null)
		{
			EnsureNotDisposed();

			var channel = new StubRabbitChannel(options);
			_channelCreated.Add(channel);
			return channel;
		}

		public RabbitConnectionInfo ConnectionInfo
		{
			get { throw new NotImplementedException(); }
		}

		public IRabbitConnection NewConnection()
		{
			EnsureNotDisposed();

			var conn = new StubRabbitConnection();
			_connectionsCreated.Add(conn);
			return conn;
		}

		public void Dispose()
		{
			this._disposed = true;
		}

		private void EnsureNotDisposed()
		{
			if (_disposed) throw new ObjectDisposedException("StubRabbitConnection");
		}

	}
}