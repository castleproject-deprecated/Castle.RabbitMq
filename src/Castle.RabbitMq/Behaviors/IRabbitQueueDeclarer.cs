namespace Castle.RabbitMq
{
	public interface IRabbitQueueDeclarer
	{
		IRabbitQueue DeclareQueue(string name, QueueOptions	options);
	}

	public static class	QueueDeclarerExtensions
	{
		///	<summary>
		///	The	queue is declared non-passive, non-durable,
		///	but	exclusive and autodelete, with no arguments. The
		///	server autogenerates a name	for	the	queue
		///	</summary>
		///	<returns></returns>
		public static IRabbitQueue DeclareEphemeralQueue(this IRabbitQueueDeclarer source, QueueOptions	options)
		{
			options	= options ?? new QueueOptions();
			options.AutoDelete = true;
			options.Exclusive =	true;
			options.Durable	= false;

			return source.DeclareQueue("", options);
		}

		///	<summary>
		///	The	queue is declared non-passive, non-durable,
		///	but	exclusive and autodelete, with no arguments. The
		///	server autogenerates a name	for	the	queue
		///	</summary>
		public static IRabbitQueue DeclareEphemeralQueue(this IRabbitQueueDeclarer source)
		{
			return DeclareEphemeralQueue(source, null);
		}


		public static IRabbitQueue DeclareQueue(this IRabbitQueueDeclarer source, string name)
		{
			return source.DeclareQueue(name, new QueueOptions()
			{
				// defaults	from the api
				AutoDelete = false,	
				Durable	= false, 
				Exclusive =	false
			});
		}
	}
}