namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

    public class QueueSubscription : IDisposable
    {
        private readonly IModel _model;
        private readonly string _consumerTag;
        private volatile bool _disposed;

        public QueueSubscription(IModel model, string consumerTag)
        {
            _model = model;
            _consumerTag = consumerTag;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            _model.BasicCancel(_consumerTag);
        }
    }
}