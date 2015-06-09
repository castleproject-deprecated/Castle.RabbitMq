namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using MgmtConsole;
	using RabbitMQ.Client;

	public class RabbitConnection :	IRabbitConnection
	{
		private	readonly List<WeakReference<IModel>> _models = new List<WeakReference<IModel>>();
		private	readonly Lazy<HttpBasedRabbitConsole> _console;
		private	readonly IConnection _connection;
		private	readonly ConnectionFactory _connInfo;
		private	volatile bool _isDisposed;

		public RabbitConnection(IConnection	connection,	ConnectionFactory connInfo)
		{
			_connection	= connection;
			_connInfo =	connInfo;
			_console = new Lazy<HttpBasedRabbitConsole>(() => new HttpBasedRabbitConsole(connInfo)); 
		}

		public IRabbitConsole Console
		{
			get	{ return _console.Value; }
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

		public void	Dispose()
		{
			if (_isDisposed) return;

			_isDisposed	= true;

			if (_console.IsValueCreated)
			{
				_console.Value.Dispose();
			}

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

		private	void EnsureNotDisposed()
		{
			if (_isDisposed) throw new ObjectDisposedException("RabbitConnection");
		}
	}
}