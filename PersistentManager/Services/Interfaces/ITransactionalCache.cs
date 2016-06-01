using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Cache;

namespace PersistentManager.Services.Interfaces
{
    interface ITransactionalCache : IDiscoverable<ITransactionalCache> , IEnumerable<ICacheObject>
    {
        TransactionCacheBoundary TransactionCacheBoundary { get; set; }
        Guid TransactionId { get; set; }
        ICacheObject this[string key] { get; }

        void Add( ICacheObject candidate );
        void Add( string key , ICacheObject candidate );
        void Remove( ICacheObject candidate );
        void Remove( string key );
        bool ContainsKey( string key );
        bool ContainsInTransaction( string key );
        void MarkForDeletion( string key ); 
        void Eviction( TransactionState state );
        void Clear( );
        T CloneEntity<T>( T entity );
    }
}
