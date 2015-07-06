namespace Castle.RabbitMq
{
	using System;

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