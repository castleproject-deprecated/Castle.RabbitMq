namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;

    public class RabbitConnection : IRabbitConnection
    {
        private readonly List<WeakReference<IModel>> _models = new List<WeakReference<IModel>>();
        private readonly IConnection _connection;
        private volatile bool _isDisposed;

        public RabbitConnection(IConnection connection)
        {
            _connection = connection;
        }

        public IRabbitChannel CreateChannel(bool withConfirmation = false, ushort? prefetchCount = null)
        {
            EnsureNotDisposed();

            const ushort defaultPrefetch = 50;

            if (withConfirmation)
                throw new NotImplementedException();

            IModel model = _connection.CreateModel();

            if (!prefetchCount.HasValue)
            {
                prefetchCount = defaultPrefetch;
            }

            model.BasicQos(0, prefetchCount.Value, false);

            _models.Add(new WeakReference<IModel>(model));

            return new RabbitChannel(model);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            foreach (var weakReference in _models)
            {
                IModel model;
                if (weakReference.TryGetTarget(out model))
                {
                    model.Dispose();
                }
            }
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("RabbitConnection");
        }
    }
}