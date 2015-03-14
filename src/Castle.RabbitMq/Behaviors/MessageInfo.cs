namespace Castle.RabbitMq
{
    using System;

    public class MessageInfo
    {
        public ulong Tag { get; set; }

        public override string ToString()
        {
            return String.Format("Tag: {0}", Tag);
        }
    }
}