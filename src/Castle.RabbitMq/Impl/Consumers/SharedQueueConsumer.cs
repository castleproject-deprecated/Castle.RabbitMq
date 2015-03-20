namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    internal class SharedQueueConsumer<T> : QueueingBasicConsumer, IRabbitMessageProducer<T>
    {
        private readonly ConcurrentBag<IMessageConsumer<T>> _consumers = new ConcurrentBag<IMessageConsumer<T>>();
        private readonly IRabbitSerializer _serializer;
        private readonly Thread _thread;
        private volatile bool _closed;

        public SharedQueueConsumer(IModel model, IRabbitSerializer serializer) : base(model)
        {
            _serializer = serializer;
            _thread = new Thread(OnProc)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        public void Subscribe(IMessageConsumer<T> consumer)
        {
            _consumers.Add(consumer);
        }

        private void OnProc()
        {
            try
            {
                while (!_closed)
                {
                    var args = base.Queue.Dequeue();
                    if (args == null) continue;

                    if (_consumers.Count == 0)
                    {
                        // throwing out messages due to lack of consumers

                        if (LogAdapter.LogEnabled)
                            LogAdapter.LogDebug(this.GetType().FullName, "SharedQueueConsumer dropping message due to lack of consumers subscribed");

                        continue;
                    }

                    PublishToConsumers(args);
                }
            }
            catch (Exception e)
            {
                if (LogAdapter.LogEnabled)
                    LogAdapter.LogError(this.GetType().FullName, "SharedQueueConsumer error ", e);
            }
        }

        public override void OnCancel()
        {
            _closed = true;
            Thread.MemoryBarrier();

            Thread.Sleep(0);
            base.OnCancel();
        }

        private void PublishToConsumers(BasicDeliverEventArgs args)
        {
            if (typeof(T) == typeof(byte[]))
            {
                var envelope = new MessageEnvelope<byte[]>(args.BasicProperties, args.Body, args.Body)
                {
                    ConsumerTag = args.ConsumerTag,
                    DeliveryTag = args.DeliveryTag,
                    ExchangeName = args.Exchange,
                    IsRedelivery = args.Redelivered,
                    RoutingKey = args.RoutingKey
                };

                foreach (var consumer in _consumers)
                {
                    consumer.OnNext(envelope as MessageEnvelope<T>);
                }
            }
            else
            {
                var deserialized = _serializer.Deserialize<T>(args.Body);

                var envelope = new MessageEnvelope<T>(args.BasicProperties, deserialized, args.Body)
                {
                    ConsumerTag = args.ConsumerTag,
                    DeliveryTag = args.DeliveryTag,
                    ExchangeName = args.Exchange,
                    IsRedelivery = args.Redelivered,
                    RoutingKey = args.RoutingKey
                };

                foreach (var consumer in _consumers)
                {
                    consumer.OnNext(envelope);
                }
            }
        }
    }
}