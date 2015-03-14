namespace Castle.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using RabbitMQ.Client;

    class RabbitSharedQueueConsumer<T> : QueueingBasicConsumer // , IMessageProducer<T>
    {
        private readonly ConcurrentBag<Action<MessageEnvelope<T>>> _observers = new ConcurrentBag<Action<MessageEnvelope<T>>>();
        private readonly Thread _thread;
        private volatile bool _closed;

        public RabbitSharedQueueConsumer(IModel model, IRabbitSerializer serializer)
            : base(model)
        {
            _thread = new Thread(OnProc)
            {
                IsBackground = true
            };
            _thread.Start();
        }

//        public void Subscribe(Action<MessageEnvelope<T>> observer)
//        {
//            _observers.Add(observer);
//        }

        private void OnProc()
        {
            try
            {
                while (!_closed)
                {
                    var args = base.Queue.Dequeue();
                    if (args != null)
                    {
                        if (_observers.Count == 0) continue;

//                        var envelope = new MessageEnvelope<T>(args.BasicProperties,  args.Body) 
//
//                        foreach (var observer in _observers)
//                        {
//                            observer()
//                        }
                    }    
                }
            }
            catch (Exception e)
            {
            }
        }

        public override void OnCancel()
        {
            _closed = true;
            base.OnCancel();
        }
    }
}