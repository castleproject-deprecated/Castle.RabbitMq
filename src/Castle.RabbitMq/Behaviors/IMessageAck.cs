namespace Castle.RabbitMq
{
    using System;

    public interface IMessageAck
    {
        void Ack();
        void Reject(bool requeue);
    }

    public class MessageAck : IMessageAck
    {
        private readonly Action _ack;
        private readonly Action<bool> _nack;

        public MessageAck(Action ack, Action<bool> nack)
        {
            _ack = ack;
            _nack = nack;
        }

        public void Ack()
        {
            _ack();
        }
        public void Reject(bool requeue)
        {
            _nack(requeue);
        }
    }
}