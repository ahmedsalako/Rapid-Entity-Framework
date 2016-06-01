using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PersistentManager.Cache;
using PersistentManager.Exceptions;

namespace PersistentManager
{
    internal class Transaction
    {
        private GarbageCollector collector;
        private SessionRuntime runtime;
        private IDbTransaction transaction;

        internal Transaction( IDbTransaction transaction , SessionRuntime runtime )
        {
            this.transaction = transaction;
            this.runtime = runtime;

            collector = new GarbageCollector( runtime.CacheService , runtime );
        }

        public void Commit()
        {
            try
            {
                runtime.CloseActiveReaders();
                transaction.Commit();

                collector.RaisePostCommitOperation();
            }
            catch (Exception ex)
            {
                RollBack();
                throw new Exception("Could not commit transaction", ex);
            }
        }

        public void RollBack()
        {
            try
            {
                runtime.CloseActiveReaders();
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                Throwable.ThrowException("Could not rollback current transaction", ex);
            }
            finally
            {
                collector.RaisePostRollBackOperation();
            }
        }

        internal virtual void OnTransactionChangingHandler( IDbTransaction transaction )
        {
           this.transaction = transaction;
           runtime.DataBaseProvider.Transaction = transaction;
        }
    }
}
