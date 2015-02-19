namespace Castle.RabbitMq
{
    public interface ISender
    {
// 
        MessageInfo Send<T>(T message, string routing = "", bool persist = false, bool mandatory = false, MessageProperties properties = null) where T : class;

//        MessageInfo SendRaw(byte[] body, bool persist = false, bool mandatory = false, MessageProperties properties = null);
    }
}