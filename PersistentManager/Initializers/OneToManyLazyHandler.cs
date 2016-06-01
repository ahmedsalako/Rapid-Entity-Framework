using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Initializers.Interfaces;
using System.Collections;
using System.Data.Common;
using PersistentManager.Cache;
using System.Reflection;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;
using PersistentManager.Query.Sql;
using PersistentManager;
using System.Data;
using System.Linq;
using PersistentManager.Metadata;

namespace PersistentManager.Initializers
{
    public class OneToManyLazyHandler : Lazy , ILazyLoader
    {
        object frameWorkList = null;
        PropertyInfo ownerProperty;
        private Type lazyType;
        Type ownerType;
        int childCount = 0;
        GetAll GetAllHandler;
        GetType GetTypeHandler;

        public OneToManyLazyHandler( Type entityType , PropertyInfo property , PropertyMetadata propertyMetadata , object[] entityKeys )
            : base( property.PropertyType )
        {
            this.propertyMetadata = propertyMetadata;
            this.lazyType = propertyMetadata.RelationType;
            this.OwnerKey = entityKeys;
            this.ownerProperty = property;
            this.ownerType = entityType;
        }

        public object[] OwnerKey { get; set; }

        public object[] ReferencedKey
        {
            get
            {
                return GetReferencedKeys().ToArray();
            }
        }

        private IEnumerable<object> GetReferencedKeys( )
        {
            foreach ( JoinMetadata join in propertyMetadata.JoinDetails )
            {
                int keyIndex = EntityMetadata.GetMappingInfo( ownerType )
                    .GetKeyIndexByMapping( join.JoinColumn );

                yield return OwnerKey[keyIndex];
            }
        }

        #region Public Members

        public object GetAllType( )
        {
            if ( IsPersisted )
            {
                GetAllHandler = new GetAll( GetAllWithDataReader );

                return GetAllHandler.Invoke( lazyType , RelationColumns , OwnerKey );
            }
            else
            {
                return Orphans;
            }
        }

        object ILazyLoader.GetType( )
        {
            GetTypeHandler = new GetType( GetTypeWithDataReader );
            return GetTypeHandler.Invoke( lazyType , RelationColumns , ReferencedKey );
        }

        public object GetTypeWithDataReader( Type type , string[] relationship , object[] values )
        {
            QueryContext context = new From( type ).As( "a" )
                                                .Where( relationship , ReferencedKey )
                                                .Select( )
                                                .ExecuteQuery( );

            IDataReader dataReader = context.DataReader;

            while ( dataReader.Read( ) )
            {
                object row = Activator.CreateInstance( type );
                MetaDataManager.LoadSingleEntity( ref row , dataReader );
                return row;
            }

            return Activator.CreateInstance( type );
        }

        public object GetAllWithDataReader( Type type , string[] relationship , object values )
        {
            if ( null != frameWorkList )
                return frameWorkList;

            Type returnType = ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor( ownerProperty.PropertyType );
            frameWorkList = ConcreteCollectionDiscovery.MakeInstance( returnType , this , type );
            return frameWorkList;
        }

        public override void PersistChildObject( object child )
        {
            if ( IsPersisted )
            {
                bool childExist = Contains( child );
                object[] key = MetaDataManager.GetUniqueKeys( child );

                if ( childExist )
                {
                    SessionRuntime.GetInstance().UpdateEntity(child.GetType(), child, key); return;
                }

                EntityMetadata metaData = MetaDataManager.PrepareMetadata( lazyType );

                //Inverse
                foreach ( PropertyMetadata column in metaData.ColumnInfoBag )
                {
                    if ( column.IsManyToOne && ( ownerType.Name == column.PropertyType.Name || ownerType.BaseType.Name == column.PropertyType.Name ) )
                    {
                        if (ArrayUtil.ContentEquals(RelationColumns, column.RelationColumns))
                        {
                            object parentObject = SessionRuntime.GetInstance( ).LoadEntity( ownerType , OwnerKey );
                            MetaDataManager.SetPropertyValue( column.ClassDefinationName , child , parentObject );
                            break;
                        }
                    }
                }

                //Is Primary Key Null. 
                if ( !childExist )
                {
                    child = SessionRuntime.GetInstance( ).CreateNewEntityReturnsEntity( lazyType , child );
                }
                else
                    SessionRuntime.GetInstance( ).UpdateEntity( lazyType , child , key );
            }
        }

        public bool ChildExist( object child )
        {
            return Contains( child ) ? true : OrphanExists( child );
        }

        public bool Contains( object child )
        {
            if ( IsPersisted )
            {
                try
                {
                    object[] key = MetaDataManager.GetUniqueKeys( child );

                    if ( MetaDataManager.KeyIsNotValid( key ) )
                        return false;

                    string[] criterias = MetaDataManager.GetUniqueKeyNames( lazyType );
                    object[] values = key;

                    return (
                                new From( lazyType ).As( "a" ).Where( criterias , values )
                                                            .And( RelationColumns , Condition.Equals , ReferencedKey )
                                                            .Select( criterias )
                                                            .DistinctInternal( )

                           ).DataReader.Read( );
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public int Count
        {
            get { return CountDataStore + OrphanCount; }
        }

        public int CountDataStore
        {
            get
            {
                if ( IsPersisted )
                {
                    return ( int )new From( lazyType ).As( "a" )
                                                    .Where( RelationColumns , ReferencedKey )
                                                    .Select( )
                                                    .Count( );
                }

                return 0;
            }
            set
            {
                childCount = value;
            }
        }

        public bool RemoveChild( object child )
        {
            if ( IsPersisted )
            {
                if ( Contains( child ) )
                {
                    object[] key = MetaDataManager.GetUniqueKeys( child );
                    SessionRuntime.GetInstance( ).RemoveEntity( child.GetType( ) , child , key );
                    return true;
                }
                else if ( OrphanExists( child ) )
                {
                    Orphans.Remove( child );
                    return true;
                }
            }

            return false;
        }

        public void RemoveAllChildren( )
        {
            if ( IsPersisted )
            {
                GetAllWithDataReader( lazyType , RelationColumns , OwnerKey );

                if ( null == frameWorkList )
                    return;

                IEnumerator enumerator = ( IEnumerator ) ( ( IEnumerable ) frameWorkList ).GetEnumerator( );

                while ( enumerator.MoveNext( ) )
                {
                    object child = enumerator.Current;
                    object[] key = MetaDataManager.GetUniqueKeys( child );
                    SessionRuntime.GetInstance( ).RemoveEntity( child.GetType() , child , key );
                }
            }

            ClearOrphans( );
        }

        public object GetIndex( int index )
        {
            if ( IsPersisted )
            {
                int count = CountDataStore;

                if ( count >= ( index + 1 ) )
                {
                    int counter = 0;
                    QueryContext context = new From( lazyType ).As( "a" )
                                                                .Where( RelationColumns , ReferencedKey )
                                                                .Select( )
                                                                .LimitQuery( index , index + 1 );

                    IDataReader dataReader = context.DataReader;

                    CacheService persistentChache = CacheService.GetInstance( );

                    while ( dataReader.Read( ) )
                    {
                        ++counter;
                        if ( ( index + 1 ) > 0 )
                        {
                            string cacheKey = KeyGenerator.GetKey( dataReader , lazyType );

                            if ( persistentChache.Contains( cacheKey ) )
                            {
                                return AddLoaded( persistentChache[cacheKey] );
                            }

                            EntityMetadata metadata = MetaDataManager.PrepareMetadata( lazyType );
                            object row = GhostGenerator.LoadGhost( metadata , dataReader );
                            persistentChache.Add( cacheKey , lazyType , row );
                            MetaDataManager.LoadMultipleEntities( row , dataReader , metadata );

                            return AddLoaded( row );
                        }
                    }
                }
                else if( ( index + 1 ) > count )
                {
                    return GetOphanIndex( index , count );
                }
            }

            throw new Exception( "Could not locate entity at this index" );
        }

        public void RemoveAt( int index )
        {
            if ( IsPersisted )
            {
                object entity = GetIndex( index );

                if ( OrphanExists( entity ) )
                {
                    Orphans.Remove( entity );
                }

                object[] key = MetaDataManager.GetUniqueKeys( entity );
                SessionRuntime.GetInstance( ).RemoveEntity( entity.GetType( ) , entity , key );
            }
        }

        public IDataReader GetDataReader( )
        {
            QueryContext context = new From( lazyType ).As( "a" )
                                                .Where( RelationColumns , ReferencedKey )
                                                .Select( )
                                                .ExecuteQuery( );
            return context.DataReader;
        }

        public IEnumerable<T> GetEntityReader<T>( )
        {
            IDataReader dataReader = GetDataReader( );

            while ( dataReader.Read( ) )
            {
                T entity = ( T ) SelectExpression.ReturnSingle( lazyType , dataReader , false );
                AddLoaded( entity );

                yield return entity;
            }

            foreach ( object entity in Orphans )
            {
                yield return ( T ) entity;
            }
        }

        #endregion

    }
}