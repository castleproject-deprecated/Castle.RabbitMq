namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    class RpcHelper : IBasicConsumer
    {
        private readonly IModel _model;
        private readonly string _exchange;
        private readonly IRabbitSerializer _serializer;
        
        private Dictionary<string, string> _routing2RetQueue;
        private readonly ConcurrentDictionary<string, AutoResetEvent> _waits;
        private readonly ConcurrentDictionary<string, MessageEnvelope> _replyData;

        public RpcHelper(IModel model, string exchange, IRabbitSerializer serializer)
        {
            _model = model;
            _exchange = exchange;
            _serializer = serializer;

            _routing2RetQueue = new Dictionary<string, string>(StringComparer.Ordinal);
            _waits = new ConcurrentDictionary<string, AutoResetEvent>(StringComparer.Ordinal);
            _replyData = new ConcurrentDictionary<string, MessageEnvelope>(StringComparer.Ordinal);
        }

        public MessageEnvelope SendRequest(byte[] data, 
                                           string routingKey, 
                                           MessageProperties properties,
                                           RpcSendOptions options)
        {
            var prop = properties ?? _model.CreateBasicProperties();

            AutoResetEvent @event;

            lock (_model)
            {
                var returnQueue = GetOrCreateReturnQueue(routingKey);
                prop.ReplyTo = returnQueue;
                prop.CorrelationId = Guid.NewGuid().ToString();
                prop.Expiration = TimeSpan.FromSeconds(30).TotalMilliseconds.ToString(); // 30 seconds

                @event = new AutoResetEvent(false);
                _waits[prop.CorrelationId] = @event;

                _model.BasicPublish(_exchange, routingKey, false, false, properties, data);
            }

            using (@event)
            if (!@event.WaitOne(options.Timeout))
            {
                throw new Exception("timeout");
            }

            MessageEnvelope reply;
            _replyData.TryRemove(prop.CorrelationId, out reply);
            return reply;
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request,
                                                          string routingKey,
                                                          MessageProperties properties,
                                                          RpcSendOptions options)
        {
            var data = _serializer.Serialize(request);
            var reply = this.SendRequest(data, routingKey, properties, options);

            return _serializer.Deserialize<TResponse>(reply.Body);
        }

        private string GetOrCreateReturnQueue(string routingKey)
        {
            string queueName;
            if (_routing2RetQueue.TryGetValue(routingKey, out queueName)) return queueName;
            
            queueName = (string)_model.QueueDeclare();
            _routing2RetQueue[routingKey] = queueName;

            _model.BasicConsume(queueName, false, this);
            
            return queueName;
        }


        public void HandleBasicCancel(string consumerTag)
        {
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
        }

        public void HandleBasicDeliver(string consumerTag, 
                                       ulong deliveryTag, 
                                       bool redelivered, 
                                       string exchange, string routingKey,
                                       IBasicProperties properties, 
                                       byte[] body)
        {
            var correlationId = properties.CorrelationId;
            if (string.IsNullOrEmpty(correlationId))
            {
                throw new Exception("Invalid correlationId");
            }

            AutoResetEvent @event;
            if (!_waits.TryRemove(correlationId, out @event))
            {
                // wtf?
            }

            _replyData[correlationId] = new MessageEnvelope(properties, body)
            {
                ConsumerTag = consumerTag, 
                DeliveryTag = deliveryTag,
                ExchangeName = exchange, 
                IsRedelivery = redelivered, 
                RoutingKey = routingKey
            };

            try
            {
                @event.Set(); // may have been disposed

                lock (_model)
                    _model.BasicAck(deliveryTag, false);
            }
            catch (Exception)
            {
                lock (_model)
                    _model.BasicNack(deliveryTag, false, false);
            }
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
        }

        public IModel Model
        {
            get { return _model; }
        }

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;
    }
}