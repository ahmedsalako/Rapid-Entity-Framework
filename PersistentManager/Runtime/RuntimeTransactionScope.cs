using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Cache;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using PersistentManager.Contracts;

namespace PersistentManager.Runtime
{
    public class RuntimeTransactionScope
    {
        internal Guid TransactionId { get { return CacheService.TransactionId; } }
        internal CacheService CacheService { get; set; }

        internal static bool IsInScope
        {
            get { return ConfigurationBase.GetOrCreateScopes( ).Count > 0; }
        }
        
        internal static RuntimeTransactionScope Current
        {
            get
            {
                return SessionRuntime.GetInstance( );
            }
        }

        internal DirtyTrail CreateAudit( QueryType queryType , Type type , object entity , bool ignoreCache )
        {
            string key = KeyGenerator.GetKey( entity , type );

            if ( !ignoreCache && !type.IsAbstract )
            {
                CacheService.AddOverrideOrMerge( entity , true );
            }

            return new DirtyTrail( queryType , type , entity , key );
        }

        internal DirtyTrail SetAllPropertiesForCreate( object entity , Type type )
        {
            return CreateAudit( QueryType.Insert , type , entity , true );
        }

        internal DirtyTrail SetAllSelfTrackedPropertiesForUpdate( Type type , object entity )
        {
            List<IChangeCatalog> changes = new List<IChangeCatalog>( GetChanges( entity ).ToList( ) );

            foreach ( PropertyMetadata property in EntityMetadata.GetMappingInfo( type ).GetAll( ).Where( p => p.IsEntitySplitJoin ) )
            {
                object relation = MetaDataManager.GetPropertyValue( property.ClassDefinationName , entity );
                changes.AddRange( GetChanges( relation ) );
            }

            return SetAllSelfTrackedPropertiesForUpdate( type , entity , changes );
        }

        internal DirtyTrail SetAllSelfTrackedPropertiesForUpdate( Type type , object entity , IList<IChangeCatalog> changes )
        {
            if ( IsSefTracking( entity ) )
            {
                DirtyTrail dirty = CreateAudit( QueryType.Update , type , entity , false );
                EntityMetadata metadata = EntityMetadata.GetMappingInfo( type );

                foreach (IChangeCatalog change in changes.Where( t => t.EntityType == type ) )
                {
                    PropertyMetadata property = metadata.PropertyMapping(change.PropertyName);

                    if ( change.OriginalValue.IsNull( ) && change.NewValue.IsNotNull( ) )
                    {
                        dirty.AddAudit( change.PropertyName , property.MappingName ,  change.NewValue );
                    }
                    else if ( !change.OriginalValue.Equals( change.NewValue ) )
                    {
                        dirty.AddAudit( change.PropertyName , property.MappingName , change.NewValue );
                    }
                }

                CacheService.Get( CacheService.GetKey( entity ) ).IsDirty = true;

                return dirty;
            }

            return null;
        }

        internal void RemoveCacheableReference( object entity )
        {
            CacheService.MarkForDeletion( KeyGenerator.GetKey( entity ) );
        }

        internal bool IsSefTracking( object entity )
        {
            return GhostGenerator.IsSelfTracking( entity );
        }

        public void RestoreChanges( object entity , bool checkTransactionId )
        {            
            if ( IsSefTracking( entity ) )
            {
                GhostGenerator.SetLoadState( ref entity , false );

                foreach ( IChangeCatalog change in GetChanges( entity ) )
                {
                    if ( checkTransactionId )
                    {
                        if ( change.TransactionId != TransactionId ) continue;
                    }

                    if ( change.OriginalValue != change.NewValue )
                    {
                        MetaDataManager.SetPropertyValue( change.PropertyName , entity , change.OriginalValue );
                    }
                }

                GhostGenerator.ClearSelfChanges( entity );

                GhostGenerator.SetLoadState( ref entity , true );
            }            
        }

        internal ICollection<IChangeCatalog> GetChanges( object entity )
        {
            return GhostGenerator.GetSelfChanges( entity ).Values;
        }

        internal IEnumerable<T> UndoTransactionChanges<T>( IList<T> entities )
        {
            foreach ( T entity in entities )
                yield return ( T )UndoTransactionChanges( ( object )entity );
        }

        internal object UndoTransactionChanges( object entity )
        {
            RestoreChanges( entity , true );
            return entity;
        }

        internal object UndoAllChanges( object entity )
        {
            RestoreChanges( entity , false );
            return entity;
        }

        internal IEnumerable<T> UndoAllChanges<T>( IList<T> entities )
        {
            foreach ( T entity in entities )
                yield return (T) UndoAllChanges( (object) entity );
        }

        internal void SetDirtyState( object entity , bool value )
        {            
            if( CacheService.ContainsInTransaction( entity ) )
            {
                string key = CacheService.GetKey( entity );
                CacheService.Get( key ).IsDirty = value;
            }
        }
    }
}
