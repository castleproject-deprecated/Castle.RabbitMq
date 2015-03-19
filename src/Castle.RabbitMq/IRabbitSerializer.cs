namespace Castle.RabbitMq
{
    using System;

    public interface IRabbitSerializer
    {
        byte[] Serialize<T>(T instance);

        T Deserialize<T>(byte[] data);

//        byte[] Serialize(Type type, object instance);
//
        object Deserialize(byte[] data, Type type);
    }
}