namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;

	public interface IRabbitChannel	: IRabbitQueueDeclarer,	IDisposable
	{
		// OnException?

		///	<summary>
		///	Mandatory message dropped by exchange because there	were no	matching queues.
		///	</summary>
		event Action<MessageUnroutedEventArgs> MessageUnrouted;

		IRabbitExchange	DefaultExchange	{ get; }

		IRabbitExchange	DeclareExchange(string name, ExchangeOptions options);

		IRabbitExchange	DeclareExchangeNoWait(string name, ExchangeOptions options);

		IRabbitQueueBinding	Bind(IRabbitExchange exchange, IRabbitQueue	queue, string routingKeyOrFilter);
		
		IRabbitQueueBinding BindNoWait(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter);

		void UnBind(IRabbitExchange	exchange, IRabbitQueue queue, string routingKeyOrFilter	= null);

		int ChannelNumber { get; }

		IModel Model { get;	}

		IRabbitSerializer Serializer { get; }
	}

	internal interface IRabbitChannelInternal
	{
		IRabbitQueueBinding BindInternal(bool nowait, string queue, string exchange, string routingKeyOrFilter);
	}
}