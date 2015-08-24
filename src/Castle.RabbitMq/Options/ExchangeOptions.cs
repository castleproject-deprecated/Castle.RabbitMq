namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;

	public enum	RabbitExchangeType
	{
		Direct,
		Fanout,
		Headers, 
		Topic,
		Custom
	}

	public class ExchangeOptions
	{
		public ExchangeOptions()
		{
			this.AutoDelete	= true;
			this.Durable = false;
			this.ExchangeType =	RabbitExchangeType.Direct;
		}

		public RabbitExchangeType ExchangeType { get; set; }

		public string CustomExchangeType { get; set; }
		
		/// <summary>
		/// If set when creating a new exchange, the exchange will be marked as durable. 
		/// Durable exchanges remain active when a server restarts. Non-durable exchanges 
		/// (transient exchanges) are purged if/when a server restarts.
		/// </summary>
		public bool	Durable	{ get; set;	}
		
		/// <summary>
		/// If set, the exchange is deleted when all queues have finished using it.
		/// </summary>
		public bool	AutoDelete { get; set; }
		
		public IDictionary<string, object> Arguments { get;	set; }

		public override	string ToString()
		{
			return String.Format("{0} Durable: {1} AutoDelete: {2}", ExchangeType, Durable,	AutoDelete);
		}
	}
}