namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;


	internal class SharedQueueConsumer : QueueingBasicConsumer, IRabbitMessageProducer
	{
		private readonly ConcurrentBag<IMessageConsumer> _consumers = new ConcurrentBag<IMessageConsumer>();
//		private readonly IRabbitSerializer _serializer;
		private readonly Thread _thread;
		private volatile bool _closed;

		public SharedQueueConsumer(IModel model) : base(model)
		{
//			_serializer = serializer;
			_thread = new Thread(OnProc)
			{
				IsBackground = true
			};
			_thread.Start();
		}

		public void Subscribe(IMessageConsumer consumer)
		{
			_consumers.Add(consumer);
		}

		private void OnProc()
		{
			try
			{
				while (!_closed)
				{
					var args = base.Queue.Dequeue();
					if (args == null) continue;

					if (_consumers.Count == 0)
					{
						// throwing out messages due to lack of consumers

						if (LogAdapter.LogEnabled)
							LogAdapter.LogDebug(this.GetType().FullName,
								"SharedQueueConsumer dropping message due to lack of consumers subscribed");

						continue;
					}

					PublishToConsumers(args);
				}
			}
			catch(Exception e)
			{
				if (LogAdapter.LogEnabled)
					LogAdapter.LogError(this.GetType().FullName, "SharedQueueConsumer error ", e);
			}
		}

		public override void OnCancel()
		{
			_closed = true;
			Thread.MemoryBarrier();

			Thread.Sleep(0);
			base.OnCancel();
		}

		private void PublishToConsumers(BasicDeliverEventArgs args)
		{
			var envelope = new MessageEnvelope(args.BasicProperties, args.Body)
			{
				ConsumerTag = args.ConsumerTag,
				DeliveryTag = args.DeliveryTag,
				ExchangeName = args.Exchange,
				IsRedelivery = args.Redelivered,
				RoutingKey = args.RoutingKey
			};

			foreach(var consumer in _consumers)
			{
				consumer.OnNext(envelope);
			}
		}
	}
}