namespace Castle.RabbitMq
{
	using System;
	using Serializers;

	public class ChannelOptions
	{
		internal static	ChannelOptions Default = new ChannelOptions();

		public ChannelOptions()
		{
			this.DefaultSerializer = new DotNetSerializer();
		}

		public bool	WithConfirmation { get;	set; }
		public ushort? PrefetchCount { get;	set; }
		public IRabbitSerializer DefaultSerializer { get; set; }
	}

	public sealed class RabbitConnectionInfo
	{
		public string HostName { get; private set; }
		public string VirtualHost { get; private set; }
		public string UserName { get; private set; }
		public string Password { get; private set; }
		public int Port { get; private set; }

		public RabbitConnectionInfo(string hostName, string virtualHost, string userName, string password, int port)
		{
			HostName = hostName;
			VirtualHost = virtualHost;
			UserName = userName;
			Password = password;
			Port = port;
		}
	}

	public interface IRabbitConnection : IDisposable
	{
		IRabbitChannel CreateChannel(ChannelOptions	options	= null);

		RabbitConnectionInfo ConnectionInfo { get; }

		///	<summary>
		///	Duplicates the connection, opening a new one
		///	</summary>
		///	<returns>New connection</returns>
		IRabbitConnection NewConnection();
	}
}