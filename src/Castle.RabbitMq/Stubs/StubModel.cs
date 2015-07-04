namespace Castle.RabbitMq.Stubs
{
	using System;
	using System.Collections.Generic;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;

	public class StubModel : IModel
	{
		public event EventHandler<BasicAckEventArgs> BasicAcks;
		public event EventHandler<BasicNackEventArgs> BasicNacks;
		public event EventHandler<EventArgs> BasicRecoverOk;
		public event EventHandler<BasicReturnEventArgs> BasicReturn;
		public event EventHandler<CallbackExceptionEventArgs> CallbackException;
		public event EventHandler<FlowControlEventArgs> FlowControl;
		public event EventHandler<ShutdownEventArgs> ModelShutdown;

		public void Abort()
		{
			throw new NotImplementedException();
		}

		public void Abort(ushort replyCode, string replyText)
		{
			throw new NotImplementedException();
		}

		public void BasicAck(ulong deliveryTag, bool multiple)
		{
			throw new NotImplementedException();
		}

		public void BasicCancel(string consumerTag)
		{
			throw new NotImplementedException();
		}

		public string BasicConsume(string queue, bool noAck, IBasicConsumer consumer)
		{
			throw new NotImplementedException();
		}

		public string BasicConsume(string queue, bool noAck, string consumerTag, IBasicConsumer consumer)
		{
			throw new NotImplementedException();
		}

		public string BasicConsume(string queue, bool noAck, string consumerTag, IDictionary<string, object> arguments, IBasicConsumer consumer)
		{
			throw new NotImplementedException();
		}

		public string BasicConsume(string queue, bool noAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments,
			IBasicConsumer consumer)
		{
			throw new NotImplementedException();
		}

		public BasicGetResult BasicGet(string queue, bool noAck)
		{
			throw new NotImplementedException();
		}

		public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
		{
			throw new NotImplementedException();
		}

		public void BasicPublish(PublicationAddress addr, IBasicProperties basicProperties, byte[] body)
		{
			throw new NotImplementedException();
		}

		public void BasicPublish(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
		{
			throw new NotImplementedException();
		}

		public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
		{
			throw new NotImplementedException();
		}

		public void BasicPublish(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties,
			byte[] body)
		{
			throw new NotImplementedException();
		}

		public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
		{
			throw new NotImplementedException();
		}

		public void BasicRecover(bool requeue)
		{
			throw new NotImplementedException();
		}

		public void BasicRecoverAsync(bool requeue)
		{
			throw new NotImplementedException();
		}

		public void BasicReject(ulong deliveryTag, bool requeue)
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
		}

		public void Close(ushort replyCode, string replyText)
		{
			throw new NotImplementedException();
		}

		public void ConfirmSelect()
		{
			throw new NotImplementedException();
		}

		public IBasicProperties CreateBasicProperties()
		{
			throw new NotImplementedException();
		}

		public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void ExchangeBind(string destination, string source, string routingKey)
		{
			throw new NotImplementedException();
		}

		public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeclare(string exchange, string type, bool durable)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeclare(string exchange, string type)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeclarePassive(string exchange)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDelete(string exchange, bool ifUnused)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDelete(string exchange)
		{
			throw new NotImplementedException();
		}

		public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
		{
			throw new NotImplementedException();
		}

		public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void ExchangeUnbind(string destination, string source, string routingKey)
		{
			throw new NotImplementedException();
		}

		public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void QueueBind(string queue, string exchange, string routingKey)
		{
			throw new NotImplementedException();
		}

		public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public QueueDeclareOk QueueDeclare()
		{
			throw new NotImplementedException();
		}

		public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public QueueDeclareOk QueueDeclarePassive(string queue)
		{
			throw new NotImplementedException();
		}

		public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
		{
			throw new NotImplementedException();
		}

		public uint QueueDelete(string queue)
		{
			throw new NotImplementedException();
		}

		public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
		{
			throw new NotImplementedException();
		}

		public uint QueuePurge(string queue)
		{
			throw new NotImplementedException();
		}

		public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
		{
			throw new NotImplementedException();
		}

		public void TxCommit()
		{
			throw new NotImplementedException();
		}

		public void TxRollback()
		{
			throw new NotImplementedException();
		}

		public void TxSelect()
		{
			throw new NotImplementedException();
		}

		public bool WaitForConfirms()
		{
			throw new NotImplementedException();
		}

		public bool WaitForConfirms(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
		{
			throw new NotImplementedException();
		}

		public void WaitForConfirmsOrDie()
		{
			throw new NotImplementedException();
		}

		public void WaitForConfirmsOrDie(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public int ChannelNumber
		{
			get { throw new NotImplementedException(); }
		}

		public ShutdownEventArgs CloseReason
		{
			get { throw new NotImplementedException(); }
		}

		public IBasicConsumer DefaultConsumer
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public bool IsClosed
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsOpen
		{
			get { throw new NotImplementedException(); }
		}

		public ulong NextPublishSeqNo
		{
			get { throw new NotImplementedException(); }
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}
}