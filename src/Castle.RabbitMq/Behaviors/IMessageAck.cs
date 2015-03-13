namespace Castle.RabbitMq
{
    using System;

    public interface IMessageAck
    {
        void Ack();
        void Reject();
    }

    public class MessageAck : IMessageAck
    {
        private readonly Action _ack;
        private readonly Action _nack;

        public MessageAck(Action ack, Action nack)
        {
            _ack = ack;
            _nack = nack;
        }

        public void Ack()
        {
            _ack();
        }
        public void Reject()
        {
            _nack();
        }
    }
}