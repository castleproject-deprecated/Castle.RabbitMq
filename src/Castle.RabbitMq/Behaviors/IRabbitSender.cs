namespace Castle.RabbitMq
{
	using System;

	public interface IRabbitSender : IRabbitTypedSender, IRabbitRawSender
	{
	}

	public interface IRabbitRawSender
	{
		MessageInfo SendRaw(byte[] body, string routingKey = "",
			MessageProperties properties = null,
			SendOptions options = null);

		MessageEnvelope SendRequestRaw(byte[] data, string routingKey = "",
			MessageProperties properties = null,
			RpcSendOptions options = null);

		// Action<int> ConfirmationCallback { get; set; }
	}

	public interface IRabbitTypedSender
	{
		MessageInfo Send<T>(T message, string routingKey = "",
			MessageProperties properties = null,
			SendOptions options = null) where T : class;

		TResponse SendRequest<TRequest, TResponse>(TRequest request,
			string routingKey = "",
			MessageProperties properties = null,
			RpcSendOptions options = null)
			where TRequest : class
			where TResponse : class;

//        Action<int> ConfirmationCallback { get; set; }
	}

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