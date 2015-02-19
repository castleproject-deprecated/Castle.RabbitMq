namespace Castle.RabbitMq
{
    using System;

    public class SendOptions
    {
        public static SendOptions Default = new SendOptions()
        {
            Mandatory = false,
            Persist = false,
            Routing = string.Empty
        }; 

        public string Routing { get; set; }
        public bool Persist { get; set; }
        public bool Mandatory { get; set; }

        
    }

    public interface ISender
    {
        Action<int> ConfirmationCallback { get; set; }

// 
        MessageInfo Send<T>(T message, MessageProperties properties = null, SendOptions options = null) where T : class;

//        MessageInfo SendRaw(byte[] body, MessageProperties properties = null, SendOptions options = null);
    }
}