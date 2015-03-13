namespace Castle.RabbitMq
{
    public interface IRabbitQueueDeclarer
    {
        IRabbitQueue DeclareQueue(string name, QueueOptions options);

        /// <summary>
        /// The queue is declared non-passive, non-durable,
        /// but exclusive and autodelete, with no arguments. The
        /// server autogenerates a name for the queue
        /// </summary>
        /// <returns></returns>
        IRabbitQueue DeclareEphemeralQueue(QueueOptions options);
    }

    public static class QueueDeclarerExtensions
    {
        public static IRabbitQueue DeclareQueue(this IRabbitQueueDeclarer source, string name)
        {
            return source.DeclareQueue(name, new QueueOptions()
            {
                // defaults from the api
                AutoDelete = false, 
                Durable = false, 
                Exclusive = false
            });
        }
    }
}