namespace Castle.RabbitMq
{
    using System.Collections.Generic;

    public enum RabbitExchangeType
    {
        Direct, Fanout, Headers, Topic
    }

    public class RabbitExchangeOptions
    {
        public RabbitExchangeOptions()
        {
            this.AutoDelete = true;
            this.Durable = false;
            this.ExchangeType = RabbitExchangeType.Direct;
        }

        public RabbitExchangeType ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}