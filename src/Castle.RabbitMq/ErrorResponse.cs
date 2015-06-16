namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Used for rpc returns to indicate error raised by the callee. 
	/// </summary>
	[Serializable]
	public class ErrorResponse
	{
		private const string Header = "castle.rabbitmq.exception";
		private const int FlagVal = 1;

		public Exception Exception { get; set; }

		public static void FlagHeaders(IDictionary<string, object> headers)
		{
			headers[Header] = FlagVal;
		}

		public static bool IsHeaderErrorFlag(IDictionary<string, object> headers)
		{
			object v;
			return headers.TryGetValue(Header, out v) && ((int)v) == FlagVal;
		}
	}
}