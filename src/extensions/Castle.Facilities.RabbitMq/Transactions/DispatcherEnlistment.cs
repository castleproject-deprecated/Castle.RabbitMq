namespace Castle.RabbitMq.Extensions.MessageHandler.Transactions
{
    using System;
    using System.Transactions;

    internal class DispatcherEnlistment : IEnlistmentNotification
    {
//        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DispatcherEnlistment));

        private readonly Action dispatcher;

        public DispatcherEnlistment(Action dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            try
            {
                dispatcher();
            }
            catch (Exception e)
            {
//                logger.Error("Error dispatcher message", e);

                throw;
            }
            finally
            {
                enlistment.Done();
            }
        }

        public void Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }
}