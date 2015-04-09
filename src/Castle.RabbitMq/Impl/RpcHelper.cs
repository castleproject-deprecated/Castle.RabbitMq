namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    class RpcHelper : IBasicConsumer
    {
        private readonly IModel _model;
        private readonly IRabbitSerializer _serializer;
        private readonly string _exchange;
        
        private readonly Dictionary<string, string> _routing2RetQueue;
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
            // CreateBasicProperties doesnt need the lock
            var prop = _model.CreateBasicProperties();
            if (properties != null)
            {
                properties.CopyTo(prop);
            }

            var @event = new AutoResetEvent(false);

            try
            {
                lock (_model)
                {
                    var returnQueue = GetOrCreateReturnQueue(routingKey);
                    prop.ReplyTo = returnQueue;
                    prop.CorrelationId = Guid.NewGuid().ToString();
                    prop.Expiration = options.Timeout.TotalMilliseconds.ToString();

                    _waits[prop.CorrelationId] = @event;

                    _model.BasicPublish(_exchange, routingKey, prop, data);
                }

                if (!@event.WaitOne(options.Timeout))
                {
					MessageEnvelope val;
					_replyData.TryRemove(prop.CorrelationId, out val);

                    throw new Exception("timeout");
                }

                MessageEnvelope reply;
                _replyData.TryRemove(prop.CorrelationId, out reply);
                return reply;
            }
            finally
            {
                @event.Dispose();
            }
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request,
                                                          string routingKey,
                                                          MessageProperties properties,
                                                          RpcSendOptions options)
        {
            options = options ?? RpcSendOptions.Default;

			var data = _serializer.Serialize(request, properties);
            var reply = this.SendRequest(data, routingKey, properties, options);

			return _serializer.Deserialize<TResponse>(reply.Body, reply.Properties);
        }

        private string GetOrCreateReturnQueue(string routingKey)
        {
            string queueName;
            if (_routing2RetQueue.TryGetValue(routingKey, out queueName)) return queueName;
            
            queueName = _model.QueueDeclare();
            _routing2RetQueue[routingKey] = queueName;

            // starts a bare metal consumer with no acks
            _model.BasicConsume(queueName, true, this);
            
            return queueName;
        }

        //
        // IBasicConsumer implementation
        //

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
                throw new Exception("Invalid correlationId: got a null or empty one");
            }

            AutoResetEvent @event;
            if (!_waits.TryRemove(correlationId, out @event))
            {
                // timeout'd - no need to move further
                return;
            }

            // hold reply
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
            }
            catch (Exception)
            {
	            // potential object disposed

	            MessageEnvelope val;
	            _replyData.TryRemove(correlationId, out val);
            }
        }

        public IModel Model
        {
            get { return _model; }
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
        }

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        public void HandleBasicCancel(string consumerTag)
        {
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
        }
    }
}