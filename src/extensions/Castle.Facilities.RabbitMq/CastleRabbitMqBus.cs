namespace Castle.RabbitMq.Extensions.MessageHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Transactions;
    using Messaging;
    using Transactions;
    using Castle.RabbitMq;


    public class CastleRabbitMqBus : IBus, IDisposable
    {
        private readonly ConfigSettings _config;
        private readonly ConcurrentDictionary<string, IRabbitExchange> _declaredExchange;
        private readonly ConcurrentDictionary<string, IRabbitQueue> _declaredQueue;
        private readonly object _exchangeDeclareLock = new object();
        private readonly object _queueDeclareLock = new object();

        private RabbitConnection _connection;
        private IRabbitChannel _channel;
        private volatile bool _started;
        private volatile bool _disposed;

        public CastleRabbitMqBus(ConfigSettings config)
        {
            _config = config;

            _declaredExchange = new ConcurrentDictionary<string, IRabbitExchange>(StringComparer.Ordinal);
            _declaredQueue = new ConcurrentDictionary<string, IRabbitQueue>(StringComparer.Ordinal);
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

        public void Start()
        {
            if (_started) return;

            _started = true;
            Thread.MemoryBarrier();

            _connection = 
                RabbitConnector.Connect(
                    _config.TargetHost,
                    _config.Port, 
                    _config.Username, 
                    _config.Password, 
                    _config.TargetVHost);

            // Single consumer thread + single IO thread
            _channel = _connection.CreateChannel(new ChannelOptions());
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            Thread.MemoryBarrier();

            if (_channel != null)
            {
                _channel.Dispose();
            }

            if (_connection != null)
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
            IRabbitQueue queue;
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
                Persist = persist
            });

            var curTransaction = Transaction.Current;

            if (curTransaction != null)
            {
                curTransaction.EnlistVolatile(
                    new DispatcherEnlistment(sendAction), EnlistmentOptions.EnlistDuringPrepareRequired);
            }
            else
            {
                sendAction();
            }
        }

        private IRabbitExchange EnsureDeclaredAndBound(IMessage message, bool exact, bool createQueue, out string routingKey)
        {
            EnsureStarted();

            var exchangeName = ResolveExchangeName(message, _config, exact);

            IRabbitExchange exchange = GetOrDeclareExchange(exchangeName);

            routingKey = ResolveRoutingKey(message);

            if (routingKey == null) // message does not implement IRoutable
            {
                routingKey = message.GetType().FullName;
            }

            if (createQueue)
            {
                IRabbitQueue queue;
                if (DeclareQueueIfNecessary(routingKey, out queue))
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
            if (config.Endpoints.TryGetValue(asmName, out endpoint))
                return endpoint;

            if (config.Endpoints.TryGetValue(msgType.Name, out endpoint))
                return endpoint;

            if (exact)
                throw new Exception("Messaging's Endpoint not found for " + msgType.FullName);

            return config.Id;
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
