namespace Castle.RabbitMq
{

    public interface IMessageAction
    {
        void Ack();
        void Reject();
    }

    public class MessageAction : IMessageAction
    {
        public MessageAction()
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