namespace Castle.RabbitMq
{
    using System.Collections.Generic;

    public class QueueOptions
    {
        public QueueOptions()
        {
            this.Durable = true;
            this.Exclusive = true;
            this.AutoDelete = true;
        }

        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        IRabbitSerializer Serializer { get; set; }
    }
}