namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Framing;


	class RpcResponder : IMessageConsumer
	{
		private	readonly IModel	_model;
		private readonly Func<MessageEnvelope, IMessageAck, MessageEnvelope> _onRespond;
//		private	readonly IRabbitSerializer _serializer;

		public RpcResponder(IModel model, 
//							IRabbitSerializer serializer,
							Func<MessageEnvelope, IMessageAck, MessageEnvelope> onRespond)
		{
			_model = model;
//			_serializer	= serializer;
			_onRespond = onRespond;
		}

		public void	OnNext(MessageEnvelope newMsg)
		{
			var incomingMsgProperties = newMsg.Properties;
			var replyQueue = incomingMsgProperties.ReplyTo;
			var correlationId = incomingMsgProperties.CorrelationId;

			IBasicProperties replyProperties = null;

			byte[] replyData = new byte[0];

			var msgAcker = CreateAcker(newMsg);

			try
			{
				var response = _onRespond(newMsg, msgAcker);
				response = response ?? new MessageEnvelope(new BasicProperties(), new byte[0]);
				replyData = response.Body;

				replyProperties = response.Properties ?? new BasicProperties();
			}
			catch(Exception e)
			{
				replyProperties = replyProperties ?? new BasicProperties();

				if (LogAdapter.LogEnabled) LogAdapter.LogError("Rpc", "OnNext error", e);

				// Empty data
//				replyData = _serializer.Serialize(new ErrorResponse() { Exception = e }, newProp);

				ErrorResponse.FlagHeaders(replyProperties);
			}

			// which call is which
			replyProperties.CorrelationId = correlationId;

			// Rabbit client will explode if this is not true
			(replyProperties is RabbitMQ.Client.Impl.BasicProperties)
				.AssertIsTrue("expected BasicProperties implementation of IBasicProperties");

			lock (_model)
			{
				_model.BasicPublish("", replyQueue, replyProperties, replyData);
			}
		}

		private MessageAck CreateAcker(MessageEnvelope newMsg)
		{
			var msgAcker = new MessageAck(() => { lock(_model) _model.BasicAck(newMsg.DeliveryTag, false); },
				(requeue) => { lock(_model) _model.BasicNack(newMsg.DeliveryTag, false, requeue); });
			
			return msgAcker;
		}
	}
}