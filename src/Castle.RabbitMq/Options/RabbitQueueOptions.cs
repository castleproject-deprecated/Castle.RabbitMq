namespace Castle.RabbitMq
{
    public class RabbitQueueOptions
    {
        public RabbitQueueOptions()
        {
            this.Durable = true;
            this.Exclusive = true;
            this.AutoDelete = true;
        }

        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }

        IRabbitSerializer Serializer { get; set; }
    }
}