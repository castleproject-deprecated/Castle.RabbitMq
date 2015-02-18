namespace Castle.RabbitMq.Serializers
{
    using System;

    public class JsonSerializer : IRabbitSerializer
    {
        public byte[] Serialize(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(byte[] data, Type type)
        {
            throw new NotImplementedException();
        }
    }
}