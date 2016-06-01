using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Initializers.Interfaces;
using System.Collections;
using PersistentManager.Mapping;
using PersistentManager.Cache;
using PersistentManager.Util;
using PersistentManager.Exceptions.EntityManagerException;
using PersistentManager.Query.Keywords;
using PersistentManager.Metadata;
using PersistentManager.Runtime;
using PersistentManager.Initializers;
using PersistentManager.Collections;

namespace PersistentManager.Query.Sql
{
    internal class UpdateExpression : QueryExpression
    {
        internal UpdateExpression( QueryContext context )
        {
            QueryContext = context;
        }

        protected override QueryContext PrepareQuery( )
        {
            if ( QueryContext.QueryType == QueryType.Update )
            {
                return PrepareUpdateQuery( );
            }
            else if ( QueryContext.QueryType == QueryType.Insert )
            {
                return PrepareCreateQuery( );
            }
            else if ( QueryContext.QueryType == QueryType.Delete )
            {
                return PrepareDeleteQuery( );
            }

            return null;
        }

        private QueryContext PrepareDeleteQuery( )
        {
            return new Delete( )
                        .From( QueryContext.EntityType )
                        .Where( QueryContext.Names , QueryContext.Values , QueryContext )
                        .GetSyntax( )
                        .ExecuteUpdateInternal( );
        }

        private QueryContext PrepareUpdateQuery( )
        {
            CacheService cache = CacheService.GetInstance( );
            string cacheKey = KeyGenerator.GetKey( QueryContext.EntityInstance );

            if ( cache.IsDirty( cacheKey ) )
            {
                QueryContext = new Update( ).Into( QueryContext.EntityType )
                                    .ParameterValues( QueryContext.EntityInstance , QueryContext )
                                    .GetSyntax( )
                                    .ExecuteUpdateInternal( );

                cache.Get( cacheKey ).WasFlushed = true;


                return QueryContext;
            }

            QueryContext.IsUpdated = true;

            return QueryContext;
        }

        private QueryContext PrepareCreateQuery( )
        {
            string AutoKeyName = QueryContext.MetaStructure.GetAutoGenKeyName( );
            object dbgeneratedKey = new object( );

            if ( !AutoKeyName.IsNullOrEmpty( ) )
            {
                QueryContext = new Insert( ).Into( QueryContext.EntityType )
                                            .ParameterValues( QueryContext.EntityInstance , QueryContext )
                                            .GetSyntax( )
                                            .ExecuteCreateStatementWithReturnKey( out dbgeneratedKey , AutoKeyName );

                MetaDataManager.SetUniqueKey( QueryContext.EntityType , AutoKeyName , dbgeneratedKey , QueryContext.EntityInstance );
            }
            else
            {
                QueryContext = new Insert( ).Into( QueryContext.EntityType )
                                            .ParameterValues( QueryContext.EntityInstance , QueryContext )
                                            .GetSyntax( )
                                            .ExecuteUpdateInternal( );
            }

            IDictionary<string , object> keys = MetaDataManager.GetUniqueKeys( QueryContext.EntityType , QueryContext.EntityInstance );

            QueryContext.Names = keys.Keys.ToArray( );
            QueryContext.Values = keys.Values.ToArray( );

            return QueryContext;
        }

        internal bool ExecuteUpdateNoCascade( QueryContext context )
        {
            return Interpret( context ).IsUpdated;
        }

        internal bool ExecuteUpdate( QueryContext context )
        {
            if ( context.QueryType == QueryType.Insert || context.QueryType == QueryType.Update )
            {
                UpdateInheritanceHierachy( context );
            }

            if ( context.QueryType == QueryType.Delete )
            {
                UpdateEmbeddedEntities( context );
            }

            IDictionary<string , ILazyLoader> lazyHandlers = null;

            if ( context.QueryType == QueryType.Delete )
            {
                lazyHandlers = GetChildrenHandle( context.EntityInstance );
                CascadeRemove( context.EntityInstance , context.MetaStructure , lazyHandlers );
            }

            bool result = Interpret( context ).IsUpdated;

            if ( context.QueryType == QueryType.Insert || context.QueryType == QueryType.Update )
            {
                UpdateEmbeddedEntities( context );
            }

            if ( result )
            {
                switch ( context.QueryType )
                {
                    case QueryType.Update:
                        CascadeUpdate( context.EntityInstance , context.MetaStructure , GetChildrenHandle( context.EntityInstance ) );
                        break;
                    case QueryType.Insert:
                        CascadeCreateSingle( context.MetaStructure , context.EntityInstance );
                        CascadeCreateMany( context.EntityInstance , context.MetaStructure , GetChildrenHandle( context ) );
                        break;
                    case QueryType.Delete:
                        CacheService.GetInstance( ).Remove( KeyGenerator.GetKey( context.EntityInstance ) );
                        break;
                }
            }

            return result;
        }

        private void UpdateInheritanceHierachy( QueryContext context )
        {
            if ( context.MetaStructure.HasBaseEntity )
            {
                context = ExecuteUpdate( context , context.MetaStructure.BaseEntity , context.EntityInstance , context.QueryType );
            }
        }

        private void UpdateEmbeddedEntities( QueryContext context )
        {
            if ( context.MetaStructure.HasEmbeddedEntities )
            {
                SessionRuntime runtime = SessionRuntime.GetInstance( );

                foreach ( EmbeddedEntity embedded in context.MetaStructure.EmbeddedTypes )
                {
                    int index = context.MetaStructure.GetKeyIndexByMapping( embedded.JoinColumn );

                    int count = runtime.Count( embedded.Type , 
                                                        new[] { embedded.JoinColumn } , 
                                                        new[] { context.Values[index] } 
                                              );

                    if ( count > 0 && context.QueryType == QueryType.Delete )
                    {
                        runtime.RemoveEntity( embedded.Type , context.EntityInstance );
                    }
                    else
                    {
                        context.QueryType = count > 0 ? QueryType.Update : QueryType.Insert;

                        if ( context.QueryType == QueryType.Insert )
                        {
                            runtime.CreateNewWithoutCascade( embedded.Type , context.EntityInstance );
                        }
                        else
                        {
                            runtime.SaveEntity( embedded.Type , context.EntityInstance , false , false );
                        }
                    }
                }
            }
        }

        internal static QueryContext ExecuteUpdate( QueryContext context , Type type , object entity , QueryType queryType )
        {
            SessionRuntime runtime = SessionRuntime.GetInstance( );

            object[] keys = MetaDataManager.GetUniqueKeys( entity );
            object[] names = MetaDataManager.GetUniqueKeyNames( type );

            int exist = ( int )new From( type ).As( "a" ).Where( names , keys ).Select( ).Count( );

            if ( exist > 0 )
            {
                runtime.ExecuteChanges( QueryType.Update , type , entity );
            }
            else
            {
                keys = runtime.CreateNewEntity( type , entity );                
            }

            if( context.Audit.WasAudited( context.MetaStructure.InheritanceRelation.ClassDefinationName ) )
            {
                context.Audit[context.MetaStructure.InheritanceRelation.ClassDefinationName].Value = keys[0];
            }

            return context;
        }

        #region Cascade Operations

        private void CascadeRemove( object instance , EntityMetadata metadata , IDictionary<string , ILazyLoader> lazyHandlers )
        {
            foreach ( PropertyMetadata columnInfo in metadata.GetAll( ).Where( c => c.IsManyToOne || c.IsOneToOne || c.IsManyToMany || c.IsOneToMany ) )
            {
                if ( ( columnInfo.IsManyToMany || columnInfo.IsOneToMany ) && IsCascadeDelete( columnInfo.Cascade ) )
                {
                    if ( lazyHandlers[columnInfo.ClassDefinationName].Count > 0 )
                    {
                        lazyHandlers[columnInfo.ClassDefinationName].RemoveAllChildren( );
                    }
                }
                else if ( ( ( columnInfo.IsOneToOne || columnInfo.IsManyToOne ) && !columnInfo.IsEntitySplitJoin ) && IsCascadeDelete( columnInfo.Cascade ) )
                {
                    //Many To One is a bit dangerous for cascade delete operation
                    //You cannot delete your parent, because of other orphans
                    if ( columnInfo.IsManyToOne )
                    {
                        throw new CascadeException( "Cascade does not support deletion of Key owner" );
                    }

                    object value = MetaDataManager.GetPropertyValue( columnInfo.ClassDefinationName , instance );
                    object[] uniqueKeys = MetaDataManager.GetUniqueKeys( value );

                    if ( null == value )
                        continue;

                    SessionRuntime.GetInstance( ).RemoveEntity( columnInfo.PropertyType , value , uniqueKeys );
                }
            }
        }

        private void CascadeUpdate( object instance , EntityMetadata metadata , IDictionary<string , ILazyLoader> lazyHandlers )
        {
            foreach ( PropertyMetadata columnInfo in metadata.GetAll( ).Where( c => c.IsRelationshipMapping && ( c.Cascade == Cascade.UPDATE || c.Cascade == Cascade.ALL ) ) )
            {
                SessionRuntime runtime = SessionRuntime.GetInstance( );

                if ( columnInfo.IsOneSided )
                {
                    object value = MetaDataManager.GetPropertyValue( columnInfo.ClassDefinationName , instance );

                    if ( null == value )
                        continue;

                    if ( !GhostGenerator.IsAGhost( value ) )
                    {
                        runtime.CreateNewEntity( columnInfo.PropertyType , ref value , RuntimeBehaviour.DoNothing );
                    }
                    else
                    {
                        runtime.SaveEntity( columnInfo.PropertyType , value );
                    }
                }
                else if ( columnInfo.IsManySided )
                {
                    object value = MetaDataManager.GetPropertyValue( columnInfo.ClassDefinationName , instance );

                    if ( value == null )
                        continue;

                    if ( value is IInternalList )
                    {
                        lazyHandlers[columnInfo.ClassDefinationName].SaveAllChanges( );
                    }
                    else if( lazyHandlers.IsNotNull( ) )
                    {
                        IEnumerator enumerator = ( IEnumerator )( ( IEnumerable )value ).GetEnumerator( );

                        while( enumerator.MoveNext( ) )
                        {
                            object child = enumerator.Current;
                            lazyHandlers[columnInfo.ClassDefinationName].PersistChildObject( child );
                        }
                    }
                }
            }
        }

        private void CascadeCreateSingle( EntityMetadata metadata , object entity )
        {
            foreach ( PropertyMetadata columnInfo in metadata.GetAll( ).Where( c => c.IsOneSided ) )
            {
                object relation = MetaDataManager.GetPropertyValue( columnInfo.ClassDefinationName , entity );

                if ( relation.IsNull( ) )
                    continue;

                bool isLoaded = GhostGenerator.EntityIsLoaded( relation );
                SessionRuntime runtime = SessionRuntime.GetInstance( );

                EntityMetadata childMeta = MetaDataManager.PrepareMetadata( columnInfo.RelationType );
                PropertyMetadata column = childMeta.GetOneSideRelationshipWith( metadata.Type );

                if ( IsCascadeCreate( columnInfo.Cascade ) && !columnInfo.IsEntitySplitJoin )
                {
                    if ( isLoaded )
                    {
                        runtime.SaveEntity( columnInfo.PropertyType , relation );
                    }
                    else
                    {
                        runtime.CreateNewEntity( columnInfo.PropertyType , ref relation , RuntimeBehaviour.DoNothing );
                    }

                    CacheService.GetInstance( ).Add( relation , true );
                }
            }
        }

        private void CascadeCreateMany( object entity , EntityMetadata metadata , IDictionary<string , ILazyLoader> lazyHandlers )
        {
            foreach ( PropertyMetadata columnInfo in metadata.LazyBag )
            {
                if ( columnInfo.IsManySided && IsCascadeCreate( columnInfo.Cascade ) )
                {
                    object value = MetaDataManager.GetPropertyValue( columnInfo.ClassDefinationName , entity );

                    if ( value == null )
                        continue;

                    if ( value is IInternalList )
                    {
                            lazyHandlers[columnInfo.ClassDefinationName].SaveAllChanges( );
                    }
                    else
                    {
                        IEnumerator enumerator = ( IEnumerator )( ( IEnumerable )value ).GetEnumerator( );

                        while ( enumerator.MoveNext( ) )
                        {
                            object child = enumerator.Current;
                            lazyHandlers[columnInfo.ClassDefinationName].PersistChildObject( child );
                        }
                    }
                }
            }
        }

        #endregion


        #region Helper Methods

        private static IDictionary<string , ILazyLoader> GetChildrenHandle( QueryContext context )
        {
            return GhostGenerator.GetLazyLoadHandlers( context.EntityType , context.Values );
        }

        private static IDictionary<string , ILazyLoader> GetChildrenHandle( object entity )
        {
            return GhostGenerator.GetLazyLoadHandlers( entity );
        }

        private bool IsCascadeCreate( Cascade cascade )
        {
            return ( cascade == Cascade.CREATE || cascade == Cascade.ALL );
        }

        private bool IsCascadeUpdate( Cascade cascade )
        {
            return ( cascade == Cascade.UPDATE || cascade == Cascade.ALL );
        }

        private bool IsCascadeDelete( Cascade cascade )
        {
            return ( cascade == Cascade.DELETE || cascade == Cascade.ALL );
        }

        #endregion
    }
}
