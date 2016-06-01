using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Services.Interfaces;
using System.Collections;

namespace PersistentManager.Cache
{
    internal class InternalSharedMemory : ISharedMemoryStore
    {
        static private IDictionary<string, ICacheObject> store;

        internal InternalSharedMemory( )
        {
            store = new Dictionary<string , ICacheObject>( );
        }

        public ISharedMemoryStore Create( )
        {
            return this;
        }

        public void Add( string key , ICacheObject candidate )
        {
            store.Add( key , candidate );
        }

        public bool ContainsKey( string key )
        {
            return store.ContainsKey( key );
        }

        public void Remove( string key )
        {
            store.Remove( key );
        }

        public int Count { get { return store.Count; } }

        public void Clear( )
        {
            store.Clear( );
        }

        #region IEnumerable<ICacheObject> Members

        public IEnumerator<ICacheObject> GetEnumerator( )
        {
            return store.Values.GetEnumerator( );
        }

        #endregion

        #region IEnumerable Members

        IEnumerator System.Collections.IEnumerable.GetEnumerator( )
        {
            return store.Values.GetEnumerator( );
        }

        #endregion

        #region ISharedMemoryStore Members

        public ICacheObject this[string key]
        {
            get { return store[key]; }
        }

        #endregion

        #region ISharedMemoryStore Members


        public void Eviction( )
        {
            
        }

        #endregion        
    
        public void Add( ICacheObject candidate )
        {
            Add( candidate.Key , candidate );
        }

        public void Remove( ICacheObject candidate )
        {
            Remove( candidate.Key );
        }
    }
}
