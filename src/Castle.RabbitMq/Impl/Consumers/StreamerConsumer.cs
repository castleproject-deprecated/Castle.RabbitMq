namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Concurrent;
	using RabbitMQ.Client;

	internal class StreamerConsumer : DefaultBasicConsumer, IRabbitMessageProducer
	{
		private readonly ConcurrentBag<IMessageConsumer> _consumers;

		public StreamerConsumer(IModel model) : base(model)
		{
			_consumers = new ConcurrentBag<IMessageConsumer>();
		}

		public void Subscribe(IMessageConsumer consumer)
		{
			_consumers.Add(consumer);
		}

		public override void HandleBasicDeliver(string consumerTag,
			ulong deliveryTag, bool redelivered,
			string exchange, string routingKey,
			IBasicProperties properties,
			byte[] body)
		{
			var envelope = new MessageEnvelope(properties, body)
			{
				ConsumerTag = consumerTag,
				DeliveryTag = deliveryTag,
				ExchangeName = exchange,
				IsRedelivery = redelivered,
				RoutingKey = routingKey
			};

			InternalPublish(envelope);
		}

		private void InternalPublish(MessageEnvelope envelope)
		{
			foreach(var consumer in _consumers)
			{
				consumer.OnNext(envelope);
			}
		}
	}
}