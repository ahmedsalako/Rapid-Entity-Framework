using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Cache;

namespace PersistentManager
{
    public interface ISharedMemoryStore : IEnumerable<ICacheObject> , IDiscoverable<ISharedMemoryStore>
    {
        ICacheObject this[string key] { get; }

        void Add( ICacheObject candidate );
        void Add( string key , ICacheObject candidate );
        void Remove( ICacheObject candidate );
        void Remove( string key );
        bool ContainsKey( string key );
        void Eviction( );
        int Count { get; }
        void Clear( );
    }
}
