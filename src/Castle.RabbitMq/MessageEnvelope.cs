namespace Castle.RabbitMq
{
    public class MessageEnvelope
    {
        // string consumerTag,
        // ulong deliveryTag,
        // bool redelivered,
        // string exchange,
        // string routingKey,
        // IBasicProperties properties,
        // byte[] body
    }

    public class MessageEnvelope<T> : MessageEnvelope where T : class
    {
    }

}
