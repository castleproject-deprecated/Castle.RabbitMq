namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;

    public class MessageProperties : IBasicProperties
    {
        private int? _protocolClassId;
        private string _protocolClassName;
        private string _appId;
        private string _clusterId;
        private string _contentEncoding;
        private string _contentType;
        private string _correlationId;
        private byte? _deliveryMode;
        private string _expiration;
        private IDictionary<string, object> _headers;
        private string _messageId;
        private byte? _priority;
        private string _replyTo;
        private PublicationAddress _replyToAddress;
        private AmqpTimestamp? _timestamp;
        private string _type;
        private string _userId;

        public int ProtocolClassId
        {
            get { return _protocolClassId.Value; }
            set { _protocolClassId = value; }
        }

        public string ProtocolClassName
        {
            get { return _protocolClassName; }
            set { _protocolClassName = value; }
        }

        public bool IsAppIdPresent()
        {
            return _appId != null;
        }

        public bool IsClusterIdPresent()
        {
            return _clusterId != null;
        }

        public bool IsContentEncodingPresent()
        {
            return _contentType != null;
        }

        public bool IsContentTypePresent()
        {
            return _contentType != null;
        }

        public bool IsCorrelationIdPresent()
        {
            return _correlationId != null;
        }

        public bool IsDeliveryModePresent()
        {
            return _deliveryMode != 0;
        }

        public bool IsExpirationPresent()
        {
            return _expiration != null;
        }

        public bool IsHeadersPresent()
        {
            return _headers != null;
        }

        public bool IsMessageIdPresent()
        {
            return _messageId != null;
        }

        public bool IsPriorityPresent()
        {
            return _priority.HasValue;
        }

        public bool IsReplyToPresent()
        {
            return _replyTo != null;
        }

        public bool IsTimestampPresent()
        {
            return _timestamp.HasValue;
        }

        public bool IsTypePresent()
        {
            return _type != null;
        }

        public bool IsUserIdPresent()
        {
            return _userId != null;
        }

        public void SetPersistent(bool persistent)
        {
            _deliveryMode = (byte) (persistent ? 2 : 1);
        }

        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        public string ClusterId
        {
            get { return _clusterId; }
            set { _clusterId = value; }
        }

        public string ContentEncoding
        {
            get { return _contentEncoding; }
            set { _contentEncoding = value; }
        }

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public string CorrelationId
        {
            get { return _correlationId; }
            set { _correlationId = value; }
        }

        public byte DeliveryMode
        {
            get { return _deliveryMode.Value; }
            set { _deliveryMode = value; }
        }

        public string Expiration
        {
            get { return _expiration; }
            set { _expiration = value; }
        }

        public IDictionary<string, object> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        public string MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        public byte Priority
        {
            get { return _priority.Value; }
            set { _priority = value; }
        }

        public string ReplyTo
        {
            get { return _replyTo; }
            set { _replyTo = value; }
        }

        public PublicationAddress ReplyToAddress
        {
            get { return _replyToAddress; }
            set { _replyToAddress = value; }
        }

        public AmqpTimestamp Timestamp
        {
            get { return _timestamp.Value; }
            set { _timestamp = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        #region Clears

        public void ClearAppId()
        {
            _appId = null;
        }

        public void ClearClusterId()
        {
            _clusterId = null;
        }

        public void ClearContentEncoding()
        {
            _contentEncoding = null;
        }

        public void ClearContentType()
        {
            _contentType = null;
        }

        public void ClearCorrelationId()
        {
            _correlationId = null;
        }

        public void ClearDeliveryMode()
        {
            _deliveryMode = null;
        }

        public void ClearExpiration()
        {
            _expiration = null;
        }

        public void ClearHeaders()
        {
            _headers = null;
        }

        public void ClearMessageId()
        {
            _messageId = null;
        }

        public void ClearPriority()
        {
            _priority = null;
        }

        public void ClearReplyTo()
        {
            _replyTo = null;
        }

        public void ClearTimestamp()
        {
            _timestamp = null;
        }

        public void ClearType()
        {
            _type = null;
        }

        public void ClearUserId()
        {
            _userId = null;
        }

        #endregion

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}