namespace Castle.RabbitMq.Stubs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using RabbitMQ.Client.Framing;


	public class StubMessageAcker : IMessageAck
	{
		public bool Acked { get; private set; }
		public bool Rejected { get; private set; }
		public bool Requeued { get; private set; }

		public void Ack()
		{
			Acked = true;
		}

		public void Reject(bool requeue)
		{
			Rejected = true;
			Requeued = true;
		}
	}


	[DebuggerDisplay("StubRabbitQueue {Name}")]
	public class StubRabbitQueue : IRabbitQueue
	{
		private bool _consumedCalled;
		private bool _respondCalled;
		private StubModel _model;

		private Action<MessageEnvelope, IMessageAck> _consumer;
		private Func<MessageEnvelope, IMessageAck, MessageEnvelope> _responder;

		public StubRabbitQueue(string name, QueueOptions options)
		{
			this.Name = name;
			this.Options = options;

			_model = new StubModel();

			this.DefaultSerializer = new StubRabbitSerializer();
		}

		// Stub helpers

		public QueueOptions Options { get; private set; }
		public bool Deleted { get; set; }
		public bool Purged { get; set; }

		public void StubPublish(MessageEnvelope message, IMessageAck acker = null)
		{
			if (!_consumedCalled) throw new Exception("Not consumer set up");
			if (acker == null) acker = new StubMessageAcker();

			_consumer(message, acker);
		}

		public void StubPublish<T>(T message, IMessageAck acker = null)
		{
			var props = new BasicProperties();
			var bytes = this.DefaultSerializer.TypedSerialize<T>(message, props);
			var env = new MessageEnvelope(props, bytes);

			StubPublish(env, acker);
		}

		public MessageEnvelope StubRespond(MessageEnvelope requestMessage, IMessageAck acker = null)
		{
			if (!_respondCalled) throw new Exception("Not responder set up");
			if (acker == null) acker = new StubMessageAcker();

			return _responder(requestMessage, acker);
		}

		public MessageEnvelope<TReply> StubRespond<TRequest, TReply>(TRequest requestMessage, IMessageAck acker = null)
			where TReply : class
		{
			var props = new BasicProperties();
			var bytes = this.DefaultSerializer.TypedSerialize<TRequest>(requestMessage, props);
			var env = new MessageEnvelope(props, bytes);

			return (MessageEnvelope<TReply>)StubRespond(env, acker);
		}

		// End Stub helpers

		public Subscription RespondRaw(Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond, ConsumerOptions options)
		{
			if (_consumedCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			if (_respondCalled) throw new Exception("setting up more than one responder for the same queue?");
			_respondCalled = true;

			_responder = onRespond;

			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public Subscription ConsumeRaw(Action<MessageEnvelope, IMessageAck> onReceived, ConsumerOptions options)
		{
			if (_respondCalled) throw new Exception("Consumed was called - have a respond and a consume on the same queue and you're gonna have a bad day");
			if (_consumedCalled) throw new Exception("setting up more than one consumer for the same queue?");
			_consumedCalled = true;

			_consumer  = onReceived;

			return new Subscription(_model, Guid.NewGuid().ToString());
		}

		public string Name { get; private set; }

		public IRabbitSerializer DefaultSerializer { get; set; }

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