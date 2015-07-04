namespace Castle.RabbitMq
{
	using System;

	interface IMessageProducer
	{
		void Subscribe(IMessageConsumer consumer);
	}

//	interface IMessageProducer<T>
//	{
//		void Subscribe(IMessageConsumer<T> consumer);
//	}
}