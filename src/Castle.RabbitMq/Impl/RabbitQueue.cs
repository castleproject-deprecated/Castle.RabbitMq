namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

    [System.Diagnostics.DebuggerDisplay("Queue '{Name}' {_queueOptions}", Name = "Queue")]
    public class RabbitQueue : IRabbitQueue
    {
        private readonly IModel _model;
        private readonly IRabbitSerializer _defaultSerializer;
        private readonly QueueOptions _queueOptions;

        public RabbitQueue(IModel model, IRabbitSerializer serializer, 
                           QueueDeclareOk result, QueueOptions queueOptions)
        {
            _model = model;
            _defaultSerializer = serializer ?? queueOptions.Serializer;
            _queueOptions = queueOptions;

            this.Name = result.QueueName;
            this.ConsumerCount = result.ConsumerCount;
            this.MessageCount = result.MessageCount;
        }

        #region IRabbitQueue

        public string Name { get; private set; }
        public uint ConsumerCount { get; private set; }
        public uint MessageCount { get; private set; }

        public void Purge()
        {
            lock (_model)
                _model.QueuePurge(this.Name);
        }

        public void Delete()
        {
            lock (_model)
                _model.QueueDelete(this.Name);
        }

        public void Delete(bool ifUnused, bool ifEmpty)
        {
            lock (_model)
                _model.QueueDelete(this.Name, ifUnused, ifEmpty);
        }

        #endregion

        #region IRabbitQueueConsumer

        public Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond,
                                                         ConsumerOptions options)
        {
            options = options ?? ConsumerOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            var consumer = CreateConsumer<TRequest>(serializer, options);

            consumer.Subscribe(new RpcResponder<TRequest, TResponse>(_model, serializer, onRespond));

            lock (_model)
            {
                var consumerTag = _model.BasicConsume(this.Name, options.NoAck, consumer);

                return new Subscription(_model, consumerTag);
            }
        }

        public Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived, 
                                       ConsumerOptions options) 
        {
            options = options ?? ConsumerOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            // PERF: perf test consumers
            var consumer = CreateConsumer<T>(serializer, options);

            consumer.Subscribe(new ActionAdapter<T>(env =>
            {
                var msgAcker = new MessageAck(() =>
                {
                    lock (_model) _model.BasicAck(env.DeliveryTag, false);
                }, (requeue) =>
                {
                    lock (_model) _model.BasicNack(env.DeliveryTag, false, requeue);
                });

                onReceived(env, msgAcker);
            }));

            lock (_model)
            {
                var consumerTag = _model.BasicConsume(this.Name, options.NoAck, consumer);

                return new Subscription(_model, consumerTag);
            }
        }

        #endregion

        private IRabbitMessageProducer<T> CreateConsumer<T>(IRabbitSerializer serializer, ConsumerOptions options)
        {
            if (options.ConsumerStrategy == ConsumerStrategy.Default)
            {
                return new StreamerConsumer<T>(_model, serializer);
            }
            // else if (options.ConsumerStrategy == ConsumerStrategy.Queue)
            {
                return new SharedQueueConsumer<T>(_model, serializer);
            }
        }
    }
}