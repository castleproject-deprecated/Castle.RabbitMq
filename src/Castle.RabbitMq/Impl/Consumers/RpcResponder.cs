namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using RabbitMQ.Client;

	class RpcResponder<T, TResponse> : IMessageConsumer<T>
	{
		private	readonly IModel	_model;
		private	readonly IRabbitSerializer _serializer;
		private	readonly Func<MessageEnvelope<T>, IMessageAck, TResponse> _onRespond;

		public RpcResponder(IModel model, 
							IRabbitSerializer serializer, 
							Func<MessageEnvelope<T>, IMessageAck, TResponse> onRespond)
		{
			_model = model;
			_serializer	= serializer;
			_onRespond = onRespond;
		}

		public void	OnNext(MessageEnvelope<T> newMsg)
		{
			var	msgAcker = new MessageAck(() =>
			{
				lock (_model) _model.BasicAck(newMsg.DeliveryTag, false);
			}, (requeue) =>
			{
				lock (_model) _model.BasicNack(newMsg.DeliveryTag, false, requeue);
			});

			var prop = newMsg.Properties;
			var replyQueue = prop.ReplyTo;
			var correlationId = prop.CorrelationId;
			var newProp = _model.CreateBasicProperties();
			newProp.CorrelationId = correlationId;
			newProp.Headers = new Dictionary<string, object>();

			byte[] replyData = null;

			try
			{
				var response = _onRespond(newMsg, msgAcker);

				if (typeof(TResponse) == typeof(byte[]))
				{
					// ugly, but should	be safe	- 
					// would this cause	an expensive boxing/unboxing??
					replyData = (byte[])(object)response;
				}
				else
				{
					replyData = _serializer.Serialize(response, newProp);
				}
			}
			catch(Exception e)
			{
				// Empty data
				replyData = _serializer.Serialize(new ErrorResponse() { Exception = e }, newProp);

				ErrorResponse.FlagHeaders(newProp);
			}

			lock (_model)
			{
				_model.BasicPublish("",	replyQueue,	newProp, replyData);
			}
		}
	}
}