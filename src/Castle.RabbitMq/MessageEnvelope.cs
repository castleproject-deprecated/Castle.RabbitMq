namespace Castle.RabbitMq
{
	using ProtoBuf;
	using RabbitMQ.Client;

	public class MessageEnvelope
	{
		public MessageEnvelope(IBasicProperties	properties,	byte[] body)
		{
			this.Properties	= properties;
			this.Body =	body;
		}

		public string ConsumerTag {	get; set; }
		public string ExchangeName { get; set; }
		public string RoutingKey { get;	set; }
		public ulong DeliveryTag { get;	set; }
		public bool	IsRedelivery { get;	set; }

		public IBasicProperties	Properties { get; private set; }
		public byte[] Body { get; private set; }

		public override	string ToString()
		{
			return string.Format("RoutingKey: {0} DeliveryTag: {1} IsRedelivery: {2} ConsumerTag: {3} Exchange:	{4}",
				RoutingKey,	DeliveryTag, IsRedelivery, ConsumerTag,	ExchangeName);
		}
	}

	public class MessageEnvelope<T>	: MessageEnvelope
	{
		public MessageEnvelope(IBasicProperties	properties,	T message, byte[] body)	
			: base(properties, body)
		{
			this.Message = message;
		}

		public T Message { get;	private	set; }
	}
}
