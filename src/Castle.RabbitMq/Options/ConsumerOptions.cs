namespace Castle.RabbitMq
{

	public enum	ConsumerStrategy
	{
		///	<summary>
		///	Receives a predefined number of	messages from server (configurable via PreFetch)
		///	and	dispatches them	to your	code on	the	same thread	as Rabbit's	consumer thread
		///	</summary>
		Default,

		///	<summary>
		///	Receives a predefined number of	messages from server (configurable via PreFetch)
		///	and	put	them in	an in-memory queue.	The	messages from this queue are dispatch 
		///	to your	code in	a separate thread. 
		///	</summary>
		Queue
	}

	public class ConsumerOptions : TransportOptions
	{
		internal static	ConsumerOptions	Default	= new ConsumerOptions()
		{
			ConsumerStrategy = ConsumerStrategy.Default
		};
		internal static ConsumerOptions DefaultForRespond = new ConsumerOptions()
		{
			ConsumerStrategy = ConsumerStrategy.Default,
			ShouldSerializeExceptions = true
		};

		public ConsumerOptions()
		{
			this.NoAck = true;
		}

		///	<summary>
		///	Defines	the	behavior to	use	when getting the messages from the
		///	server and pushing them	to your	code. See enum values for details.
		///	</summary>
		public ConsumerStrategy	ConsumerStrategy { get;	set; }

		///	<summary>
		///	Note that if the "noAck" option	is enabled (which it is	by default), 
		///	then received deliveries are automatically acked within	the	
		///	server before they are even	transmitted	across the network to us. 
		///	</summary>
		public bool	NoAck {	get; set; }

		public bool ShouldSerializeExceptions { get; set; }
	}
}