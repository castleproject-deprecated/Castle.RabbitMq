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
        private readonly ConcurrentDictionary<string, Tuple<AutoResetEvent, byte[]>> _replies;

        public RpcHelper(IModel model, string exchange, IRabbitSerializer serializer)
        {
            _model = model;
            _exchange = exchange;
            _serializer = serializer;

            _routing2RetQueue = new Dictionary<string, string>(StringComparer.Ordinal);
            _replies = new ConcurrentDictionary<string, Tuple<AutoResetEvent, byte[]>>(StringComparer.Ordinal);
        }

        public MessageEnvelope SendRequest(byte[] data, 
                                           string routingKey, 
                                           MessageProperties properties,
                                           RpcSendOptions options)
        {
            var prop = properties ?? _model.CreateBasicProperties();

            lock (_model)
            {
                var returnQueue = GetOrCreateReturnQueue(routingKey);
                prop.ReplyTo = returnQueue;
                prop.CorrelationId = Guid.NewGuid().ToString();
                prop.Expiration = TimeSpan.FromSeconds(30).TotalMilliseconds.ToString(); // 30 seconds

                var tuple = Tuple.Create(new AutoResetEvent(false), new byte[0]);
                _replies[prop.CorrelationId] = tuple;

                _model.BasicPublish(_exchange, routingKey, false, false, properties, data);
        

            }

            return null;
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request,
                                                          string routingKey,
                                                          MessageProperties properties,
                                                          RpcSendOptions options)
        {
            return default(TResponse);
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