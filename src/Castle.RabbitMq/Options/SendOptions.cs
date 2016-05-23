namespace Castle.RabbitMq
{
	using System;

	public class SendOptions : TransportOptions
	{
		internal static	SendOptions	Default	= new SendOptions()
		{
			Mandatory =	false,
			Persist	= false,
		};

		internal static	SendOptions	Persistent = new SendOptions()
		{
			Mandatory =	false,
			Persist	= true,
		};

		public bool	Persist	{ get; set;	}
		public bool	Mandatory {	get; set; }

		public override	string ToString()
		{
			return String.Format("Mandatory: {0} Persist: {1}", Mandatory, Persist);
		}

		protected bool Equals(SendOptions other)
		{
			return Persist == other.Persist && Mandatory == other.Mandatory;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SendOptions) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Persist.GetHashCode()*397) ^ Mandatory.GetHashCode();
			}
		}
	}

	public class RpcSendOptions	: SendOptions
	{
		internal new static RpcSendOptions Default = new RpcSendOptions()
		{
			Mandatory =	false,
			Persist	= false,
			Timeout	= TimeSpan.FromSeconds(30)
		};

		public TimeSpan	Timeout	{ get; set;	}

		public override	string ToString()
		{
			return String.Format("{0} Timeout: {1}", base.ToString(), Timeout);
		}

		protected bool Equals(RpcSendOptions other)
		{
			return base.Equals(other) && Timeout.Equals(other.Timeout);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RpcSendOptions) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode()*397) ^ Timeout.GetHashCode();
			}
		}
	}

}