namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Used to specify queue options, including the <see cref="IRabbitSerializer"/>
    /// </summary>
    public class QueueOptions
    {
        internal static QueueOptions Default = new QueueOptions()
        {
            Durable = false,
            Exclusive = false,
            AutoDelete = false
        };

        public QueueOptions()
        {
            this.Durable = false;
            this.Exclusive = false;
            this.AutoDelete = false;
        }

        /// <summary>
        /// Default serializer to use. Can be overriden in a per-operation basis
        /// </summary>
        public IRabbitSerializer Serializer { get; set; }

        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        public override string ToString()
        {
            return String.Format("Durable: {0} Exclusive: {1} AutoDelete: {2}", Durable, Exclusive, AutoDelete);
        }
    }
}