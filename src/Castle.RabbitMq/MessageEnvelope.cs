namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    public class MessageEnvelope
    {
        private readonly IBasicProperties _properties;
        private readonly byte[] _body;

        public MessageEnvelope(IBasicProperties properties, byte[] body)
        {
            _properties = properties;
            _body = body;
        }

        public string ConsumerTag { get; set; }
        public ulong DeliveryTag { get; set; }
        public bool IsRedelivery { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }

        public IBasicProperties Properties
        {
            get { return _properties; }
        }

        public byte[] Body
        {
            get { return _body; }
        }
    }

    public class MessageEnvelope<T> : MessageEnvelope where T : class
    {
        public MessageEnvelope(IBasicProperties properties, byte[] body) : base(properties, body)
        {
        }
    }

}
