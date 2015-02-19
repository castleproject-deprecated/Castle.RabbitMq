namespace Castle.RabbitMq
{
    public interface IMessageAck
    {
        void Ack();
        void Reject();
    }

    public class MessageAck : IMessageAck
    {
        public MessageAck()
        {
        }

        public void Ack()
        {
        }
        public void Reject()
        {
        }
    }
}