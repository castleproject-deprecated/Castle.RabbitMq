namespace Castle.RabbitMq.Stubs
{
	using System;
	using System.Collections.Concurrent;
	using System.Security.Cryptography;
	using RabbitMQ.Client;

	/// <summary>
	/// Fake serialization by generating random bytes
	/// and matching those to the instance. 
	/// 
	/// issue being whatever serialization method we could chose
	/// would have different implications 
	/// (proto requires mapping, 
	/// json wont support subclasses, 
	/// .net requires serialization attributes)
	/// 
	/// </summary>
	public class StubRabbitSerializer : IRabbitSerializer
	{
		private readonly RandomNumberGenerator _number = new RNGCryptoServiceProvider();

		
		private readonly ConcurrentDictionary<byte[], object> _byte2instance = new ConcurrentDictionary<byte[], object>(); 

		public byte[] Serialize(object instance, IBasicProperties prop)
		{
			var buffer = new byte[16];
			_number.GetNonZeroBytes(buffer);

			_byte2instance[buffer] = instance;

			return buffer;
		}

		public object Deserialize(byte[] data, Type type, IBasicProperties prop)
		{
			return _byte2instance[data];
		}
	}
}