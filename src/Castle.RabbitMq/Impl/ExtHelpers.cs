namespace Castle.RabbitMq
{
	using System;

	internal static class ExtHelpers
	{
		public static string ToExchangeType(this ExchangeOptions source)
		{
			if (source.ExchangeType == RabbitExchangeType.Custom)
			{
				if (string.IsNullOrEmpty(source.CustomExchangeType)) 
					throw new ArgumentException("If the exchange type is set to 'Custom' you must provide its type in the property CustomExchangeType");

				return source.CustomExchangeType;
			}

			return source.ExchangeType.ToString().ToLowerInvariant();
		}
	}
}