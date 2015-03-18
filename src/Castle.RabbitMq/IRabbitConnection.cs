namespace Castle.RabbitMq
{
    using System;
    using System.Collections;
    using RabbitMQ.Client;
    using Serializers;

    public class ChannelOptions
    {
        internal static ChannelOptions Default = new ChannelOptions();

        public ChannelOptions()
        {
            this.DefaultSerializer  = new JsonSerializer();
        }

        public bool WithConfirmation { get; set; }
        public ushort? PrefetchCount { get; set; }
        public IRabbitSerializer DefaultSerializer { get; set; }
    }

    public interface IRabbitConnection : IDisposable
    {
        IRabbitChannel CreateChannel(ChannelOptions options = null);

        IRabbitConsole Console { get; }
    }
}