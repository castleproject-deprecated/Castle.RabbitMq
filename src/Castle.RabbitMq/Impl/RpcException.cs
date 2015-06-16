namespace Castle.RabbitMq
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class RpcException : Exception
	{
		public RpcException(string message) : base(message)
		{
		}

		public RpcException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}