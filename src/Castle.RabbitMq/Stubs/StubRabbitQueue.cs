namespace Castle.RabbitMq.Stubs
{
	using System;

	public class StubRabbitQueue : IRabbitQueue
	{
		public Subscription Respond<TRequest, TResponse>(Func<MessageEnvelope<TRequest>, IMessageAck, TResponse> onRespond, ConsumerOptions options)
		{
			throw new NotImplementedException();
		}

		public Subscription Consume<T>(Action<MessageEnvelope<T>, IMessageAck> onReceived, ConsumerOptions options)
		{
			throw new NotImplementedException();
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		public uint ConsumerCount
		{
			get { throw new NotImplementedException(); }
		}

		public uint MessageCount
		{
			get { throw new NotImplementedException(); }
		}

		public void Purge()
		{
			throw new NotImplementedException();
		}

		public void Delete()
		{
			throw new NotImplementedException();
		}

		public void Delete(bool ifUnused, bool ifEmpty)
		{
			throw new NotImplementedException();
		}
	}
}