namespace Castle.RabbitMq
{
    using System;

    /// <summary>
    /// Represents a binding between an exchange and a queue
    /// </summary>
    public interface IRabbitQueueBinding : IDisposable
    {
        void Unbind();
    }
}