namespace Castle.RabbitMq
{
	using RabbitMQ.Client;

	public class MessageEnvelope<T>	: MessageEnvelope
	{
		public MessageEnvelope(MessageEnvelope copy, T message) : base(copy)
		{
			this.Message = message;
		}

		public MessageEnvelope(IBasicProperties	properties,	T message, byte[] body)	
			: base(properties, body)
		{
			this.Message = message;
		}

		public T Message { get;	private	set; }
	}
}