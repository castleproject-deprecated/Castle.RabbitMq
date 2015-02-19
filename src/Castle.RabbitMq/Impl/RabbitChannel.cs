namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitChannel : IRabbitChannel
    {
        private readonly IModel _model;

        private RabbitExchange _defaultExchange;
        private volatile bool _disposed;

        public RabbitChannel(IModel model)
        {
            _model = model;

            _defaultExchange = new RabbitExchange(string.Empty, new ExchangeOptions()
            {
                
            }, this._model);
        }

        // _model.ExchangeDeclare(name, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            EnsureNotDisposed();

            _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments)
        }

        public IRabbitQueue DeclareEphemeralQueue(QueueOptions options)
        {
            EnsureNotDisposed();

            // _model.ExchangeDeclare(string.Empty, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);
        }

        public IRabbitExchange DeclareExchange(string name, ExchangeOptions options)
        {
            EnsureNotDisposed();

            var exchange = new RabbitExchange(name, options, this._model);

            _model.ExchangeDeclare(name, ToExchangeType(options.ExchangeType), options.Durable, options.AutoDelete, options.Arguments);

            return exchange;
        }

        public IExchangeQueueBinding Bind(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter = null)
        {
            EnsureNotDisposed();

            throw new NotImplementedException();
        }


        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            _model.Close();
        }

        private static string ToExchangeType(RabbitExchangeType exchangeType)
        {
            return exchangeType.ToString().ToLowerInvariant();
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("RabbitConnection");
        }
    }

    public class RabbitExchange : IRabbitExchange
    {
        private readonly string _name;
        private readonly ExchangeOptions _options;
        private readonly IModel _channel;

        public RabbitExchange(string name, ExchangeOptions options, IModel channel)
        {
            _name = name;
            _options = options;
            _channel = channel;
        }

        public MessageInfo Send<T>(T message, string routing = "", bool persist = false, bool mandatory = false,
            MessageProperties properties = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            var result = _channel.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);

            return new RabbitQueue(result.QueueName, result.ConsumerCount, result.MessageCount, options);
        }

        public IRabbitQueue DeclareEphemeralQueue(QueueOptions options)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class RabbitQueue : IRabbitQueue
    {
        private readonly QueueOptions _options;
        private readonly IModel _model;
        private Action<int> _confirmationCallback;

        public RabbitQueue(string name, uint consumerCount, uint messageCount, QueueOptions options, IModel model)
        {
            _options = options;
            _model = model;
            Name = name;
            ConsumerCount = consumerCount;
            MessageCount = messageCount;
        }

        public string Name { get; set; }
        public uint ConsumerCount { get; set; }
        public uint MessageCount { get; set; }

        public Action<int> ConfirmationCallback
        {
            get { return _confirmationCallback; }
            set { _confirmationCallback = value; }
        }

        public MessageInfo Send<T>(T message, MessageProperties properties = null, SendOptions options = null) where T : class
        {
            throw new NotImplementedException();
        }

        public MessageEnvelope<T> Receive<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public MessageEnvelope<T> Peek<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Consume<T>(Action<MessageEnvelope<T>> onMsgReceived, ConsumeOptions options) where T : class
        {
            throw new NotImplementedException();
        }

        public void Consume<T>(Action<MessageEnvelope<T>, MessageAck> onMsgReceived, ConsumeOptions options) where T : class
        {
            throw new NotImplementedException();
        }

        public void ConsumeRaw(Action<MessageEnvelope, MessageAck> onMsgReceived, ConsumeOptions options)
        {
            _model.BasicConsume(this.Name, noAck: true, consumer: new ManualAckBasicConsumer());

        }

        public void Delete()
        {
            _model.QueueDelete(this.Name);
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }

    class ManualAckBasicConsumer : IBasicConsumer
    {
        private readonly IModel _model;

        public ManualAckBasicConsumer(IModel model)
        {
            this._model = model;
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
                                       ulong deliveryTag, bool redelivered, string exchange, string routingKey,
                                       IBasicProperties properties, byte[] body)
        {
            var envelope = new MessageEnvelope(properties, body)
            {
                ConsumerTag = consumerTag,
                DeliveryTag = deliveryTag,
                IsRedelivery = redelivered, 
                ExchangeName = exchange, 
                RoutingKey = routingKey,
            };
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