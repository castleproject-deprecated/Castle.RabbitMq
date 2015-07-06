namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;

	public interface IRabbitSerializer
	{
		byte[] Serialize(object instance, IBasicProperties prop);

		object Deserialize(byte[] data, Type type, IBasicProperties prop);
	}
}