namespace Castle.RabbitMq
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using MgmtConsole;

	public interface IRabbitConsole
	{
		Task<IEnumerable<ExchangeInfo>>	GetExchangesAsync(string vhost = "/");

		Task<IEnumerable<QueueInfo>> GetQueuesAsync(string vhost = "/");

		Task<IEnumerable<BindingInfo>> GetBindingsAsync(string vhost = "/");

		Task<IEnumerable<BindingInfo>> GetBindingsAsync(string exchange, string	queue, string vhost	= "/");

		Task<IEnumerable<BindingInfo>> GetBindingsAsync(string exchange, string	vhost =	"/");

		// TODO:
		// /cluster-name
		// /nodes
		// /nodes/<name>
		// /definitions	-->	returns	all	above
	}
}