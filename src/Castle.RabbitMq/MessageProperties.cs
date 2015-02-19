namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;

    public class MessageProperties : IBasicProperties
    {
        private int _protocolClassId;
        private string _protocolClassName;
        private string _appId;
        private string _clusterId;
        private string _contentEncoding;
        private string _contentType;
        private string _correlationId;
        private byte _deliveryMode;
        private string _expiration;
        private IDictionary<string, object> _headers;
        private string _messageId;
        private byte _priority;
        private string _replyTo;
        private PublicationAddress _replyToAddress;
        private AmqpTimestamp _timestamp;
        private string _type;
        private string _userId;

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int ProtocolClassId
        {
            get { return _protocolClassId; }
        }

        public string ProtocolClassName
        {
            get { return _protocolClassName; }
        }

        public void ClearAppId()
        {
            throw new NotImplementedException();
        }

        public void ClearClusterId()
        {
            throw new NotImplementedException();
        }

        public void ClearContentEncoding()
        {
            throw new NotImplementedException();
        }

        public void ClearContentType()
        {
            throw new NotImplementedException();
        }

        public void ClearCorrelationId()
        {
            throw new NotImplementedException();
        }

        public void ClearDeliveryMode()
        {
            throw new NotImplementedException();
        }

        public void ClearExpiration()
        {
            throw new NotImplementedException();
        }

        public void ClearHeaders()
        {
            throw new NotImplementedException();
        }

        public void ClearMessageId()
        {
            throw new NotImplementedException();
        }

        public void ClearPriority()
        {
            throw new NotImplementedException();
        }

        public void ClearReplyTo()
        {
            throw new NotImplementedException();
        }

        public void ClearTimestamp()
        {
            throw new NotImplementedException();
        }

        public void ClearType()
        {
            throw new NotImplementedException();
        }

        public void ClearUserId()
        {
            throw new NotImplementedException();
        }

        public bool IsAppIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsClusterIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsContentEncodingPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsContentTypePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsCorrelationIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsDeliveryModePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsExpirationPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsHeadersPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsMessageIdPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsPriorityPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsReplyToPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsTimestampPresent()
        {
            throw new NotImplementedException();
        }

        public bool IsTypePresent()
        {
            throw new NotImplementedException();
        }

        public bool IsUserIdPresent()
        {
            throw new NotImplementedException();
        }

        public void SetPersistent(bool persistent)
        {
            throw new NotImplementedException();
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
            get { return _deliveryMode; }
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
            get { return _priority; }
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
            get { return _timestamp; }
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
    }
}