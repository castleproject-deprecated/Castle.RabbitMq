namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    public class RabbitQueueBinding : IRabbitQueueBinding
    {
        private readonly IModel _model;
        private readonly string _queueName;
        private readonly string _exchangeName;
        private readonly string _routingKeyOrFilter;
        private volatile bool _unbound = false;

        public RabbitQueueBinding(IModel model, string queueName, string exchangeName, string routingKeyOrFilter)
        {
            _model = model;
            _queueName = queueName;
            _exchangeName = exchangeName;
            _routingKeyOrFilter = routingKeyOrFilter;
        }

        public void Unbind()
        {
            if (_unbound) return;

            lock (_model)
            {
                _unbound = true;
                _model.QueueUnbind(_queueName, _exchangeName, _routingKeyOrFilter, null);
            }
        }

        public void Dispose()
        {
            Unbind();
        }
    }
}