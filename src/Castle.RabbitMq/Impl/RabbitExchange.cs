namespace Castle.RabbitMq
{
    using RabbitMQ.Client;


    [System.Diagnostics.DebuggerDisplay("Exchange '{Name}' {_options}", Name = "Exchange")]
    public class RabbitExchange : IRabbitExchange
    {
        private readonly IModel _model;
        private readonly IRabbitSerializer _defaultSerializer;
        private readonly bool _canDestroy;
        private readonly ExchangeOptions _options;
        private readonly RpcHelper _rpcHelper;
        private readonly bool _isDefaultExchange;

        public RabbitExchange(IModel model, IRabbitSerializer serializer, 
                              string name, bool canDestroy, ExchangeOptions options)
        {
            this.Name = name;

            _model = model;
            _defaultSerializer = serializer;
            _canDestroy = canDestroy;
            _options = options;
            _isDefaultExchange = name == string.Empty;

            _rpcHelper = new RpcHelper(_model, this.Name, serializer);
        }


        #region IRabbitExchange

        public string Name { get; private set; }

        public IRabbitQueueBinding Bind(IRabbitQueue queue, string routingKeyOrFilter)
        {
            lock (_model)
                _model.QueueBind(queue.Name, this.Name, routingKeyOrFilter);

            return new RabbitQueueBinding(_model, queue.Name, this.Name, routingKeyOrFilter);
        }

        public void Delete()
        {
            lock (_model)
                _model.ExchangeDelete(this.Name);
        }

        public void Delete(bool ifUnused)
        {
            lock (_model)
                _model.ExchangeDelete(this.Name, ifUnused);
        }


        #endregion

        #region IRabbitSender

        public MessageInfo Send(byte[] body, string routingKey = "", 
                                MessageProperties properties = null, 
                                SendOptions options = null)
        {
            options = options ?? SendOptions.Default;
            var prop = properties ?? _model.CreateBasicProperties();
            if (options.Persist)
            {
                prop.DeliveryMode = 2; // persistent
            }

            lock (_model)
            {
                var id = _model.NextPublishSeqNo;
                _model.BasicPublish("", routingKey,
                                    mandatory: options.Mandatory,
                                    immediate: options.Immediate,
                                    basicProperties: properties,
                                    body: body);
                return new MessageInfo() { Tag = id };
            }
        }

        public MessageInfo Send<T>(T message, string routingKey = "", 
                                   MessageProperties properties = null, 
                                   SendOptions options = null) 
            where T : class
        {
            options = options ?? SendOptions.Default;
            var serializer = options.Serializer ?? _defaultSerializer;
            var data = serializer.Serialize(message);

            return Send(data, routingKey, properties, options);
        }

        public MessageEnvelope SendRequest(byte[] data, string routingKey = "",
                                           MessageProperties properties = null,
                                           RpcSendOptions options = null)
        {
            return _rpcHelper.SendRequest(data, 
                routingKey, properties, options);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request, string routingKey = "",
                                                          MessageProperties properties = null,
                                                          RpcSendOptions options = null) 
            where TRequest : class 
            where TResponse : class
        {
            return _rpcHelper.SendRequest<TRequest, TResponse>(request, 
                routingKey, properties, options);
        }

        #endregion

        #region IRabbitQueueDeclarer

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            options = options ?? QueueOptions.Default;

            var serializer = options.Serializer ?? _defaultSerializer;

            lock (_model)
            {
                var result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);

                if (!this._isDefaultExchange)
                {
                    // binds it to "this" exchange
                    _model.QueueBind(result.QueueName, this.Name, "");
                }

                return new RabbitQueue(_model, this, serializer, result, options);
            }
        }

        #endregion
    }
}