namespace Castle.RabbitMq.Stubs
{
	public class StubRabbitExchange : IRabbitExchange
	{
		public StubRabbitExchange()
		{
		}

		public MessageInfo SendRaw(byte[] body, string routingKey = "", MessageProperties properties = null, SendOptions options = null)
		{
			throw new System.NotImplementedException();
		}

		public MessageInfo Send<T>(T message, string routingKey = "", MessageProperties properties = null, SendOptions options = null) where T : class
		{
			throw new System.NotImplementedException();
		}

		public MessageEnvelope SendRequestRaw(byte[] data, string routingKey = "", MessageProperties properties = null,
			RpcSendOptions options = null)
		{
			throw new System.NotImplementedException();
		}

		public TResponse SendRequest<TRequest, TResponse>(TRequest request, string routingKey = "", MessageProperties properties = null,
			RpcSendOptions options = null) where TRequest : class where TResponse : class
		{
			throw new System.NotImplementedException();
		}

		public IRabbitQueue DeclareQueue(string name, QueueOptions options)
		{
			throw new System.NotImplementedException();
		}

		public IRabbitQueue DeclareQueueNoWait(string name, QueueOptions options)
		{
			throw new System.NotImplementedException();
		}

		public string Name
		{
			get { throw new System.NotImplementedException(); }
		}

		public IRabbitQueueBinding Bind(IRabbitQueue queue, string routingKeyOrFilter)
		{
			throw new System.NotImplementedException();
		}

		public IRabbitQueueBinding BindNoWait(IRabbitQueue queue, string routingKeyOrFilter)
		{
			throw new System.NotImplementedException();
		}

		public void Delete()
		{
			throw new System.NotImplementedException();
		}

		public void Delete(bool ifUnused)
		{
			throw new System.NotImplementedException();
		}
	}
}