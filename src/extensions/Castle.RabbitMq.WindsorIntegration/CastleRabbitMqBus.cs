namespace Castle.RabbitMq.WindsorIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Transactions;
    using Messaging;
    using Castle.RabbitMq;
    using Serializers;
    using Transactions;


    public class CastleRabbitMqBus : IBus, IDisposable
    {
        private readonly ConfigSettings _config;
        private readonly ConcurrentDictionary<string, IRabbitExchange> _declaredExchange;
        private readonly ConcurrentDictionary<string, IRabbitQueue> _declaredQueue;
        private readonly IRabbitSerializer _serializer;
        private readonly object _exchangeDeclareLock = new object();
        private readonly object _queueDeclareLock = new object();

        private RabbitConnection _connection;
        private IRabbitChannel _channel;
        private volatile bool _started;
        private volatile bool _disposed;
        private bool _ownChannel;


        public CastleRabbitMqBus(ConfigSettings config, IRabbitSerializer serializer)
        {
            _config = config;
            _serializer = serializer ?? new JsonSerializer();

            _declaredExchange = new ConcurrentDictionary<string, IRabbitExchange>(StringComparer.Ordinal);
            _declaredQueue = new ConcurrentDictionary<string, IRabbitQueue>(StringComparer.Ordinal);
        }

        public CastleRabbitMqBus(ConfigSettings config) : this(config, (IRabbitSerializer)null)
        {
        }

        public CastleRabbitMqBus(ConfigSettings config, IRabbitChannel channel)
            : this(config, (IRabbitSerializer)null)
        {
            _channel = channel;
        }

        public IRabbitSerializer Serializer
        {
            get { return _serializer; }
        }

        public event EventHandler Started;

        public void Publish(IMessage message)
        {
            Publish(new[] { message });
        }

        public void Publish(IEnumerable<IMessage> messages)
        {
            InternalPublish(messages, exact: false, createQueue: false);
        }

        public void Send(IMessage message)
        {
            Send(new[] { message });
        }

        public void Send(IEnumerable<IMessage> messages)
        {
            InternalPublish(messages, exact: true, createQueue: true);
        }

        public IDisposable Consume(string queueName, Action<MessageEnvelope<byte[]>, IMessageAck> onReceived)
        {
            EnsureStarted();

            if (!string.IsNullOrEmpty(_config.QueueNamePrefix) && !queueName.StartsWith(_config.QueueNamePrefix, StringComparison.Ordinal))
            {
                queueName = _config.QueueNamePrefix + queueName;
            }

            var queue = _channel.DeclareQueue(queueName, new QueueOptions() {Durable = true});
            return queue.Consume(onReceived);
        }

        public void Start()
        {
            if (_started) return;

            _started = true;
            Thread.MemoryBarrier();

            // channel was supplied on alternative constructor
            if (_channel != null) return;

            _connection =
                RabbitConnector.Connect(
                    _config.Host,
                    _config.Port,
                    _config.Username,
                    _config.Password,
                    _config.VHost);

            // Single consumer thread + single IO thread
            _channel = _connection.CreateChannel(new ChannelOptions());

            _ownChannel = true;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            Thread.MemoryBarrier();

            if (_ownChannel && _channel != null)
            {
                _channel.Dispose();
            }

            if (_ownChannel && _connection != null)
            {
                _connection.Dispose();
            }
        }

        private void InternalPublish(IEnumerable<IMessage> messages, bool exact, bool createQueue)
        {
            foreach (var message in messages)
            {
                InternalPublish(message, exact, createQueue);
            }
        }

        private void InternalPublish(IMessage message, bool exact, bool createQueue)
        {
            string routingKey;
            var exchange = EnsureDeclaredAndBound(message, exact, createQueue, out routingKey);

            InternalSend(exchange, message, routingKey, persist: true);
        }

        private void InternalSend(IRabbitExchange exchange, IMessage message, string routingKey, bool persist)
        {
            var props = new MessageProperties()
            {
                Type = message.GetType().ExtendedName()
            };

            Action sendAction = () => exchange.Send(message, routingKey, props, new SendOptions()
            {
                Persist = persist,
                Serializer = _serializer
            });

            var curTransaction = Transaction.Current;

            if (curTransaction != null)
            {
                curTransaction.EnlistVolatile(
                    new DispatcherEnlistment(sendAction), 
                    EnlistmentOptions.EnlistDuringPrepareRequired);
            }
            else
            {
                sendAction();
            }
        }

        private IRabbitExchange EnsureDeclaredAndBound(IMessage message, bool exact, bool createQueue, out string routingKey)
        {
            Argument.NotNull(message, "message");

            EnsureStarted();

            var exchangeName = ResolveExchangeName(message, _config, exact);
            exchangeName.AssertNotNullOrEmpty();

            IRabbitExchange exchange = GetOrDeclareExchange(exchangeName);

            routingKey = ResolveRoutingKey(message);

            if (routingKey == null) // message does not implement IRoutable
            {
                routingKey = message.GetType().FullName;
            }

            if (createQueue)
            {
                var queueName = GetQueueName(routingKey);
                IRabbitQueue queue;
                if (DeclareQueueIfNecessary(queueName, out queue))
                {
                    BindQueueToExchange(exchange, queue, routingKey);
                }
            }

            return exchange;
        }

        private IRabbitExchange GetOrDeclareExchange(string exchangeName)
        {
            IRabbitExchange exchange;
            if (_declaredExchange.TryGetValue(exchangeName, out exchange))
            {
                return exchange;
            }

            lock (_exchangeDeclareLock)
            {
                if (_declaredExchange.TryGetValue(exchangeName, out exchange))
                {
                    return exchange;
                }

                exchange = _channel.DeclareExchange(exchangeName, new ExchangeOptions()
                {
                    Durable = true,
                    ExchangeType = RabbitExchangeType.Direct
                });

                _declaredExchange[exchangeName] = exchange;
            }

            return exchange;
        }

        private bool DeclareQueueIfNecessary(string queueName, out IRabbitQueue queue)
        {
            if (_declaredQueue.TryGetValue(queueName, out queue))
            {
                return false; // false == was not created here, by us
            }

            lock (_queueDeclareLock)
            {
                if (_declaredQueue.TryGetValue(queueName, out queue))
                {
                    return false; // ditto
                }

                queue = _channel.DeclareQueue(queueName, new QueueOptions()
                {
                    Durable = true,
                    Exclusive = false,
                    AutoDelete = false
                });

                _declaredQueue[queueName] = queue;
            }

            return true;
        }

        private void BindQueueToExchange(IRabbitExchange exchange, IRabbitQueue queue, string routingKey)
        {
            _channel.Bind(exchange, queue, routingKey);
        }

        private string GetQueueName(string routingKey)
        {
            return (_config.QueueNamePrefix ?? "") + routingKey;
        }

        private string ResolveRoutingKey(IMessage message)
        {
            var routable = message as IRoutable;

            if (routable != null)
            {
                routable.RoutingKey.AssertNotNullOrEmpty();
                return routable.RoutingKey;
            }

            return null;
        }

        private string ResolveExchangeName(IMessage message, ConfigSettings config, bool exact)
        {
            // PERF: this all could be cached.
            var msgType = message.GetType();

            string endpoint;

            // PERF: Assembly.GetName is slow
            var asmName = msgType.Assembly.GetName().Name;
            if (config.NamespaceExchangeMapping.TryGetValue(asmName, out endpoint))
                return (_config.ExchangeNamePrefix ?? "") + endpoint;

            if (config.NamespaceExchangeMapping.TryGetValue(msgType.Name, out endpoint))
                return (_config.ExchangeNamePrefix ?? "") + endpoint;

            if (exact)
                throw new Exception("Messaging's Endpoint not found for " + msgType.FullName);

            return (_config.ExchangeNamePrefix ?? "") + config.Id;
        }

        private void EnsureStarted()
        {
            // TODO: Potential race
            if (!_started)
            {
                Start();
            }
        }
    }
}
