namespace Castle.RabbitMq.Extensions
{
    using System;

    public interface IMessage
    {
    }

    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        void Handle(TMessage message);
    }

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
}
