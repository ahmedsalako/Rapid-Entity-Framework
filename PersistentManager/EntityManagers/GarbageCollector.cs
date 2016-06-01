using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Cache;
using System.Collections;
using PersistentManager.Runtime;
using PersistentManager.Descriptors;

namespace PersistentManager
{
    internal class GarbageCollector
    {
        RuntimeTransactionScope transactionScope;
        ICacheService cacheService;

        internal GarbageCollector( ICacheService cacheService , RuntimeTransactionScope transactionScope )
        {
            this.cacheService = cacheService;
        }

        internal void RaisePostCommitOperation( )
        {
            foreach ( ICacheObject candidate in cacheService.GetFlushedEntities( ) )
            {
                candidate.CleanseDirtyState( ); 
            }

            PromoteCommitedEntities( );
        }

        internal void RaisePostRollBackOperation( )
        {
            foreach( ICacheObject cacheObject in cacheService.GetDirtyEntities( ) )
            {
                transactionScope.UndoTransactionChanges( cacheObject.EntityInstance );          
            }

            DetachUncommitedEntities( );
        }

        internal void PromoteCommitedEntities( )
        {
            foreach ( ICacheObject cacheObject in cacheService.GetUncommittedTransactionalEntities() )
            {
                cacheObject.CreatedUncommited = false;
                cacheObject.ClearSelfChanges( );
            }

            cacheService.CollectGarbage( TransactionState.Committed );
        }

        internal void DetachUncommitedEntities( )
        {
            foreach ( ICacheObject cacheObject in cacheService.GetUncommittedTransactionalEntities( ) )
            {
                cacheService.Detach( cacheObject );
                cacheObject.CreatedTransactionId = Guid.Empty;
                cacheObject.CreatedUncommited = false;
            }

            cacheService.CollectGarbage( TransactionState.RolledBack );
        }
    }
}
