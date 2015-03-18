namespace Castle.RabbitMq.Extensions.MessageHandler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Transactions;
    using Messaging;


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
            EnsureDeclaredAndBound(message, exact, createQueue, out queue, out routingKey);

            InternalSend(queue, message, routingKey, persist: true);
        }

        private void InternalSend(IRabbitQueue queue, IMessage message, string routingKey, bool persist)
        {
            Action sendAction = () => queue.Send(message, routingKey, options: new SendOptions()
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

        private void EnsureDeclaredAndBound(IMessage message, bool exact, bool createQueue, out IRabbitQueue queue, out string routingKey)
        {
            EnsureStarted();

            var exchangeName = ResolveExchangeName(message, _config, exact);

            IRabbitExchange exchange = GetOrDeclareExchange(exchangeName);

            var queueName = ResolveQueueNameAndRoutingKey(message, out routingKey);

            queue = null;
            if (createQueue)
            {
                if (DeclareQueueIfNecessary(queueName, out queue))
                {
                    BindQueueToExchange(exchange, queue, routingKey);
                }
            }

            if (queue == null)
            {
                queue = GetQueue(queueName);
            }
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

        private IRabbitQueue GetQueue(string queueName)
        {
            IRabbitQueue queue;
            if (_declaredQueue.TryGetValue(queueName, out queue))
            {
                return queue;
            }

            throw new Exception("This just cannot happen");
        }

        private void BindQueueToExchange(IRabbitExchange exchange, IRabbitQueue queue, string routingKey)
        {
            _channel.Bind(exchange, queue, routingKey);
        }

        private string ResolveQueueNameAndRoutingKey(IMessage message, out string routingKey)
        {
            var queueName = "";
            var routable = message as IRoutable;

            if (routable != null)
            {
                routingKey = routable.RoutingKey;
                queueName = routingKey;
            }
            else
            {
                routingKey = message.GetType().FullName;
                queueName = routingKey;
            }

            return queueName;
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



    internal class DispatcherEnlistment : IEnlistmentNotification
    {
//        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DispatcherEnlistment));

        private readonly Action dispatcher;

        public DispatcherEnlistment(Action dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            try
            {
                dispatcher();
            }
            catch (Exception e)
            {
//                logger.Error("Error dispatcher message", e);

                throw;
            }
            finally
            {
                enlistment.Done();
            }
        }

        public void Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }
}
