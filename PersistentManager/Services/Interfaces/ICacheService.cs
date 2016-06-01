using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Services.Interfaces;
using PersistentManager.Cache;

namespace PersistentManager
{
    internal interface ICacheService : IDiscoverable<ICacheService>
    {
        Guid TransactionId { get; }
        List<ICacheObject> GetUncommittedTransactionalEntities();
        List<ICacheObject> GetFlushedEntities();
        List<ICacheObject> GetDirtyEntities( );
        void Detach(ICacheObject candidate);
        void CollectGarbage( TransactionState state );
    }
}
