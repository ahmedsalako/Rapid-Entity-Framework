using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using PersistentManager.Mapping;
using System.Data;
using System.Data.Common;
using PersistentManager.Descriptors;
using PersistentManager.Cache;
using System.Runtime.Remoting.Messaging;
using PersistentManager.Initializers.Interfaces;
using System.Collections.Specialized;
using PersistentManager.Util;
using PersistentManager.Exceptions.EntityManagerException;
using PersistentManager.Initializers;
using PersistentManager.Collections;
using PersistentManager.Query;
using PersistentManager.Query.Sql;
using PersistentManager.Provider;
using PersistentManager.Runtime;

namespace PersistentManager
{
    internal class SessionRuntime : RuntimeTransactionScope , IDisposable
    {
        private List<IDataReader> activeDataReaders = new List<IDataReader>();
        private IDatabaseProvider dataBaseProvider;      

        private SessionRuntime( ) 
        {
            this.CacheService = new CacheService( TransactionalCache.CreateInstance( ) );
            Guid scopeId = Guid.NewGuid( );
            ConfigurationBase.SetCurrentScope( scopeId  );
            ScopeContext.SetData( scopeId.ToString( ) , this );
        }

        internal SessionRuntime( IDatabaseProvider dataBaseProvider , string name ) : this()
        {
            this.RuntimeRegistryName = name;
            this.dataBaseProvider = dataBaseProvider;
            this.dataBaseProvider.RegisterReader += new RegisterDataReader( DataBaseProvider_RegisterReader );
        }        

        internal IDatabaseProvider DataBaseProvider
        {
            get { return dataBaseProvider; }
            set { dataBaseProvider = value; }
        }

        internal string RuntimeRegistryName { get; set; }

        internal static SessionRuntime GetInstance( )
        {
            Guid currentScope = ConfigurationBase.GetCurrentScope( );
            return ScopeContext.GetData( currentScope.ToString( ) ) as SessionRuntime;
        }

        internal object LoadEntity( Type type , params object[] keys )
        {
            QueryContext context = new QueryContext( QueryType.Select , type , keys , null );
            
            object entity = new SelectExpression( context ).ReturnSingle( context );

            PersistenceInterceptor.Intercept( entity , context.MetaStructure , QueryType.Select );

            return entity;
        }

        internal IDataReader LoadEntityReturnReader( Type type , params object[] keys )
        {
            QueryContext context = new QueryContext( QueryType.Select , type , keys , null );

            return new SelectExpression( context ).ReturnDataReader( context );
        }

        internal T Detach<T>( object entity )
        {
           return ( T ) GhostGenerator.Detach<T>( entity );
        }

        internal IList FindEntityByProperties( Type type , string[] criteriaNames , object[] criteriaValues )
        {
            QueryContext context = new QueryContext( QueryType.SelectAll , type , criteriaValues , criteriaNames );
            return new SelectExpression( context ).ReturnMany( context );
        }

        internal object FindFirst( Type type , string[] criteriaNames , object[] criteriaValues )
        {
            QueryContext context = new QueryContext( QueryType.SelectFirst , type , criteriaValues , criteriaNames );

            return new SelectExpression( context ).ReturnSingle( context );
        }

        internal object LoadSingleWithoutRead( Type type , IDataReader dataReader )
        {
            return SelectExpression.ReturnSingle( type , dataReader , false );
        }

        internal object LoadSingleWithoutRead( Type type , CompositeCriteria criteria , IDataReader dataReader )
        {
            return SelectExpression.ReturnSingle( type , criteria , dataReader );
        }

        internal int Count( Type type , string[] criteriaNames , object[] criteriaValues )
        {
            type = GhostGenerator.CreateGhostType( type );
            QueryContext context = new QueryContext( QueryType.SelectCount );
            context.EntityType = type;
            context.MetaStructure = MetaDataManager.PrepareMetadata( type );
            context.Names = ( string[] ) criteriaNames;
            context.Values = criteriaValues;

            return new SelectExpression( context ).Count( context );
        }

        internal IList FindEntityByFields( Type type , string[] criteriaNames , object[] criteriaValues )
        {
            return FindEntityByProperties( type , criteriaNames , criteriaValues );
        }

        internal T CreateInstance<T>( params object[] constructorArgs )
        {
            return ( T ) GhostGenerator.CreateGhostInstance( typeof( T ) , null );
        }

        internal IList<T> GetAllLazily<T>( string[] criterias , object[] values )
        {
            return ( IList<T> ) GetAllLazily( typeof( T ) , true , criterias , values );
        }

        internal object GetAllLazily( Type type , bool isGeneric )
        {
            QueryContext context = new QueryContext( QueryType.SelectAll );
            context.EntityType = type;
            context.MetaStructure = MetaDataManager.PrepareMetadata( type );

            return new SelectExpression( context ).ReturnManyLazy( type , isGeneric );
        }

        internal IList GetAll( Type type )
        {
            EntityMetadata typeMeta = EntityMetadata.GetMappingInfo( type );

            QueryContext context = new QueryContext( QueryType.SelectAll );
            context.MetaStructure = typeMeta;
            context.EntityType = type;

            return new SelectExpression( context ).ReturnMany( context );
        }

        internal void SaveEntity( Type type , object entity )
        {
            SaveEntity( type , entity , MetaDataManager.GetUniqueKeys( entity ) , false , true );
        }

        internal void SaveEntity( Type type , object entity , bool forceAudit , bool allowCascade )
        {
            SaveEntity( type , entity , MetaDataManager.GetUniqueKeys( entity ) , forceAudit , allowCascade );
        }

        internal void ExecuteChanges( QueryType queryType , Type type , object entity )
        {
            switch ( queryType )
            {
                case QueryType.Insert:
                    CreateNewEntity( type , entity );
                    break;
                case QueryType.Update:
                    SaveEntity( type , entity );
                    break;
                case QueryType.Delete:
                    RemoveEntity( type , entity );
                    break;
                default:
                    break;
            }
        }

        internal object CreateOrUpdate( object entity )
        {
            Type type = MetaDataManager.GetEntityType( entity );

            if ( MetaDataManager.IsEntityLoaded( entity ) )
            {
                SaveEntity( type , entity );
            }
            else
            {
                CreateNewEntity( type , ref entity , RuntimeBehaviour.DoNothing );
            }

            return entity;
        }

        internal object Merge( object source )
        {
            object destination = LoadEntity( MetaDataManager.GetEntityType( source ) , MetaDataManager.GetUniqueKeys( source ) );

            object merge = Merge( source , destination  , new Dictionary<string , object>( ) );

            SaveEntity( MetaDataManager.GetEntityType( source ) , merge ?? destination );

            return merge ?? destination;
        }

        internal object Merge( object source , object destination , IDictionary<string, object> changeCache )
        {
            string key = KeyGenerator.GetKey( source );

            if ( destination.IsNull( ) )
                return null;

            if ( !changeCache.ContainsKey( key ) )
            {
                Type type = MetaDataManager.GetEntityType( source );
                EntityMetadata metadata = type.GetMetataData( );
                changeCache[key] = destination;

                foreach ( PropertyMetadata property in metadata.GetAllIncludeBase( ) )
                {
                    object sourceValue = MetaDataManager.GetPropertyValue( property.ClassDefinationName , source );
                    object destValue = MetaDataManager.GetPropertyValue( property.ClassDefinationName , destination );

                    if ( sourceValue.IsNull( ) ) continue;

                    if ( property.IsRelationshipMapping )
                    {
                        if ( sourceValue.IsNotNull( ) )
                        {
                            IDictionary<string , ILazyLoader> lazyHandlers = GhostGenerator.GetLazyLoadHandlers( destination );

                            if ( property.IsOneSided )
                            {
                                if ( destValue.IsNotNull( ) )
                                {
                                    Merge( sourceValue , destValue , changeCache );
                                }
                                else
                                {
                                    Merge( sourceValue , sourceValue , changeCache );
                                }
                            }
                            else if ( property.IsManySided )
                            {
                                IEnumerator enumerator = ( IEnumerator )( ( IEnumerable )sourceValue ).GetEnumerator( );

                                while ( enumerator.MoveNext( ) )
                                {
                                    bool exist = lazyHandlers.IsNull( ) ? false : lazyHandlers[property.ClassDefinationName].ChildExist( enumerator.Current );
                                    object entity = null;

                                    if ( exist )
                                    {
                                        entity = Merge( enumerator.Current , LoadEntity( property.RelationType , MetaDataManager.GetUniqueKeys( enumerator.Current ) ) , changeCache );
                                    }
                                    else
                                    {
                                        entity = Merge( enumerator.Current , enumerator.Current , changeCache );

                                        if ( lazyHandlers.IsNotNull( ) )
                                        {
                                            lazyHandlers[property.ClassDefinationName].Add( entity );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if( !property.IsDiscriminator && !property.IsAutoGenerated )
                    {
                        if( !sourceValue.Equals( destValue ) )
                        {
                            MetaDataManager.SetPropertyValue( property.ClassDefinationName , destination , sourceValue );
                        }
                    }
                }                                
            }

            return changeCache[key];
        }

        internal void SaveEntity( Type type , object entity , object[] keys , bool forceAudit , bool allowCascade )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );
            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Update );

            QueryContext context = new QueryContext( QueryType.Update );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Values = keys;
            context.Audit = SetAllSelfTrackedPropertiesForUpdate( metadata.Type , entity );

            if ( forceAudit && ( context.Audit.IsNull( ) || context.Audit.Count <= 0 ) )
            {
                context.Audit = SetAllPropertiesForCreate( entity , metadata.Type );
                SetDirtyState( entity , true );
            }

            UpdateExpression update = new UpdateExpression( context );

            if ( allowCascade )
            {
                update.ExecuteUpdate( context );
            }
            else
            {
                update.ExecuteUpdateNoCascade( context );
            }
        }

        internal object[] CreateNewEntity( Type type , ref object entity , RuntimeBehaviour runtimeBehaviour )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Insert );

            QueryContext context = new QueryContext( QueryType.Insert );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Audit = SetAllPropertiesForCreate( entity , type );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdate( context );

            entity = LoadEntity( type , context.Values );
            CacheService.SetCreatedUncommited( entity , TransactionId );

            return context.Values;
        }

        internal object[] CreateNewWithoutCascade( Type type , object entity )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Insert );

            QueryContext context = new QueryContext( QueryType.Insert );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Audit = SetAllPropertiesForCreate( entity , type );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdateNoCascade( context );

            return context.Values;
        }

        internal object[] CreateNewEntity( Type type , object entity )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Insert );

            QueryContext context = new QueryContext( QueryType.Insert );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Audit = SetAllPropertiesForCreate( entity , type );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdate( context );

            return context.Values;
        }

        internal object CreateNewEntityReturnsEntity( Type type , object entity )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Insert );

            QueryContext context = new QueryContext( QueryType.Insert );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Audit = SetAllPropertiesForCreate( entity , type );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdate( context );

            entity = LoadEntity( type , context.Values );
            CacheService.SetCreatedUncommited( entity , TransactionId );

            return entity;
        }

        internal int UpdateEntity( Type type , object entity , object[] keys )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Update );

            QueryContext context = new QueryContext( QueryType.Update );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Values = keys;
            context.Audit = SetAllSelfTrackedPropertiesForUpdate( metadata.Type , entity );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdate( context );

            return context.IsUpdated ? 1 : 0;
        }

        internal int RemoveEntity( Type type , object entity )
        {
            return RemoveEntity( type , entity , MetaDataManager.GetUniqueKeys( entity ) );
        }

        internal int RemoveEntity( Type type , object entity , object[] keys )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( entity , metadata , QueryType.Delete );

            QueryContext context = new QueryContext( QueryType.Delete );
            context.EntityType = type;
            context.EntityInstance = entity;
            context.MetaStructure = metadata;
            context.Names = MetaDataManager.GetUniqueKeyNames( type );
            context.Values = keys;

            RemoveCacheableReference( entity );

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdate( context );

            return context.IsUpdated ? 1 : 0;
        }

        /// <summary>
        /// Note this method will not trigger cascade
        /// </summary>
        /// <param name="type"></param>
        /// <param name="names"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool RemoveEntityByFieldName( Type type , string[] names , object[] values )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );

            PersistenceInterceptor.Intercept( type , metadata , QueryType.Update );

            QueryContext context = new QueryContext( QueryType.Delete );
            context.EntityType = type;
            context.MetaStructure = metadata;
            context.Names = names;
            context.Values = values;

            UpdateExpression update = new UpdateExpression( context );
            update.ExecuteUpdateNoCascade( context );

            return context.IsUpdated;
        }

        internal void CloseActiveReaders( )
        {
            foreach ( IDataReader dataReader in activeDataReaders )
            {
                if ( !dataReader.IsClosed )
                {
                    dataReader.Close( );
                }
            }

            activeDataReaders.Clear( );
        }

        internal void DataBaseProvider_RegisterReader( IDataReader dataReader )
        {
            activeDataReaders.Add( dataReader );
        }

        internal bool HasActiveReaders( )
        {
            foreach ( IDataReader dataReader in activeDataReaders )
            {
                if ( !dataReader.IsClosed )
                    return true;
            }

            return false;
        }

        public void Dispose( )
        {
            Guid currentScope = ConfigurationBase.GetCurrentScope( );

            if (currentScope == Guid.Empty)
                return;

            ScopeContext.RemoveData( currentScope.ToString( ) );
            ConfigurationBase.RemoveCurrentScope( );
        }

        internal object GetAllLazily( Type type , bool isGeneric , string[] criterias , object[] values )
        {
            QueryContext context = new QueryContext( QueryType.SelectAll );
            context.EntityType = type;
            context.Names = criterias;
            context.Values = values;

            context.MetaStructure = MetaDataManager.PrepareMetadata( type );

            return new SelectExpression( context ).ReturnManyLazy( type , isGeneric );
        }

        internal object GetLazyManyToMany( PropertyMetadata property , bool isGeneric , string[] ownerCriterias , object[] ownerKeys )
        {
            QueryContext context = new QueryContext( QueryType.SELECT_MANY_TO_MANY );
            context.EntityType = property.RelationType;
            context.JoinTableType = property.JoinTableType;
            context.Names = ownerCriterias;
            context.Values = ownerKeys;

            context.Property = property;
            context.MetaStructure = MetaDataManager.PrepareMetadata( property.RelationType );

            return new SelectExpression( context ).ReturnManyLazy( property.RelationType , isGeneric );
        }

        internal IList GetRange<T>( int Start , int End , string[] properties , object[] values )
        {
            QueryContext context = new QueryContext( QueryType.SelectAllInRange );
            context.EntityType = typeof( T );
            context.Names = properties;
            context.Values = values;

            context.MetaStructure = MetaDataManager.PrepareMetadata( typeof( T ) );

            return new SelectExpression( context ).ReturnManyByRange( Start , End , context );
        }
    }
}
