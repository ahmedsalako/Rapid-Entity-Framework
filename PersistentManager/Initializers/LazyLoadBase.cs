using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Initializers.Interfaces;
using System.Reflection;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;
using PersistentManager.Cache;

namespace PersistentManager.Initializers
{
    public abstract class Lazy 
    {        
        public PropertyMetadata propertyMetadata;

        public virtual IList Orphans { get; set; }

        public virtual IDictionary<Guid , List<string>> LoadTrack { get; set; }

        public virtual int OrphanCount
        {
            get { return Orphans.Count; }
        }

        protected Lazy( Type lazyProperty  )
        {
            if ( lazyProperty.IsNotNull( ) )
            {
                if ( lazyProperty.IsGenericType && lazyProperty.IsCollection( ) )
                {
                    Orphans = ( IList ) ConcreteCollectionDiscovery.GenericCreate( typeof( List<> ) , lazyProperty.GetGenericArguments( )[0] );
                }
                else if ( lazyProperty.IsCollection( ) )
                {
                    Orphans = new ArrayList( );
                }
                else
                {
                    Orphans = new List<object>( );
                }
            }
            LoadTrack = new Dictionary<Guid , List<string>>( );
        }

        public virtual bool HasChanges
        {
            get 
            { 
                return Orphans.Count > 0 || LoadTrack.Count > 0; 
            }
        }

        public bool IsPersisted
        {
            get { return ( ( ILazyLoader )this ).OwnerKey.IsNotNull( ); }
        }

        public void ClearOrphans()
        {
            Orphans.Clear();
        }

        protected string[] RelationColumns
        {
            get
            {
                return propertyMetadata.RelationColumns;
            }
        }

        protected string[] JoinColumns
        {
            get
            {
                return propertyMetadata.JoinColumns;
            }
        }

        public virtual void Add( object child )
        {
            Orphans.Add( child );
        }

        public virtual void SaveAllChanges( )
        {
            if ( HasChanges )
            {
                Guid transactionId = SessionRuntime.Current.TransactionId;

                if (Orphans.IsNotNull())
                {
                    foreach (object entity in Orphans)
                    {
                        PersistChildObject(entity);
                    }
                }

                foreach (var store in LoadTrack.Where(k => k.Key == transactionId))
                {
                    foreach (string key in store.Value)
                    {
                        PersistChildObject(CacheService.GetInstance().Get(key).EntityInstance);
                    }
                }

                ClearOrphans();
                ClearLoadTrack();
            }
        }

        public bool OrphanExists( object entity )
        {
            return ( Orphans.Contains( entity ) ) ;
        }

        public virtual object GetOphanIndex( int index , int count )
        {
            if( ( index + 1 ) > count )
            {
                index = ( index + 1 ) - count;

                return Orphans[index - 1];
            }

            throw new Exception( "Could not locate entity at this index" );
        }

        public abstract void PersistChildObject( object child );

        public virtual object AddLoaded( object entity )
        {
            Guid transactionId = SessionRuntime.Current.TransactionId;
            Guid lastTransactionId = LoadTrack.FirstOrDefault( ).Key;

            if ( lastTransactionId != Guid.Empty || lastTransactionId.IsNotNull( ) )
            {
                LoadTrack.Remove( lastTransactionId );
            }
        
            string key = KeyGenerator.GetKey( entity );

            if ( LoadTrack.ContainsKey( ( transactionId ) ) )
            {
                LoadTrack[transactionId].Add( key );
            }
            else
            {
                LoadTrack.Add( transactionId , new List<string>( new[] { key } ) );
            }

            return entity;
        }

        public virtual void ClearLoadTrack( )
        {
            if ( LoadTrack.IsNull( ) ) return;
            LoadTrack.Clear( );
        }
    }
}
