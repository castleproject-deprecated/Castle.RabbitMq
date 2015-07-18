namespace Castle.RabbitMq.Serializers
{
	using System;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using RabbitMQ.Client;

	public class DotNetSerializer : IRabbitSerializer
	{
		public byte[] Serialize(object instance, IBasicProperties prop)
		{
			var formatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();
			formatter.Serialize(memoryStream, instance);

			var buffer = memoryStream.GetBuffer();
			var length = (int)memoryStream.Length;
			if (buffer.Length == length) return buffer;

			var data = new byte[length];
			Buffer.BlockCopy(buffer, 0, data, 0, length);
			return data;
		}

		public object Deserialize(byte[] data, Type type, IBasicProperties prop)
		{
			var formatter = new BinaryFormatter();

			return formatter.Deserialize(new MemoryStream(data));
		}
	}
}