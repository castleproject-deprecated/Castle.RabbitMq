namespace Castle.RabbitMq
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;

    [System.Diagnostics.DebuggerDisplay("Exchange {Name} Options {_options}", Name = "Exchange")]
    public class RabbitExchange : IRabbitExchange
    {
        private readonly IModel _model;
        private readonly bool _canDestroy;
        private readonly ExchangeOptions _options;

        public RabbitExchange(IModel model, string name, bool canDestroy, ExchangeOptions options)
        {
            this.Name = name;

            _model = model;
            _canDestroy = canDestroy;
            _options = options;
        }

        #region IRabbitExchange

        public string Name { get; private set; }

        #endregion

        #region IRabbitSender

        public MessageInfo Send(byte[] body, string routingKey = "", 
                                MessageProperties properties = null, 
                                SendOptions options = null)
        {
            throw new NotImplementedException();
        }

        public MessageInfo Send<T>(T message, string routingKey = "", 
                                   MessageProperties properties = null, 
                                   SendOptions options = null) 
            where T : class
        {
            throw new NotImplementedException();
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest request, string routingKey = "",
                                                          MessageProperties properties = null, 
                                                          SendOptions options = null) 
            where TRequest : class 
            where TResponse : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRabbitQueueDeclarer

        public IRabbitQueue DeclareQueue(string name, QueueOptions options)
        {
            options = options ?? QueueOptions.Default;

            lock (_model)
            {
                var result = _model.QueueDeclare(name, options.Durable, options.Exclusive, options.AutoDelete, options.Arguments);

                // binds it to "this" exchange
                _model.QueueBind(result.QueueName, this.Name, "");
                
                return new RabbitQueue(_model, this, result, options);
            }
        }

        public IRabbitQueue DeclareEphemeralQueue(QueueOptions options)
        {
            options = options ?? new QueueOptions();
            options.AutoDelete = true;
            options.Exclusive = true;

            lock (_model)
            {
                var result = _model.QueueDeclare();

                // binds it to "this" exchange
                _model.QueueBind(result.QueueName, this.Name, "");

                return new RabbitQueue(_model, this, result, options);
            }
        }

        #endregion

        #region IDestroyable

        public void Delete()
        {
            lock (_model)
            {
                _model.ExchangeDelete(this.Name);
            }
        }

        #endregion
    }
}