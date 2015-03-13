namespace Castle.RabbitMq
{
    using System;
    using RabbitMQ.Client;

    [System.Diagnostics.DebuggerDisplay("Queue {Name} Options {_queueOptions}", Name = "Queue")]
    public class RabbitQueue : IRabbitQueue
    {
        private readonly IModel _model;
        private readonly IRabbitExchange _exchange;
        private readonly QueueOptions _queueOptions;

        public RabbitQueue(IModel model, IRabbitExchange exchange, QueueDeclareOk result, QueueOptions queueOptions)
        {
            _model = model;
            _exchange = exchange;
            _queueOptions = queueOptions;

            this.Name = result.QueueName;
            this.ConsumerCount = result.ConsumerCount;
            this.MessageCount = result.MessageCount;
        }

        #region IRabbitQueue

        public string Name { get; private set; }
        public uint ConsumerCount { get; private set; }
        public uint MessageCount { get; private set; }

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
            var data = new byte[0];

            return Send(data, routingKey, properties, options);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request, 
                                                          string routingKey = "",
                                                          MessageProperties properties = null, 
                                                          SendOptions options = null) 
            where TRequest : class where TResponse : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRabbitQueueConsumer

        public QueueSubscription Respond<TRequest, TResponse>(string routingKey,
                                                              Func<MessageEnvelope<TRequest>, MessageAck, TResponse> onRespond,
                                                              ConsumerOptions options)
            where TRequest : class 
            where TResponse : class
        {
            
        }

        public QueueSubscription Consume<T>(string routingKey, 
                                            Action<MessageEnvelope<T>, MessageAck> onReceived, 
                                            ConsumerOptions options) 
            where T : class
        {
            options = options ?? ConsumerOptions.Default;
            lock (_model)
            {
                var serializer = options.Serializer ?? _queueOptions.Serializer;

                // TODO: perf test consumers
                // var consumer = new RabbitSharedQueueConsumer(_model, serializer);
                var consumer = new RabbitDefaultConsumer<T>(_model, serializer, onReceived, routingKey);
                var consumerTag = _model.BasicConsume(this.Name, options.NoAck, consumer);
                return new QueueSubscription(_model, consumerTag);
            }
        }

        #endregion

        #region IDestroyable

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}