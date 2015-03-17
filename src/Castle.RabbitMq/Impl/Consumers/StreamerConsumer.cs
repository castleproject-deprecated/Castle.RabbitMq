namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using RabbitMQ.Client;


    class StreamerConsumer<T> : DefaultBasicConsumer, IMessageProducer<T>
    {
        private readonly ConcurrentBag<IMessageConsumer<T>> _consumers;
        private readonly IRabbitSerializer _serializer;

        public StreamerConsumer(IModel model, IRabbitSerializer serializer) : base(model)
        {
            _consumers = new ConcurrentBag<IMessageConsumer<T>>();
            _serializer = serializer;
        }

        public void Subscribe(IMessageConsumer<T> consumer)
        {
            _consumers.Add(consumer);
        }

        public override void HandleBasicDeliver(string consumerTag,
                                                ulong deliveryTag, bool redelivered,
                                                string exchange, string routingKey,
                                                IBasicProperties properties,
                                                byte[] body)
        {
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

                InternalPublish(envelope);
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

                InternalPublish(envelope);
            }
        }

        private void InternalPublish(MessageEnvelope<T> envelope)
        {
            foreach (var consumer in _consumers)
            {
                consumer.OnNext(envelope);
            }
        }
    }
}