namespace Castle.RabbitMq.WindsorIntegration.Impl
{
    using System;
    using System.Transactions;
    using Core.Logging;
    using Messaging;
    using Services.Transaction;
    using Services.Transaction.Internal;
    using TransactionException = Services.Transaction.TransactionException;

    public class TransactionalMessageHandlerInvoker : DefaultMessageHandlerInvoker
    {
        private readonly ITransactionManager _txManager;

        public TransactionalMessageHandlerInvoker(ITransactionManager transactionManager)
        {
            this._txManager = transactionManager;
            this.Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override void Invoke(Type msgType, IMessage message, IMessageHandler handler)
        {
            SynchronizedCase(msgType, message, handler, _txManager.CreateTransaction().Value.Transaction);
        }

        private void SynchronizedCase(Type msgType, IMessage message, IMessageHandler handler, ITransaction transaction)
        {
            using (new TxScope(transaction.Inner, NullLogger.Instance))
            {
                var localIdentifier = transaction.LocalIdentifier;

                try
                {
                    base.Invoke(msgType, message, handler);

                    if (transaction.State == TransactionState.Active)
                        transaction.Complete();
                    else if (this.Logger.IsWarnEnabled)
                        this.Logger.WarnFormat(
                            "transaction was in state {0}, so it cannot be completed. the 'consumer' method, so to speak, might have rolled it back.",
                            transaction.State);
                }
                catch (TransactionAbortedException)
                {
                    // if we have aborted the transaction, we both warn and re-throw the exception
                    if (this.Logger.IsWarnEnabled)
                        this.Logger.WarnFormat("transaction aborted - synchronized case, tx#{0}", localIdentifier);

                    throw;
                }
                catch (TransactionException ex)
                {
                    if (this.Logger.IsFatalEnabled)
                        this.Logger.Fatal("internal error in transaction system - synchronized case", ex);

                    throw;
                }
                catch (AggregateException ex)
                {
                    if (this.Logger.IsWarnEnabled)
                        this.Logger.Warn("one or more dependent transactions failed, re-throwing exceptions!", ex);

                    throw;
                }
                catch (Exception)
                {
                    if (this.Logger.IsErrorEnabled)
                        this.Logger.ErrorFormat("caught exception, rolling back transaction - synchronized case - tx#{0}", localIdentifier);

                    throw;
                }
                finally
                {
                    if (this.Logger.IsDebugEnabled)
                        this.Logger.DebugFormat("dispoing transaction - synchronized case - tx#{0}", localIdentifier);

                    transaction.Dispose();
                }
            }
        }
    }
}