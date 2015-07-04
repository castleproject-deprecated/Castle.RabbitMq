namespace Castle.RabbitMq
{
	using System;

	public interface IRabbitQueueConsumer
	{
		Subscription RespondRaw(Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond, ConsumerOptions options);

		Subscription ConsumeRaw(Action<MessageEnvelope, IMessageAck> onReceived, ConsumerOptions options);

		///	<summary>
		///	Start listening for rpc request messages with the supplied func. 
		/// A <see cref="IRabbitQueue"/> should have at most one Respond and these are mutually exclusive 
		/// with the Consume. 
		///	</summary>
		///	<typeparam name="TRequest">Request type</typeparam>
		///	<typeparam name="TResponse">Reply type</typeparam>
		///	<param name="onRespond"></param>
		///	<param name="options"></param>
		[Obsolete]
		Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond,
												  ConsumerOptions options);

		///	<summary>
		///	Starts listening for messages to process with the supplied action. 
		/// A <see cref="IRabbitQueue"/> should have at most one Respond and these are mutually exclusive 
		/// with the Consume. 
		///	</summary>
		///	<param name="onReceived"></param>
		///	<param name="options"></param>
		[Obsolete]
		Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived,	
								ConsumerOptions	options);

		// MessageEnvelope<T> Receive<T>() where T : class;
		// MessageEnvelope<T> Peek<T>()	where T	: class;
	}

	public static class	RabbitQueueConsumerExtensions
	{
		// TODO: move this one to Extension	method
		// void	Consume<T>(Action<MessageEnvelope<T>> onMsgReceived, ConsumerOptions options) where	T :	class;

		public static Subscription Consume<T>(this IRabbitQueueConsumer	source,
											  Action<MessageEnvelope<T>, IMessageAck> onReceived)
		{
			return source.Consume<T>(onReceived, null);
		}

		public static Subscription Respond<TRequest, TResponse>(this IRabbitQueueConsumer source, 
			Func<MessageEnvelope<TRequest>,	IMessageAck, TResponse>	onRespond)
		{
			return source.Respond(onRespond, null);
		}
	}
}