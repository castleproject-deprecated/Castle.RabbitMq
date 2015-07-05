namespace Castle.RabbitMq.Stubs
{
	using System;
	using RabbitMQ.Client;

	public class StubRabbitSerializer : IRabbitSerializer
	{
//		public byte[] Serialize<T>(T instance, IBasicProperties prop)
//		{
//			throw new NotImplementedException();
//		}

//		public T Deserialize<T>(byte[] data, IBasicProperties prop)
//		{
//			throw new NotImplementedException();
//		}

		public byte[] Serialize(object instance, IBasicProperties prop)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(byte[] data, Type type, IBasicProperties prop)
		{
			throw new NotImplementedException();
		}
	}
}