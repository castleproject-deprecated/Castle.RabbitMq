namespace Castle.RabbitMq.Stubs
{
	public class StubRabbitQueueBinding : IRabbitQueueBinding
	{
		public IRabbitExchange Exchange { get; private set; }
		public IRabbitQueue Queue { get; private set; }
		public string RoutingKeyOrFilter { get; private set; }

		public StubRabbitQueueBinding(IRabbitExchange exchange, IRabbitQueue queue, string routingKeyOrFilter)
		{
			Exchange = exchange;
			Queue = queue;
			RoutingKeyOrFilter = routingKeyOrFilter;
		}

		public void Unbind()
		{
		}

		public void Dispose()
		{
		}
	}
}