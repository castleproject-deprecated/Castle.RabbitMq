namespace Castle.RabbitMq
{
	public static class	RabbitChannelExtensions
	{
		public static IRabbitQueueBinding Bind(this IRabbitChannel source, 
			string exchange,	string queue,
			string routingKeyOrFilter)
		{
			return (source as IRabbitChannelInternal).BindInternal(false, queue, exchange, routingKeyOrFilter);
		}

		public static IRabbitQueueBinding BindNoWait(this IRabbitChannel source,
			string exchange, string queue,
			string routingKeyOrFilter)
		{
			return (source as IRabbitChannelInternal).BindInternal(true, queue, exchange, routingKeyOrFilter);
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