namespace Castle.RabbitMq.Serializers
{
    using System;
    using System.Text;
    using Newtonsoft.Json;

    public class JsonSerializer : IRabbitSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public byte[] Serialize<T>(T instance)
        {
            var json = JsonConvert.SerializeObject(instance, Formatting.None, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

//        public byte[] Serialize(Type type, object instance)
//        {
//            throw new NotImplementedException();
//        }
//
//        public object Deserialize(byte[] data, Type type)
//        {
//            throw new NotImplementedException();
//        }
    }
}