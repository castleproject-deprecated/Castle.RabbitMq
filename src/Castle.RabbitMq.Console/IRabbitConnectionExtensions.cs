namespace Castle.RabbitMq
{
	using MgmtConsole;

	public static class IRabbitConnectionExtensions
	{
		public static IRabbitConsole GetConsole(this IRabbitConnection source)
		{
			return new HttpBasedRabbitConsole(source.ConnectionInfo);
		}
	}
}