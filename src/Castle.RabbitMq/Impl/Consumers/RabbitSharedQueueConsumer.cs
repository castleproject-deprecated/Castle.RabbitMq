namespace Castle.RabbitMq
{
    using System;
    using System.Threading;
    using RabbitMQ.Client;

    class RabbitSharedQueueConsumer : QueueingBasicConsumer
    {
        private readonly Thread _thread;

        public RabbitSharedQueueConsumer(IModel model, IRabbitSerializer serializer)
            : base(model)
        {
            _thread = new Thread(OnProc)
            {
                IsBackground = true
            };
        }

        private void OnProc()
        {
            try
            {
                var args = base.Queue.Dequeue();

            }
            catch (Exception e)
            {
                
            }
        }

        public override void OnCancel()
        {

            base.OnCancel();
        }
    }
}