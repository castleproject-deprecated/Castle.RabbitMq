namespace Castle.RabbitMq.WindsorIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Impl;
    using Messaging;


    public class MessageHandlerRegistry
    {
        private readonly IRabbitChannel _channel;
        private readonly IRabbitSerializer _serializer;
        private readonly ConcurrentDictionary<Type, Tuple<MessageHandlerInvoker, Func<IMessageHandler>>> _message2Invoker;
        private readonly MessageHandlerInvoker _defaultMsgInvoker;

        public MessageHandlerRegistry(IRabbitChannel channel, IRabbitSerializer serializer)
        {
            _channel = channel;
            _serializer = serializer;

            _message2Invoker = new ConcurrentDictionary<Type, Tuple<MessageHandlerInvoker, Func<IMessageHandler>>>();
            _defaultMsgInvoker = new DefaultMessageHandlerInvoker();
        }

        public void Add(Type messageType, Func<IMessageHandler> builder)
        {
            _message2Invoker[messageType] = Tuple.Create(_defaultMsgInvoker, builder);
        }

        public void Start()
        {
            // _channel.DeclareQueue("").Consume(OnReceived, new ConsumerOptions());
        }

        public void Stop()
        {
        }

        private void OnReceived(MessageEnvelope<byte[]> envelope, IMessageAck ack)
        {
            try
            {
                var typeName = envelope.Properties.Type;
                typeName.AssertNotNullOrEmpty("typename was expected to be added to message properties");

                // PERF: needs caching
                var msgType = Type.GetType(typeName, throwOnError: true);

                Tuple<MessageHandlerInvoker, Func<IMessageHandler>> tuple;
                if (!_message2Invoker.TryGetValue(msgType, out tuple))
                    throw new Exception("No IMessageHandler registered for message type " + msgType.FullName);

                MessageHandlerInvoker invoker = tuple.Item1;
                Func<IMessageHandler> builder = tuple.Item2;
                IMessageHandler handler = builder();

                var message = (IMessage) _serializer.Deserialize(envelope.Body, msgType);

                invoker.Invoke(msgType, message, handler);

                ack.Ack();
            }
            catch (Exception e)
            {
                ack.Reject(requeue: false);
            }
        }
    }

    public abstract class MessageHandlingStrategy
    {
        public abstract void Register(Type handlerImpl, Func<object> handler);
    }

    public class DefaultMessageHandlingStrategy : MessageHandlingStrategy
    {
        public override void Register(Type handlerContract, Func<object> handlerBuilder)
        {
            var messageType = handlerContract.GenericTypeArguments.First();
        }
    }
}