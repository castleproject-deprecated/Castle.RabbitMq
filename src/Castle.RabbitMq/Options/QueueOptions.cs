namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;

	///	<summary>
	///	Used to	specify	queue options, including the <see cref="IRabbitSerializer"/>
	///	</summary>
	public class QueueOptions
	{
		internal static	QueueOptions Default = new QueueOptions()
		{
			Durable	= false,
			Exclusive =	false,
			AutoDelete = false
		};

		public QueueOptions()
		{
			this.Durable = false;
			this.Exclusive = false;
			this.AutoDelete	= false;
		}

		///	<summary>
		///	Default	serializer to use. Can be overriden	in a per-operation basis
		///	</summary>
		public IRabbitSerializer Serializer	{ get; set;	}

		/// <summary>
		/// If set when creating a new queue, the queue will be marked as durable. 
		/// Durable queues remain active when a server restarts. Non-durable queues 
		/// (transient queues) are purged if/when a server restarts. Note that durable 
		/// queues do not necessarily hold persistent messages, although it does not 
		/// make sense to send persistent messages to a transient queue.
		/// </summary>
		public bool	Durable	{ get; set;	}

		/// <summary>
		/// Exclusive queues may only be accessed by the current connection, and are deleted 
		/// when that connection closes. Passive declaration of an exclusive queue by 
		/// other connections are not allowed.
		/// </summary>
		public bool	Exclusive {	get; set; }

		/// <summary>
		/// If set, the queue is deleted when all consumers have finished using it. The last consumer 
		/// can be cancelled either explicitly or because its channel is closed. If there was no consumer
		/// ever on the queue, it won't be deleted. Applications can explicitly delete auto-delete queues 
		/// using the Delete method as normal.
		/// </summary>
		public bool	AutoDelete { get; set; }


		public IDictionary<string, object> Arguments { get;	set; }

		public override	string ToString()
		{
			return String.Format("Durable: {0} Exclusive: {1} AutoDelete: {2}",	Durable, Exclusive,	AutoDelete);
		}
	}
}