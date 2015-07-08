namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;

	public static class RabbitSerializerExtensions
	{
		public static byte[] TypedSerialize<T>(this IRabbitSerializer source, T instance, IBasicProperties properties)
		{
			if (properties.Type == null)
			{
				var msgType = instance.GetType();

				if (Castle.Core.ProxyServices.IsDynamicProxy(instance.GetType()))
					throw new Exception("Serialization of a proxy type will be really bad for your sanity");

				var fullname = msgType.AssemblyQualifiedName;
				var sndComma = fullname.IndexOf("Version=", msgType.FullName.Length, StringComparison.Ordinal);

				properties.Type = fullname.Substring(0, sndComma - 2); ;
			}

			return source.Serialize(instance, properties);
		}

		public static T TypedDeserialize<T>(this IRabbitSerializer source, byte[] data, IBasicProperties properties)
		{
			properties.Type.AssertNotNullOrEmpty("The message property 'Type' must have a qualified type name");

			var expectedType = Type.GetType(properties.Type);

			return (T) source.Deserialize(data, expectedType, properties);
		}
	}
}