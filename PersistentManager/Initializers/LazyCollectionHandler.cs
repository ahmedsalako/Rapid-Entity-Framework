using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Initializers.Interfaces;
using System.Data.Common;
using PersistentManager.Descriptors;
using PersistentManager.Cache;
using PersistentManager.Query.Sql;
using PersistentManager.Query;
using System.Data;
using PersistentManager.Query.Keywords;
using PersistentManager.Util;

namespace PersistentManager.Initializers
{
    public class LazyCollectionHandler : Lazy , ILazyLoader 
    {
        QueryContext QueryContext { get; set; }

        public object[] OwnerKey { get; set; }

        internal LazyCollectionHandler( QueryContext queryContext ) : base( queryContext.EntityType )
        {
            QueryContext = queryContext;
            Orphans = new List<object>();
        }

        public object GetAllType( )
        {
            throw new NotImplementedException( );
        }

        public new object GetType( )
        {
            throw new NotImplementedException( );
        }

        public override void PersistChildObject( object child )
        {
            if ( ChildExist( child ) )
            {
                SessionRuntime.GetInstance( ).UpdateEntity( child.GetType( ) , child , MetaDataManager.GetUniqueKeys( child ) );
            }

            object[] key = MetaDataManager.GetUniqueKeys( child );
            
            if ( !MetaDataManager.IsEntityLoaded( child ) )
            {
                child = SessionRuntime.GetInstance( ).CreateNewEntityReturnsEntity( child.GetType( ) , child );
            }
            else
                SessionRuntime.GetInstance( ).UpdateEntity( child.GetType( ) , child , key );
        }

        public bool ChildExist( object child )
        {
            return Contains( child ) ? true : OrphanExists( child );
        }

        public bool Contains( object child )
        {
            object exist = null;

            try
            {
                object[] keys = MetaDataManager.GetUniqueKeys( child );
                string[] criterias = MetaDataManager.GetUniqueKeyNames( child.GetType( ) );

                exist = SessionRuntime.GetInstance( ).FindEntityByFields( child.GetType( ) , criterias , keys )[0];
            }
            catch
            {
                exist = null;
            }

            return ( null != exist );
        }

        public int Count
        {
            get { return CountDataStore + OrphanCount; }
        }

        public int CountDataStore
        {
            get
            {
                return ( int ) new From( QueryContext.EntityType ).As( "a" )
                                .Where( QueryContext.Names , QueryContext.Values )
                                .Select( )
                                .Count( );
            }
            set
            {

            }
        }

        public bool RemoveChild( object child )
        {
            if ( Contains( child ) )
            {
                object[] key = MetaDataManager.GetUniqueKeys( child );
                SessionRuntime.GetInstance( ).RemoveEntity( child.GetType() , child , key );
                return true;
            }
            else if ( OrphanExists( child ) )
            {
                Orphans.Remove( child );
                return true;
            }

            return false;
        }

        public object GetIndex( int index )
        {
            int count = CountDataStore;
            if ( count >= ( index + 1 ) )
            {
                IDataReader dataReader = dataReader = new From( QueryContext.EntityType ).As( "a" )
                                                            .Where( QueryContext.Names , QueryContext.Values )
                                                            .OrderBy( ArrayUtil.PrefixWith( "a." , MetaDataManager.GetUniqueKeyNames( QueryContext.EntityType ) ) )
                                                            .Select( )
                                                            .LimitQuery( (index + 1) - 1 , index + 1 )                                                          
                                                            .DataReader;

                return AddLoaded( SelectExpression.ReturnSingle( QueryContext.EntityType , dataReader , true ) );
            }
            else if ( ( index + 1 ) > count )
            {
                return GetOphanIndex( index , count );
            }

            throw new Exception( "Could not locate entity at this index" );
        }

        public void RemoveAt( int index )
        {
            object entity = GetIndex( index );

            if ( OrphanExists( entity ) )
            {
                Orphans.Remove( entity );
            }

            CacheService.GetInstance( ).Remove( KeyGenerator.GetKey( entity ) );
        }

        public IDataReader GetDataReader( )
        {
            return QueryContext.DataReader;
        }

        public IEnumerable<T> GetEntityReader<T>( )
        {
            IDataReader dataReader = GetDataReader( );
            while ( dataReader.Read( ) )
            {
                yield return (T) AddLoaded( SelectExpression.ReturnSingle( QueryContext.EntityType , dataReader , false ) );
            }

            foreach ( object entity in Orphans )
            {
                yield return ( T ) entity;
            }
        }

        public void RemoveAllChildren( )
        {
            ClearOrphans( );
        }
    }
}
