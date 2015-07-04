namespace Castle.RabbitMq.Stubs
{
	using System;
	using System.Collections.Generic;

	public class StubRabbitExchange : IRabbitExchange
	{
		private Func<object, object> _rpcFunc;
		private readonly List<Tuple<MessageEnvelope, string, SendOptions>> _sendRaws;
		private readonly List<Tuple<MessageEnvelope, string, SendOptions>> _sends;
		private readonly List<Tuple<MessageEnvelope, string, RpcSendOptions>> _sendRequestsRaw;
		private readonly List<Tuple<MessageEnvelope, string, RpcSendOptions>> _sendRequests;
		private readonly List<StubRabbitQueue> _queuesDeclared;
		private readonly List<StubRabbitQueue> _queuesDeclaredNoWait;
		private readonly List<StubRabbitQueueBinding> _bindings;
		private readonly List<StubRabbitQueueBinding> _bindingsNoWait;

		public StubRabbitExchange(string name, ExchangeOptions options, Func<object, object> rpcFunc = null)
		{
			_rpcFunc = rpcFunc;

			this.Name = name;
			this.Options = options;

			_sendRaws = new List<Tuple<MessageEnvelope, string, SendOptions>>();
			_sends = new List<Tuple<MessageEnvelope, string, SendOptions>>();
			_sendRequestsRaw = new List<Tuple<MessageEnvelope, string, RpcSendOptions>>();
			_sendRequests = new List<Tuple<MessageEnvelope, string, RpcSendOptions>>();

			_queuesDeclared = new List<StubRabbitQueue>();
			_queuesDeclaredNoWait = new List<StubRabbitQueue>();

			_bindings = new List<StubRabbitQueueBinding>();
			_bindingsNoWait = new List<StubRabbitQueueBinding>();
		}

		// Stub helpers

		public bool Deleted { get; private set; }

		public Func<object, object> RpcFunc
		{
			get { return _rpcFunc; }
			set { _rpcFunc = value; }
		}

		public List<StubRabbitQueueBinding> BindingsNoWait
		{
			get { return _bindingsNoWait; }
		}

		public List<StubRabbitQueueBinding> Bindings
		{
			get { return _bindings; }
		}

		public List<StubRabbitQueue> QueuesDeclaredNoWait
		{
			get { return _queuesDeclaredNoWait; }
		}

		public List<StubRabbitQueue> QueuesDeclared
		{
			get { return _queuesDeclared; }
		}

		public List<Tuple<MessageEnvelope, string, RpcSendOptions>> SendRequests
		{
			get { return _sendRequests; }
		}

		public List<Tuple<MessageEnvelope, string, RpcSendOptions>> SendRequestsRaw
		{
			get { return _sendRequestsRaw; }
		}

		public List<Tuple<MessageEnvelope, string, SendOptions>> Sends
		{
			get { return _sends; }
		}

		public List<Tuple<MessageEnvelope, string, SendOptions>> SendRaws
		{
			get { return _sendRaws; }
		}

		// End Stub helpers

		public string Name { get; private set; }
		public ExchangeOptions Options { get; private set; }

		public MessageInfo SendRaw(byte[] body, string routingKey = "", 
								   MessageProperties properties = null, 
								   SendOptions options = null)
		{
			_sendRaws.Add(Tuple.Create(new MessageEnvelope(properties, body), routingKey, options));
			return new MessageInfo();
		}

		public MessageInfo Send<T>(T message, string routingKey = "", 
								   MessageProperties properties = null, 
								   SendOptions options = null) where T : class
		{
			_sends.Add(Tuple.Create<MessageEnvelope,string, SendOptions>(
				new MessageEnvelope<T>(properties, message, null), routingKey, options));
			return new MessageInfo();
		}

		public MessageEnvelope SendRequestRaw(byte[] data, string routingKey = "", 
											  MessageProperties properties = null,
											  RpcSendOptions options = null)
		{
			_sendRequestsRaw.Add(Tuple.Create(new MessageEnvelope(properties, data), routingKey, options));
			
			return new MessageEnvelope(new MessageProperties(), new byte[0]);
		}

		public TResponse SendRequest<TRequest, TResponse>(TRequest request, string routingKey = "", 
														  MessageProperties properties = null,
														  RpcSendOptions options = null) 
			where TRequest : class where TResponse : class
		{
			var reply = _rpcFunc(request);

			_sendRequests.Add(Tuple.Create(new MessageEnvelope(properties, new byte[0]), routingKey, options));

			return (TResponse) reply;
		}

		public IRabbitQueue DeclareQueue(string name, QueueOptions options)
		{
			var queue = new StubRabbitQueue(name, options);
			_queuesDeclared.Add(queue);
			return queue;
		}

		public IRabbitQueue DeclareQueueNoWait(string name, QueueOptions options)
		{
			var queue = new StubRabbitQueue(name, options);
			_queuesDeclaredNoWait.Add(queue);
			return queue;
		}

		public IRabbitQueueBinding Bind(IRabbitQueue queue, string routingKeyOrFilter)
		{
			var binding = new StubRabbitQueueBinding(this, queue, routingKeyOrFilter);
			_bindings.Add(binding);
			return binding;
		}

		public IRabbitQueueBinding BindNoWait(IRabbitQueue queue, string routingKeyOrFilter)
		{
			var binding = new StubRabbitQueueBinding(this, queue, routingKeyOrFilter);
			_bindingsNoWait.Add(binding);
			return binding;
		}

		public void Delete()
		{
			this.Deleted = true;
		}

		public void Delete(bool ifUnused)
		{
			this.Deleted = true;
		}
	}
}