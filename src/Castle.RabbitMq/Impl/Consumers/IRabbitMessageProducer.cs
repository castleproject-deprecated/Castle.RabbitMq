namespace Castle.RabbitMq
{
	using RabbitMQ.Client;

	interface IRabbitMessageProducer : IBasicConsumer, IMessageProducer
	{
	}

	///	<summary>
	///	Union of two behaviors,	just to	make our life easier
	///	</summary>
//	interface IRabbitMessageProducer<T> : IRabbitMessageProducer // , IMessageProducer<T>
//	{
//	}
}