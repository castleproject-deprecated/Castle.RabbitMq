namespace Castle.RabbitMq
{
    using RabbitMQ.Client;

    public static class RabbitConnector
    {
        // TODO: Set up logger here?

        public static RabbitConnection Connect(string hostname, int port = 5672, 
            string username = "guest", string password = "guest", string vhost = "/")
        {
            var connFactory = new ConnectionFactory()
            {
                HostName = hostname, 
                Port = port, 
                UserName = username,
                Password = password
            };

            var connection = connFactory.CreateConnection();

            return new RabbitConnection(connection);
        }
    }
}