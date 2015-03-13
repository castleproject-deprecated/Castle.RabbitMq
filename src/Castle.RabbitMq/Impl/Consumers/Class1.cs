namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    class RabbitDefaultConsumer<T> : DefaultBasicConsumer
    {
        private readonly IRabbitSerializer _serializer;

        public RabbitDefaultConsumer(IModel model, IRabbitSerializer serializer)
            : base(model)
        {
            _serializer = serializer;
        }

        public override void HandleBasicDeliver(string consumerTag, 
                                                ulong deliveryTag, bool redelivered, 
                                                string exchange, string routingKey,
                                                IBasicProperties properties, 
                                                byte[] body)
        {
            if (typeof (T) == typeof (byte[]))
            {
                
            }

            if (_serializer != null)
            {
//                _serializer.Deserialize()
            }

            // base.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
        }

        public override void HandleBasicCancel(string consumerTag)
        {
            base.HandleBasicCancel(consumerTag);
        }

        public override void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            base.HandleModelShutdown(model, reason);
        }

        public override void OnCancel()
        {
            base.OnCancel();
        }
    }

    class RabbitSharedQueueConsumer : QueueingBasicConsumer
    {
        public RabbitSharedQueueConsumer(IModel model, IRabbitSerializer serializer)
            : base(model)
        {
        }
    }
}
