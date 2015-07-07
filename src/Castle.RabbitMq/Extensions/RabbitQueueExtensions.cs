namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client.Framing;

	public static class RabbitQueueExtensions
	{
		public static Subscription Respond<TRequest, TResponse>(this IRabbitQueue source, 
			Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond,
			ConsumerOptions options = null)
		{
			Argument.NotNull(onRespond, "onRespond");

			options = options ?? ConsumerOptions.DefaultForRespond;
			options.ShouldSerializeExceptions = true;

			var serializer = options.Serializer ?? source.DefaultSerializer;


			var typedOnRespond = new Func<MessageEnvelope, IMessageAck, MessageEnvelope>((envelope, ack) =>
			{
				var typedMessage = serializer.TypedDeserialize<TRequest>(envelope.Body, envelope.Properties);

				var replyInstance = onRespond(new MessageEnvelope<TRequest>(envelope, typedMessage), ack);

				var replyProperties = new BasicProperties();
				var replyData = serializer.TypedSerialize(replyInstance, replyProperties);

				return new MessageEnvelope(replyProperties, replyData);
			});

			return source.RespondRaw(typedOnRespond, options);
		}

		public static Subscription Consume<T>(this IRabbitQueue source, 
			Action<MessageEnvelope<T>, IMessageAck> onReceived,
			ConsumerOptions options = null)
		{
			Argument.NotNull(onReceived, "onReceived");

			options = options ?? ConsumerOptions.Default;

			var serializer = options.Serializer ?? source.DefaultSerializer;

			var typedReceived = new Action<MessageEnvelope, IMessageAck>((envelope, ack) =>
			{
				var message = serializer.TypedDeserialize<T>(envelope.Body, envelope.Properties);

				onReceived(new MessageEnvelope<T>(envelope, message), ack);
			});

			return source.ConsumeRaw(typedReceived, options);
		}
	}
}