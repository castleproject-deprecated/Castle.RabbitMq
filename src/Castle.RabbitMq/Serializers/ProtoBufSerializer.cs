namespace Castle.RabbitMq.Serializers
{
    using System;
    using System.IO;
    using ProtoBuf.Meta;
    using RabbitMQ.Client;

	public class ProtoBufSerializer : IRabbitSerializer
    {
		private readonly RuntimeTypeModel _rtModel;

		public ProtoBufSerializer() : this(RuntimeTypeModel.Default)
		{
		}

		public ProtoBufSerializer(RuntimeTypeModel rtModel)
		{
			_rtModel = rtModel;
		}

		public byte[] Serialize<T>(T instance, IBasicProperties prop)
		{
			var memoryStream = new MemoryStream();
			_rtModel.Serialize(memoryStream, instance);
			var buffer = memoryStream.GetBuffer();
			var length = (int) memoryStream.Length;
			if (buffer.Length == length) return buffer;

			var data = new byte[length];
			Buffer.BlockCopy(buffer, 0, data, 0, length);
			return data;
		}

		public object Deserialize(byte[] data, Type type, IBasicProperties prop)
        {
			return _rtModel.Deserialize(new MemoryStream(data), null, type);
        }

		public T Deserialize<T>(byte[] data, IBasicProperties prop)
		{
			return (T) Deserialize(data, typeof(T), prop);
		}
	}
}
