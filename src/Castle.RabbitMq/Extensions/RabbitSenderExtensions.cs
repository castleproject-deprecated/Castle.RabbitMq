namespace Castle.RabbitMq
{
	public static class RabbitSenderExtensions
	{
		public static MessageInfo SendRaw(this IRabbitSender source, byte[] body, string routingKey = "")
		{
			return source.SendRaw(body, routingKey, options: SendOptions.Default);
		}

		public static MessageInfo Send<T>(this IRabbitSender source, T message, string routingKey = "") where T : class
		{
			return source.Send<T>(message, routingKey, options: SendOptions.Default);
		}

		public static MessageInfo SendPersistentRaw(this IRabbitSender source, byte[] body, string routingKey = "")
		{
			return source.SendRaw(body, routingKey, options: SendOptions.Persistent);
		}

		public static MessageInfo SendPersistent<T>(this IRabbitSender source, T message, string routingKey = "")
			where T : class
		{
			return source.Send<T>(message, routingKey, options: SendOptions.Persistent);
		}
	}
}