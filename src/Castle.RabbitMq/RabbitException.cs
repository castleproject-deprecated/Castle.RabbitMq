namespace Castle.RabbitMq
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class RabbitException : Exception
	{
		public RabbitException(string message) : base(message)
		{
		}

		public RabbitException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RabbitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}