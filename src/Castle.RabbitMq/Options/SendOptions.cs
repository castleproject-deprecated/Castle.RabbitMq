namespace Castle.RabbitMq
{
    using System;

    public class SendOptions : TransportOptions
    {
        internal static SendOptions Default = new SendOptions()
        {
            Mandatory = false,
            Persist = false,
            Immediate = false
        }; 

        public bool Persist { get; set; }
        public bool Immediate { get; set; }
        public bool Mandatory { get; set; }

        public override string ToString()
        {
            return String.Format("Mandatory: {0} Persist: {1} Immediate: {2}", 
                Mandatory, Persist, Immediate);
        }
    }

    public class RpcSendOptions : SendOptions
    {
        new internal static RpcSendOptions Default = new RpcSendOptions()
        {
            Mandatory = false,
            Persist = false,
            Immediate = false,
            Timeout = TimeSpan.FromSeconds(30)
        };

        public TimeSpan Timeout { get; set; }

        public override string ToString()
        {
            return String.Format("{0} Timeout: {1}", base.ToString(), Timeout);
        }
    }

}