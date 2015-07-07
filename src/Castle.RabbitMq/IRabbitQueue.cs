namespace Castle.RabbitMq
{
	public interface IRabbitQueue :	IRabbitQueueConsumer
	{
		string Name	{ get; }
		uint ConsumerCount { get; }
		uint MessageCount {	get; }

		void Purge();

		void Delete();
		void Delete(bool ifUnused, bool	ifEmpty);

		IRabbitSerializer DefaultSerializer { get; }
	}


}