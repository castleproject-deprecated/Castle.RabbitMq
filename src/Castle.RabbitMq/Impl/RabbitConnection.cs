namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;

	public class RabbitConnection :	IRabbitConnection
	{
		private	readonly List<WeakReference<IModel>> _models = new List<WeakReference<IModel>>();
		private	readonly IConnection _connection;
		private	readonly ConnectionFactory _connInfo;
		private	volatile bool _isDisposed;

		public RabbitConnection(IConnection	connection,	ConnectionFactory connInfo)
		{
			_connection	= connection;
			_connInfo =	connInfo;

			_connection.ConnectionBlocked += OnConnectionBlocked;
			_connection.ConnectionUnblocked += OnConnectionUnblocked;
		}

		public IRabbitChannel CreateChannel(ChannelOptions options)
		{
			EnsureNotDisposed();

			options	= options ?? ChannelOptions.Default;
			var	prefetchCount =	options.PrefetchCount;

			const ushort defaultPrefetch = 50;

			if (options.WithConfirmation)
				throw new NotImplementedException();

			IModel model = _connection.CreateModel();

			if (!prefetchCount.HasValue)
			{
				prefetchCount =	defaultPrefetch;
			}

			model.BasicQos(0, prefetchCount.Value, false);

			_models.Add(new	WeakReference<IModel>(model));

			return new RabbitChannel(model,	options.DefaultSerializer);
		}

		public IRabbitConnection NewConnection()
		{
			return new RabbitConnection(_connInfo.CreateConnection(), _connInfo);
		}

		public RabbitConnectionInfo ConnectionInfo
		{
			get { return new RabbitConnectionInfo(_connInfo.HostName, _connInfo.VirtualHost, _connInfo.UserName, _connInfo.Password, _connInfo.Port); }
		}

		public void	Dispose()
		{
			if (_isDisposed) return;

			_isDisposed	= true;

			foreach	(var weakReference in _models)
			{
				IModel model;
				if (weakReference.TryGetTarget(out model))
				{
					model.Dispose();
				}
			}

			_connection.Dispose();
		}

		private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
		{
			LogAdapter.LogWarn("Connection", "Connection blocked - broker running low on resources (memory or disk)");
		}
		private void OnConnectionUnblocked(object sender, EventArgs e)
		{
			LogAdapter.LogWarn("Connection", "Connection Unblocked - broker running is back on its feet");
		}

		private	void EnsureNotDisposed()
		{
			if (_isDisposed) throw new ObjectDisposedException("RabbitConnection");
		}
	}
}