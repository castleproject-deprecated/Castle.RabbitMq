namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;

	public interface IRabbitSender // : IRabbitTypedSender, IRabbitRawSender
	{
		MessageInfo SendRaw(byte[] body, string routingKey = "",
			IBasicProperties properties = null,
			SendOptions options = null);

		MessageEnvelope CallRaw(byte[] data, string routingKey = "",
			IBasicProperties properties = null,
			RpcSendOptions options = null);

		MessageInfo Send<T>(T message, string routingKey = "",
			IBasicProperties properties = null,
			SendOptions options = null) where T : class;

		TResponse Call<TRequest, TResponse>(TRequest request,
			string routingKey = "",
			IBasicProperties properties = null,
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