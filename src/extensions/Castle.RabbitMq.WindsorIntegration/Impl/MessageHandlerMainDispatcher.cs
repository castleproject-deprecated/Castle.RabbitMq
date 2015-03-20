namespace Castle.RabbitMq.WindsorIntegration
{
    using System;
    using System.Collections.Concurrent;
    using Core.Logging;
    using Impl;
    using Messaging;

    /// <summary>
    /// Represents the role between the consumption of 
    /// queue messages and the activation/processing by <see cref="IMessageHandler{TMessage}"/>. 
    /// Note the <see cref="IMessageHandler{TMessage}.Handle"/> invocation is done through
    /// a <see cref="MessageHandlerInvoker"/>.
    /// </summary>
    public class MessageHandlerMainDispatcher : IDisposable
    {
        private readonly IBus _bus;
        private readonly MessageHandlerInvoker _invoker;
        private readonly ConfigSettings _config;
        private readonly ConcurrentDictionary<Type, Func<IMessageHandler>> _message2HandlerBuilder;
        private IDisposable _subscription;

        public MessageHandlerMainDispatcher(ConfigSettings config, 
                                            IBus bus, 
                                            MessageHandlerInvoker invoker)
        {
            this.Logger = NullLogger.Instance;

            _bus = bus;
            _invoker = invoker;
            _config = config;

            _message2HandlerBuilder = new ConcurrentDictionary<Type, Func<IMessageHandler>>();
        }

        public ILogger Logger { get; set; }

        public void Add(Type messageType, Func<IMessageHandler> builder)
        {
            Argument.NotNull(messageType, "messageType");
            Argument.NotNull(builder, "builder");

            _message2HandlerBuilder[messageType] = builder;
        }

        /// <summary>
        /// Should only be called once we know 
        /// all <see cref="IMessageHandler{TMessage}"/> were 
        /// registered
        /// </summary>
        public void Start()
        {
            _subscription = _bus.Consume(_config.Id, OnReceived);
        }

        /// <summary>
        /// Ends message consumption
        /// </summary>
        public void Stop()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void OnReceived(MessageEnvelope<byte[]> envelope, IMessageAck ack)
        {
            try
            {
                var typeName = envelope.Properties.Type;
                typeName.AssertNotNullOrEmpty("typename was expected to be added to message properties");

                // PERF: needs caching
                var msgType = Type.GetType(typeName, throwOnError: true);

                Func<IMessageHandler> builder;
                if (!_message2HandlerBuilder.TryGetValue(msgType, out builder))
                {
                    var msg = "No IMessageHandler registered for message type " + msgType.FullName;
                    this.Logger.Error(msg);
                    throw new Exception(msg);
                }

                IMessageHandler handler = builder();

                var message = (IMessage) _bus.Serializer.Deserialize(envelope.Body, msgType);

                _invoker.Invoke(msgType, message, handler);

                ack.Ack();
            }
            catch (Exception e)
            {
                this.Logger.Error("Error processing message", e);

                ack.Reject(requeue: false);
            }
        }
    }
}