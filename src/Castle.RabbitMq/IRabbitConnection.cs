namespace Castle.RabbitMq
{
    using System;

    public interface IRabbitConnection : IDisposable
    {
        IRabbitChannel CreateChannel(bool withConfirmation = false, ushort? prefetchCount = null);
    }
}