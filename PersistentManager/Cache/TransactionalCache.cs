using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Services.Interfaces;
using PersistentManager.Descriptors;
using System.Runtime.Remoting.Messaging;

namespace PersistentManager.Cache
{
    internal class TransactionalCache : ITransactionalCache 
    {
        IDictionary<string , ICacheObject> TransactionalStore { get; set; }
        ISharedMemoryStore SharedCache { get; set; }

        public TransactionCacheBoundary TransactionCacheBoundary { get; set; }
        public Guid TransactionId { get; set; }

        internal TransactionalCache( ISharedMemoryStore SharedCache , TransactionCacheBoundary boundary )
        {
            this.TransactionalStore = new Dictionary<string , ICacheObject>( );
            this.TransactionCacheBoundary = boundary;
            this.TransactionId = Guid.NewGuid( );
            this.SharedCache = SharedCache;
        }

        public ICacheObject this[string key]
        {
            get
            {
                if ( TransactionalStore.ContainsKey( key ) )
                {
                    return TransactionalStore[key];
                }
                else
                {
                    lock ( ConfigurationBase.GlobalLock )
                    {
                        if ( SharedCache.ContainsKey( key ) )
                        {
                            Add( new CacheObject( key , CloneEntity( SharedCache[key].EntityInstance ) , TransactionId , true ) );

                            ICacheObject cacheObject = TransactionalStore[key];

                            if ( cacheObject.IsNull( ) )
                                return null;

                            //Flush out over staying entity
                            //Todo: If the cache size limit has not been reached. There is no point removing this entity
                            //Just Refresh it.
                            if ( ( DateTime.Now > cacheObject.Expiry ) )
                            {
                                SharedCache.Remove( cacheObject.Key );
                            }

                            return cacheObject;
                        }
                    }
                }

                return null;
            }
        }

        public void Add( ICacheObject candidate )
        {
            Add( candidate.Key , candidate );
        }

        public void Add( string key , ICacheObject candidate )
        {
            TransactionalStore.Add( key , candidate );
        }

        public void Remove( ICacheObject candidate )
        {
            Remove( candidate.Key );
        }

        public void Remove( string key )
        {
            if( TransactionalStore.ContainsKey( key ) )
            {
                TransactionalStore.Remove( key );
            }
        }

        public void MarkForDeletion( string key )
        {
            if ( TransactionalStore.ContainsKey( key ) )
            {
                this[key].MarkedForDeletion = true;
            }
        }

        public bool ContainsKey( string key )
        {
            return TransactionalStore.ContainsKey( key ) ||
                        SharedCache.ContainsKey( key );
        }

        public bool ContainsInTransaction( string key )
        {
            return TransactionalStore.ContainsKey( key );
        }

        public void Clear( )
        {
            TransactionalStore.Clear( );
        }

        public IEnumerator<ICacheObject> GetEnumerator( )
        {
            return TransactionalStore.Values.GetEnumerator( );
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            return TransactionalStore.Values.GetEnumerator( );
        }

        public ITransactionalCache Create( )
        {
            return this;
        }

        public void Eviction( TransactionState state )
        {
            //Check the object request count. and do house keeping job
            //By sending the object back for GC to collect.
            //Remove the object from the cache. And allow a new. Instance to be created. from the DB.
            //More research in this area. for house keeping.
            //Also check object timeouts

            long cachesize = ConfigurationBase.CacheSize;
            long numberToGarbageCollected = ( cachesize / 3 );        
            
            //long highestRequest = this.Max(c => c.RequestCount);

            lock( ConfigurationBase.GlobalLock )
            {
                if( SharedCache.Count >= cachesize )
                {
                    SharedCache.Clear( );
                }
                else
                {
                    List<ICacheObject> entitiesToCleanUp = SharedCache.Where( c => c.Expiry < DateTime.Now ).ToList( );

                    if( entitiesToCleanUp.Count >= numberToGarbageCollected )
                    {
                        foreach( ICacheObject cacheObject in entitiesToCleanUp )
                        {
                            SharedCache.Remove( cacheObject.Key );
                        }

                       // return;
                    }
                }

                if ( state == TransactionState.Committed )
                {
                    foreach ( ICacheObject cache in this.Where( c => c.MarkedForDeletion ) )
                    {
                        if ( SharedCache.ContainsKey( cache.Key ) )
                        {
                            SharedCache.Remove( cache.Key );
                        }
                    }
                }

                foreach( ICacheObject cache in this )
                {
                    if ( SharedCache.Count < cachesize && !cache.IsDirty )
                    {
                        if ( SharedCache.ContainsKey( cache.Key ) )
                        {
                            ICacheObject cacheObject = SharedCache[cache.Key];
                            cacheObject.CreatedTransactionId = TransactionId;
                            cacheObject.EntityInstance = CloneEntity( cache.EntityInstance );                      
                        }
                        else
                        {
                            SharedCache.Add( new CacheObject( cache.Key , CloneEntity( cache.EntityInstance ) , Guid.Empty , false )  );
                        }
                    }
                }

                foreach ( ICacheObject cache in this.Where( c => c.Expiry < DateTime.Now ).ToList( ) )
                {
                    Remove( cache );
                }
            }
        }

        public T CloneEntity<T>( T entity )
        {
            return (T) GhostGenerator.CopyState( entity );
        }

        #region IDiscoverable<ITransactionalCache> Members

        ITransactionalCache IDiscoverable<ITransactionalCache>.Create( )
        {
            return this;
        }

        #endregion

        public static ITransactionalCache CreateInstance( )
        {
            IConfiguration configuration = ConfigurationBase.GetCurrentConfiguration( );

            if ( configuration.TransactionCacheBoundary == TransactionCacheBoundary.CrossBoundary )
            {
                object current = ScopeContext.GetData<TransactionalCache>( Constant.RUNTIME_TRANSTRACTION );

                if ( current.IsNotNull() )
                {
                    return current as ITransactionalCache;
                }
                else
                {
                    ScopeContext.SetData( Constant.RUNTIME_TRANSTRACTION ,
                                            new TransactionalCache
                                            ( 
                                                ServiceLocator.Locate<ISharedMemoryStore>( ) ,
                                                configuration.TransactionCacheBoundary
                                            )
                                       );

                    return ScopeContext.GetData<ITransactionalCache>( Constant.RUNTIME_TRANSTRACTION );
                }
            }
            else if ( configuration.TransactionCacheBoundary == TransactionCacheBoundary.EntityManagerScope )
            {
                return new TransactionalCache( ServiceLocator.Locate<ISharedMemoryStore>( ) , configuration.TransactionCacheBoundary );
            }

            throw new PersistentException( "Could not resolve TransactionCacheScope, try setting it in IConfiguration.TransactionCacheScope" );
        }
    }
}
