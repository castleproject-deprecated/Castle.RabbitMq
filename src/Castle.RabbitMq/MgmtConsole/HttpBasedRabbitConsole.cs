namespace Castle.RabbitMq.MgmtConsole
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;
	using Newtonsoft.Json;
	using RabbitMQ.Client;

	public class HttpBasedRabbitConsole	: IRabbitConsole, IDisposable
	{
		private	readonly HttpClient	_client;

		public HttpBasedRabbitConsole(ConnectionFactory	connInfo)
		{
			_client	= new HttpClient()
			{
				BaseAddress	= new Uri(string.Format("http://{0}:15672/api/", connInfo.HostName))
			};

			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_client.DefaultRequestHeaders.Authorization	= new AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}",	connInfo.UserName, connInfo.Password))));
			_client.DefaultRequestHeaders.UserAgent.Add(new	ProductInfoHeaderValue("curl", "7.30.0"));
			_client.DefaultRequestHeaders.ConnectionClose =	true;
		}

		// /api/bindings/vhost	
		// A list of all bindings in a given virtual host.
		public async Task<IEnumerable<BindingInfo>>	GetBindingsAsync(string	vhost =	"/")
		{
			var	url	= string.Format("bindings/{0}",	WebUtility.UrlEncode(vhost));
			var	json = await _client.GetStringAsync(url);

			return JsonConvert.DeserializeObject<IEnumerable<BindingInfo>>(json);
		}

		// /api/bindings/vhost/e/exchange/q/queue
		// A list of all bindings between an exchange and a	queue. Remember, an	exchange and a queue can be	bound together many	times 
		public async Task<IEnumerable<BindingInfo>>	GetBindingsAsync(string	exchange, string queue,	string vhost = "/")
		{
			var	url	= string.Format("bindings/{0}/e/{1}/q/{2}",
				WebUtility.UrlEncode(vhost),
				WebUtility.UrlEncode(exchange),
				WebUtility.UrlEncode(queue));
			var	json = await _client.GetStringAsync(url);

			return JsonConvert.DeserializeObject<IEnumerable<BindingInfo>>(json);
		}

		// /api/exchanges/vhost/exchange/bindings/source
		// A list of all bindings between an exchange and a	queue. Remember, an	exchange and a queue can be	bound together many	times 
		public async Task<IEnumerable<BindingInfo>>	GetBindingsAsync(string	exchange, string vhost = "/")
		{
			//var url =	string.Format("http://{0}:15672/api/exchanges/{2}/{1}/bindings/source",	config.Host	?? "localhost",	exchange, config.VHost ?? "%2F");
			var	url	= string.Format("exchanges/{0}/{1}/bindings/source",
				WebUtility.UrlEncode(vhost),
				WebUtility.UrlEncode(exchange));
			var	json = await _client.GetStringAsync(url);

			return JsonConvert.DeserializeObject<IEnumerable<BindingInfo>>(json);
		}

		// /api/exchanges/vhost
		public async Task<IEnumerable<ExchangeInfo>> GetExchangesAsync(string vhost	= "/")
		{
			var	url	= string.Format("exchanges/{0}", WebUtility.UrlEncode(vhost));
			var	json = await _client.GetStringAsync(url);

			return JsonConvert.DeserializeObject<IEnumerable<ExchangeInfo>>(json);
		}

		// /api/queues/vhost
		public async Task<IEnumerable<QueueInfo>> GetQueuesAsync(string	vhost =	"/")
		{
			var	url	= string.Format("queues/{0}", WebUtility.UrlEncode(vhost));
			var	json = await _client.GetStringAsync(url);

			return JsonConvert.DeserializeObject<IEnumerable<QueueInfo>>(json);
		}

		public void	Dispose()
		{
			_client.Dispose();
		}
	}
}
