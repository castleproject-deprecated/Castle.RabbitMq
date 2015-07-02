namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using RabbitMQ.Client;

	/// <summary>
	/// Used for rpc returns to indicate error raised by the callee. 
	/// </summary>
	[Serializable]
	public class ErrorResponse
	{
		private const string Header = "castle.rabbitmq.exception";
		private const int FlagVal = 1;

		public Exception Exception { get; set; }

		public static void FlagHeaders(IBasicProperties properties)
		{
			var headers = properties.Headers;
			if (headers == null)
			{
				headers = new Dictionary<string, object>();
				properties.Headers = headers;
			}
			headers[Header] = FlagVal;
		}

		public static bool IsHeaderErrorFlag(IBasicProperties properties)
		{
			var headers = properties.Headers;
			object v;
			return headers != null && headers.TryGetValue(Header, out v) && ((int)v) == FlagVal;
		}
	}
}