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

		protected bool Equals(ExchangeOptions other)
		{
			return ExchangeType == other.ExchangeType && string.Equals(CustomExchangeType, other.CustomExchangeType) && Durable == other.Durable && AutoDelete == other.AutoDelete;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ExchangeOptions) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) ExchangeType;
				hashCode = (hashCode*397) ^ (CustomExchangeType != null ? CustomExchangeType.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Durable.GetHashCode();
				hashCode = (hashCode*397) ^ AutoDelete.GetHashCode();
				return hashCode;
			}
		}
	}
}