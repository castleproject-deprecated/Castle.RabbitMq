namespace Castle.RabbitMq
{
    public class ConsumerOptions : TransportOptions
    {
        internal static ConsumerOptions Default = new ConsumerOptions();

        // bool exclusive
        // IDictionary<string, object> arguments

        public ConsumerOptions()
        {
            this.NoAck = true;
        }

        /// <summary>
        /// Note that if the "noAck" option is enabled (which it is by default), 
        /// then received deliveries are automatically acked within the 
        /// server before they are even transmitted across the network to us. 
        /// </summary>
        public bool NoAck { get; set; }
    }
}