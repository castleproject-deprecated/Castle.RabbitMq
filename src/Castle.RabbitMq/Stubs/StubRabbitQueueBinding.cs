namespace Castle.RabbitMq.Stubs
{
	using System.Diagnostics;

	[DebuggerDisplay("StubRabbitQueueBinding E:{ExchangeName} Q:{QueueName} R:{RoutingKeyOrFilter}")]
	public class StubRabbitQueueBinding : IRabbitQueueBinding
	{
		public IRabbitExchange Exchange { get; private set; }
		public IRabbitQueue Queue { get; private set; }
		public string RoutingKeyOrFilter { get; private set; }

		// Stub helpers
		public string ExchangeName { get { return this.Exchange.Name; } }
		public string QueueName { get { return this.Queue.Name; } }
		// End Stub helpers

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