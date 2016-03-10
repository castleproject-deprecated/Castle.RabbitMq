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
	}

}