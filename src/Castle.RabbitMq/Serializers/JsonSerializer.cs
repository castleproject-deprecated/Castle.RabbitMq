namespace Castle.RabbitMq.Serializers
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

	public class JsonSerializer : IRabbitSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

		public byte[] Serialize<T>(T instance, IBasicProperties prop)
        {
            var json = JsonConvert.SerializeObject(instance, Formatting.None, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

		public T Deserialize<T>(byte[] data, IBasicProperties prop)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

		public object Deserialize(byte[] data, Type type, IBasicProperties prop)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject(json, type, _settings);
        }
    }
}