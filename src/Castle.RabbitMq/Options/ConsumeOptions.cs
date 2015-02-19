namespace Castle.RabbitMq
{
    public class ConsumeOptions
    {
        public ConsumeOptions()
        {
            this.AutoAck = true;
        }

        public bool AutoAck { get; set; }
    }
}