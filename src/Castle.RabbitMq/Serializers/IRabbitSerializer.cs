namespace Castle.RabbitMq
{
    using System;

    public interface IRabbitSerializer
    {
        byte[] Serialize(Type type, object instance);

        object Deserialize(byte[] data, Type type);
    }
}