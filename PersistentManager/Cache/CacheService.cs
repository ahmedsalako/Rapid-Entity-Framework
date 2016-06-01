using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PersistentManager.Ghosting.Event;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Security.Policy;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Linq;
using PersistentManager.Descriptors;
using PersistentManager.Services.Interfaces;

namespace PersistentManager.Cache
{
    internal class CacheService : ICacheService , IEnumerable<ICacheObject>
    {
        public Guid TransactionId { get { return Storage.TransactionId; } }
        ITransactionalCache Storage { get; set; }

        internal CacheService( ITransactionalCache storage )
        {
            this.Storage = storage;
        }

        public ICacheService Create( )
        {
            return this;
        }

        List<ICacheObject> ICacheService.GetDirtyEntities()
        {
            return ( from instance in Storage
                     where instance.IsDirty == true
                     select instance ).ToList( );
        }

        List<ICacheObject> ICacheService.GetFlushedEntities()
        {
            return ( from instance in Storage
                     where instance.WasFlushed == true
                     select instance ).ToList( );
        }

        internal static CacheService GetInstance( )
        {
            return SessionRuntime.GetInstance( ).CacheService;
        }

        //We need to set this flag. For a newly created entity during an uncommited transaction
        //In case the transaction rollsback. Then we can detach this entity.
        internal static void SetCreatedUncommited( string key , Guid transactionId )
        {
            ICacheObject cacheObject = CacheService.GetInstance( ).Get( key );

            if ( cacheObject.IsNotNull( ) )
            {
                cacheObject.CreatedUncommited = true;
                cacheObject.CreatedTransactionId = transactionId; 
            }
        }

        internal static void SetCreatedUncommited( object entity , Guid transactionId )
        {
            SetCreatedUncommited( KeyGenerator.GetKey( entity ) , transactionId );
        }

        private ICacheObject GetCachedObject( string key )
        {
            return Storage[key];
        }

        internal object this[object key]
        {
            get
            {
                return GetCachedObject( key.As<string>( ) ).EntityInstance;
            }
        }

        internal bool IsDirty( string key )
        {
            if ( !Storage.ContainsKey( key ) )
                return false;

            return Storage[key].IsDirty;
        }

        internal bool WasFlushed( string key )
        {
            if ( !Storage.ContainsKey( key ) )
                return false;

            return Storage[key].WasFlushed;
        }

        internal void ResetDirtyState( string key )
        {
            if ( !Storage.ContainsKey( key ) )
                return;

            Storage[key].IsDirty = false;
        }

        internal ICacheObject Get( string key )
        {
            return GetCachedObject( key );
        }

        internal void Add( object objectInstance , bool isPropertyChanged )
        {
            string cacheKey = KeyGenerator.GetKey( objectInstance );
            ICacheObject cached = Add( objectInstance , cacheKey );

            if ( isPropertyChanged )
            {
                cached.AddPropertyChangeListener( );
            }
        }

        internal ICacheObject Add( object objectInstance , string cacheKey )
        {
            Type type = MetaDataManager.GetEntityType( objectInstance );

            return Add( cacheKey , type , objectInstance );
        }

        internal ICacheObject Add( string key , Type type , object objectInstance )
        {
            if ( !Storage.ContainsKey( key ) )
            {
                ICacheObject cacheObject = new CacheObject( key , type , objectInstance , TransactionId );
                cacheObject.IsDirty = false;

                Storage.Add( ( string )key , cacheObject );

                return cacheObject;
            }

            return GetCachedObject( key as string );
        }

        //Entities may span contexts, changes can be made out of context, and when this is done, it is required to bypass
        //entities stored on the shared cache and add the overlaping entity to the transaction cache.
        //If the transaction cache already contains a different copy of this overlapping entity, its best we merge changes
        //Overrides 
        internal void AddOverrideOrMerge( object entity , bool isDirty )
        {
            string key = GetKey( entity );

            if ( Storage.ContainsInTransaction( key ) )
            {
                if ( Storage.TransactionCacheBoundary == TransactionCacheBoundary.EntityManagerScope )
                {
                    Storage[key].EntityInstance = Storage.CloneEntity( entity );
                }

                Storage[key].IsDirty = isDirty;
            }
            else
            {
                ICacheObject cacheObject = new CacheObject( key , entity.GetType( ) , entity , TransactionId );
                cacheObject.IsDirty = isDirty;

                Storage.Add( ( string )key , cacheObject );
            }
        }

        internal bool ContainsInTransaction( object entity )
        {
            return Storage.ContainsInTransaction( GetKey( entity ) );
        }

        internal void MarkForDeletion( string key )
        {
            Storage.MarkForDeletion( key );
        }

        internal void Remove( string key )
        {
            if ( Storage.ContainsKey( key ) )
            {
                Storage.Remove( key );
            }
        }

        internal void FreeLockedObject( string key , object newInstance )
        {
            if ( !Contains( key ) )
            {
                Storage[key].IsDirty = false;
                Storage[key].EntityInstance = newInstance;
            }
        }

        internal bool IsInCache( object[] entityKeys , Type type )
        {
            string cacheKey = KeyGenerator.GetKey( entityKeys , type );

            return ( Contains( cacheKey ) );
        }

        public IEnumerator GetEnumerator( )
        {
            return Storage.GetEnumerator( );
        }

        internal bool Contains( string key )
        {
            return ( !GetCachedObject( key ).IsNull( ) );
        }

        internal bool Contains( object entity )
        {
            return Contains( GetKey( entity ) );
        }

        internal string GetKey( object entity )
        {
            return KeyGenerator.GetKey( entity );
        }

        void ICacheService.CollectGarbage( TransactionState state )
        {
            Storage.Eviction( state );
        }

        IEnumerator<ICacheObject> IEnumerable<ICacheObject>.GetEnumerator( )
        {
            return Storage.GetEnumerator( );
        }

        void ICacheService.Detach( ICacheObject cacheObject)
        {
            Storage.Remove( cacheObject.Key as string );
            cacheObject.RemovePropertyChangeListener( );
        }

        List<ICacheObject> ICacheService.GetUncommittedTransactionalEntities()
        {
            return (from entity in Storage
                    where entity.CreatedUncommited && entity.CreatedTransactionId == TransactionId
                    select entity ).ToList();
        }
    }
}
