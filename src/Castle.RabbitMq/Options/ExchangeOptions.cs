namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;

    public enum RabbitExchangeType
    {
        Direct, Fanout, Headers, Topic
    }

    public class ExchangeOptions
    {
        public ExchangeOptions()
        {
            this.AutoDelete = true;
            this.Durable = false;
            this.ExchangeType = RabbitExchangeType.Direct;
        }

        public RabbitExchangeType ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        public override string ToString()
        {
            return String.Format("{0} Durable {1} AutoDelete {2}", ExchangeType, Durable, AutoDelete);
        }
    }
}