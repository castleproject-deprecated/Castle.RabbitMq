namespace Castle.RabbitMq.Serializers
{
    using System;

    public class ProtoBufSerializer : IRabbitSerializer
    {
        public byte[] Serialize<T>(T instance)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] data)
        {
            throw new NotImplementedException();
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
