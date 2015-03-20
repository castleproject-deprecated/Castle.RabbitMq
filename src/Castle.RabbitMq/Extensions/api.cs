﻿namespace Castle.RabbitMq.Messaging
{
    using System;
    using System.Collections.Generic;

    public interface IBus
    {
        event EventHandler Started;

        IRabbitSerializer Serializer { get; }

        void Publish(IMessage message);
        void Publish(IEnumerable<IMessage> messages);

        void Send(IMessage message);
        void Send(IEnumerable<IMessage> messages);

        IDisposable Consume(string queueName, Action<MessageEnvelope<byte[]>, IMessageAck> onReceived);

        void Start();
    }

    /// <summary>
    /// Represents a message usable from a <see cref="IMessageHandler{TMessage}"/>
    /// </summary>
    public interface IMessage
    {
    }

    public interface IMessageHandler
    {
    }

    /// <summary>
    /// Implemented by a handler/processor of the specified message type
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageHandler<in TMessage> : IMessageHandler 
        where TMessage : IMessage
    {
        void Handle(TMessage message);
    }

    /// <summary>
    /// Declares that the message will define it's own routing key
    /// </summary>
    public interface IRoutable : IMessage
    {
        string RoutingKey { get; }
    }

    [Serializable]
    public class MessagingScopeAttribute : Attribute
    {
        public MessagingScopeAttribute(string scope)
        {
            this.Scope = scope;
        }

        public string Scope { get; private set; }

        public string[] GetScopes()
        {
            return this.Scope.Split(',');
        }
    }

    public class ConfigSettings
    {
        private IDictionary<string, string> _namespace2Exchange;

        public ConfigSettings()
        {
            this.Host = "localhost";
            this.VHost = "/";
            this.Username = this.Password = "guest";
            this.Port = 5672;
        }

        /// <summary>
        /// Used to scope the messages, 
        /// and as basis to create queues/exchanges names
        /// </summary>
        public string Id { get; set; }

        public string ExchangeNamePrefix { get; set; }
        public string QueueNamePrefix { get; set; }

        public string Host { get; set; }
        public string VHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public IDictionary<string, string> NamespaceExchangeMapping
        {
            get { return _namespace2Exchange ?? (_namespace2Exchange = new Dictionary<string, string>()); }
            set { _namespace2Exchange = value; }
        }
    }
}