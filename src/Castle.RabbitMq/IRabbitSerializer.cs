namespace Castle.RabbitMq
{
	using System;
	using RabbitMQ.Client;

	public interface IRabbitSerializer
	{
		byte[] Serialize<T>(T instance,	IBasicProperties prop);

		T Deserialize<T>(byte[]	data, IBasicProperties prop);

//		  byte[] Serialize(Type	type, object instance);
//
		object Deserialize(byte[] data,	Type type, IBasicProperties	prop);
	}
}