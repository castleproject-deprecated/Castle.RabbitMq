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

		IModel Model { get;	}
	}

	public static class	RabbitChannelExtensions
	{
		public static IRabbitQueueBinding Bind(this	IRabbitChannel source, 
											   string exchange,	string queue,
											   string routingKeyOrFilter)
		{
			return (source as RabbitChannel).BindInternal(queue, exchange, routingKeyOrFilter);
		}

		public static IRabbitExchange DeclareExchange(this IRabbitChannel source, string name, RabbitExchangeType exchangeType)
		{
			return source.DeclareExchange(name,	new	ExchangeOptions()
			{
				ExchangeType = exchangeType,
				// defaults	from the original api:
				Durable	= false,
				AutoDelete = false
			});
		}
	}

}