namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

    interface IMessageProducer<T>
    {
        Action<MessageEnvelope<T>> Produced { get; }
    }


    class RabbitDefaultConsumer<T> : DefaultBasicConsumer
    {
        private readonly IModel _model;
        private readonly IRabbitSerializer _serializer;
        private readonly Action<MessageEnvelope<T>, MessageAck> _onReceived;

        public RabbitDefaultConsumer(IModel model, 
                                     IRabbitSerializer serializer,
                                     Action<MessageEnvelope<T>, IMessageAck> onReceived)
            : base(model)
        {
            _model = model;
            _serializer = serializer;
            _onReceived = onReceived;
        }

        public override void HandleBasicDeliver(string consumerTag, 
                                                ulong deliveryTag, bool redelivered, 
                                                string exchange, string routingKey,
                                                IBasicProperties properties, 
                                                byte[] body)
        {
            var ackFacade = new MessageAck(
                () =>
                {
                    lock(_model) _model.BasicAck(deliveryTag, false);
                }, 
                () =>
                {
                    lock (_model) _model.BasicNack(deliveryTag, false, true);
                });

            if (typeof(T) == typeof(byte[]))
            {
                var envelope = new MessageEnvelope<T>(properties, default(T), body)
                {
                    ConsumerTag = consumerTag, 
                    DeliveryTag = deliveryTag,
                    ExchangeName = exchange,
                    IsRedelivery = redelivered, 
                    RoutingKey = routingKey
                };
                
                _onReceived(envelope, ackFacade);
            }
            else
            {
                var msg = _serializer.Deserialize<T>(body);

                var envelope = new MessageEnvelope<T>(properties, msg, body)
                {
                    ConsumerTag = consumerTag,
                    DeliveryTag = deliveryTag,
                    ExchangeName = exchange,
                    IsRedelivery = redelivered,
                    RoutingKey = routingKey
                };

                _onReceived(envelope, ackFacade);
            }
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
}