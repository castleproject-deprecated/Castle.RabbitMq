namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

    [System.Diagnostics.DebuggerDisplay("Queue '{Name}' {_queueOptions}", Name = "Queue")]
    public class RabbitQueue : IRabbitQueue
    {
        private readonly IModel _model;
        private readonly IRabbitExchange _exchange;
        private readonly IRabbitSerializer _defaultSerializer;
        private readonly QueueOptions _queueOptions;
        private readonly RpcHelper _rpcHelper;

        public RabbitQueue(IModel model, IRabbitExchange exchange, IRabbitSerializer serializer, 
                           QueueDeclareOk result, QueueOptions queueOptions)
        {
            _model = model;
            _exchange = exchange;
            _defaultSerializer = serializer ?? queueOptions.Serializer;
            _queueOptions = queueOptions;

            this.Name = result.QueueName;
            this.ConsumerCount = result.ConsumerCount;
            this.MessageCount = result.MessageCount;

            _rpcHelper = new RpcHelper(_model, _exchange.Name, _defaultSerializer);
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

        #region IRabbitSender

        public MessageInfo Send(byte[] body, string routingKey = "", 
                                MessageProperties properties = null, 
                                SendOptions options = null)
        {
            options = options ?? SendOptions.Default;

            var prop = properties ?? _model.CreateBasicProperties();
            if (options.Persist)
            {
                prop.DeliveryMode = 2; // persistent
            }

            lock (_model)
            {
                var id = _model.NextPublishSeqNo;
                _model.BasicPublish(_exchange.Name, routingKey,
                                    mandatory: options.Mandatory,
                                    immediate: options.Immediate, 
                                    basicProperties: properties, 
                                    body: body);
                return new MessageInfo() { Tag = id };
            }
        }

        public MessageInfo Send<T>(T message, string routingKey = "", 
                                   MessageProperties properties = null, 
                                   SendOptions options = null) 
            where T : class
        {
            options = options ?? SendOptions.Default;
            var serializer = options.Serializer ?? _defaultSerializer;
            var data = serializer.Serialize(message);

            return Send(data, routingKey, properties, options);
        }

        public MessageEnvelope SendRequest(byte[] data, string routingKey = "",
                                           MessageProperties properties = null,
                                           RpcSendOptions options = null)
        {

            return _rpcHelper.SendRequest(data, routingKey, properties, options);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request, 
                                                          string routingKey = "",
                                                          MessageProperties properties = null,
                                                          RpcSendOptions options = null) 
            where TRequest : class where TResponse : class
        {
            return _rpcHelper.SendRequest<TRequest, TResponse>(
                request, routingKey, properties, options);
        }

        #endregion

        #region IRabbitQueueConsumer

        public Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond,
                                                         ConsumerOptions options)
        {
            options = options ?? ConsumerOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            var consumer = new StreamerConsumer<TRequest>(_model, serializer);

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

            // TODO: perf test consumers
            var consumer = new StreamerConsumer<T>(_model, serializer);

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
    }
}