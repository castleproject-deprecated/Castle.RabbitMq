namespace Castle.RabbitMq.Stubs
{
	using System;


	public class StubRabbitQueue : IRabbitQueue
	{
		private bool _consumedCalled;
		private bool _respondCalled;
		private StubModel _model;

		public StubRabbitQueue(string name, QueueOptions options)
		{
			this.Name = name;
			this.Options = options;

			_model = new StubModel();
		}

		// Stub helpers

		public QueueOptions Options { get; private set; }
		public bool Deleted { get; set; }
		public bool Purged { get; set; }

		public void Publish()
		{
		}

		// End Stub helpers

		public Subscription RespondRaw(Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond, ConsumerOptions options)
		{
			if (_consumedCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			_respondCalled = true;

			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public Subscription ConsumeRaw(Action<MessageEnvelope, IMessageAck> onReceived, ConsumerOptions options)
		{
			if (_respondCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			_consumedCalled = true;


			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond, 
														 ConsumerOptions options)
		{
			if (_consumedCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			_respondCalled = true;

			// TODO: Respond

			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived, 
									   ConsumerOptions options)
		{
			if (_respondCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			_consumedCalled = true;

			// TODO: Consume

			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public string Name { get; private set; }

		public uint ConsumerCount { get; set; }

		public uint MessageCount { get; set; }

		public void Purge()
		{
			this.Purged = true;
		}

		public void Delete()
		{
			this.Deleted = true;
		}

		public void Delete(bool ifUnused, bool ifEmpty)
		{
			this.Deleted = true;
		}
	}
}