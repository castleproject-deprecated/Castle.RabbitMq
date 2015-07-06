namespace Castle.RabbitMq
{
	public interface IRabbitQueueDeclarer
	{
		IRabbitQueue DeclareQueue(string name, QueueOptions	options);

		IRabbitQueue DeclareQueueNoWait(string name, QueueOptions options);
	}
}