namespace Castle.RabbitMq
{
	using System;
	using System.Collections.Generic;
	using RabbitMQ.Client;

	public class MessageProperties : IBasicProperties
	{
		private	string _protocolClassName;
		private	string _appId;
		private	string _clusterId;
		private	string _contentEncoding;
		private	string _contentType;
		private	string _correlationId;
		private	string _expiration;
		private	string _messageId;
		private	string _replyTo;
		private	string _type;
		private	string _userId;
		private	int? _protocolClassId;
		private	byte? _priority;
		private	byte? _deliveryMode;
		private	PublicationAddress _replyToAddress;
		private	AmqpTimestamp? _timestamp;
		private	IDictionary<string,	object>	_headers;

		public MessageProperties()
		{
			_headers = new Dictionary<string, object>(StringComparer.Ordinal);
		}

		internal void CopyTo(IBasicProperties properties)
		{
			if (this.IsAppIdPresent())
				properties.AppId = this.AppId;

			if (this.IsClusterIdPresent())
				properties.ClusterId = this.ClusterId;
			
			if (this.IsContentEncodingPresent())
				properties.ContentEncoding = this.ContentEncoding;

			if (this.IsContentTypePresent())
				properties.ContentType = this.ContentType;

			if (this.IsCorrelationIdPresent())
				properties.CorrelationId = this.CorrelationId;

			if (this.IsExpirationPresent())
				properties.Expiration =	this.Expiration;

			if (this.IsMessageIdPresent())
				properties.MessageId = this.MessageId;

			if (this.IsTypePresent())
				properties.Type	= this.Type;

			if (this.IsUserIdPresent())
				properties.UserId =	this.UserId;

			if (this.IsPriorityPresent())
				properties.Priority	= this.Priority;

			if (this.IsDeliveryModePresent())
				properties.DeliveryMode	= this.DeliveryMode;

			if (this.IsReplyToPresent())
			{
				properties.ReplyTo = this.ReplyTo;
				properties.ReplyToAddress =	this.ReplyToAddress;
			}
			
			if (this.IsTimestampPresent())
				properties.Timestamp = this.Timestamp;

			if (this.IsHeadersPresent())
				properties.Headers = this.Headers;
		}

		#region	Properties

		public int ProtocolClassId
		{
			get	{ return _protocolClassId.Value; }
			set	{ _protocolClassId = value;	}
		}

		public string ProtocolClassName
		{
			get	{ return _protocolClassName; }
			set	{ _protocolClassName = value; }
		}

		public void	SetPersistent(bool persistent)
		{
			_deliveryMode =	(byte) (persistent ? 2 : 1);
		}

		public string AppId
		{
			get	{ return _appId; }
			set	{ _appId = value; }
		}

		public string ClusterId
		{
			get	{ return _clusterId; }
			set	{ _clusterId = value; }
		}

		public string ContentEncoding
		{
			get	{ return _contentEncoding; }
			set	{ _contentEncoding = value;	}
		}

		public string ContentType
		{
			get	{ return _contentType; }
			set	{ _contentType = value;	}
		}

		public string CorrelationId
		{
			get	{ return _correlationId; }
			set	{ _correlationId = value; }
		}

		public byte	DeliveryMode
		{
			get	{ return _deliveryMode.Value; }
			set	{ _deliveryMode	= value; }
		}

		public string Expiration
		{
			get	{ return _expiration; }
			set	{ _expiration =	value; }
		}

		public IDictionary<string, object> Headers
		{
			get	{ return _headers; }
			set	{ _headers = value;	}
		}

		public string MessageId
		{
			get	{ return _messageId; }
			set	{ _messageId = value; }
		}

		public byte	Priority
		{
			get	{ return _priority.Value; }
			set	{ _priority	= value; }
		}

		public string ReplyTo
		{
			get	{ return _replyTo; }
			set	{ _replyTo = value;	}
		}

		public PublicationAddress ReplyToAddress
		{
			get	{ return _replyToAddress; }
			set	{ _replyToAddress =	value; }
		}

		public AmqpTimestamp Timestamp
		{
			get	{ return _timestamp.Value; }
			set	{ _timestamp = value; }
		}

		public string Type
		{
			get	{ return _type;	}
			set	{ _type	= value; }
		}

		public string UserId
		{
			get	{ return _userId; }
			set	{ _userId =	value; }
		}

		#endregion

		#region	Clears

		public void	ClearAppId()
		{
			_appId = null;
		}

		public void	ClearClusterId()
		{
			_clusterId = null;
		}

		public void	ClearContentEncoding()
		{
			_contentEncoding = null;
		}

		public void	ClearContentType()
		{
			_contentType = null;
		}

		public void	ClearCorrelationId()
		{
			_correlationId = null;
		}

		public void	ClearDeliveryMode()
		{
			_deliveryMode =	null;
		}

		public void	ClearExpiration()
		{
			_expiration	= null;
		}

		public void	ClearHeaders()
		{
			_headers.Clear();
		}

		public void	ClearMessageId()
		{
			_messageId = null;
		}

		public void	ClearPriority()
		{
			_priority =	null;
		}

		public void	ClearReplyTo()
		{
			_replyTo = null;
		}

		public void	ClearTimestamp()
		{
			_timestamp = null;
		}

		public void	ClearType()
		{
			_type =	null;
		}

		public void	ClearUserId()
		{
			_userId	= null;
		}

		#endregion

		#region	Is X Present

		public bool	IsAppIdPresent()
		{
			return _appId != null;
		}

		public bool	IsClusterIdPresent()
		{
			return _clusterId != null;
		}

		public bool	IsContentEncodingPresent()
		{
			return _contentType	!= null;
		}

		public bool	IsContentTypePresent()
		{
			return _contentType	!= null;
		}

		public bool	IsCorrelationIdPresent()
		{
			return _correlationId != null;
		}

		public bool	IsDeliveryModePresent()
		{
			return _deliveryMode.HasValue;
		}

		public bool	IsExpirationPresent()
		{
			return _expiration != null;
		}

		public bool	IsHeadersPresent()
		{
			return _headers.Count != 0;
		}

		public bool	IsMessageIdPresent()
		{
			return _messageId != null;
		}

		public bool	IsPriorityPresent()
		{
			return _priority.HasValue;
		}

		public bool	IsReplyToPresent()
		{
			return _replyTo	!= null	|| _replyToAddress != null;
		}

		public bool	IsTimestampPresent()
		{
			return _timestamp.HasValue;
		}

		public bool	IsTypePresent()
		{
			return _type !=	null;
		}

		public bool	IsUserIdPresent()
		{
			return _userId != null;
		}

		#endregion

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}