using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Cache;
using PersistentManager.Initializers;
using PersistentManager.Collections;
using System.Data;
using PersistentManager.Query.Keywords;
using PersistentManager.Util;
using PersistentManager.Provider;

namespace PersistentManager.Query.Sql
{
    internal class SelectExpression : QueryExpression
    {
        public SelectExpression( QueryContext context )
        {
            this.QueryContext = context;
        }

        protected override QueryContext PrepareQuery( )
        {
            if ( QueryContext.QueryType == QueryType.Select )
            {
                return PrepareSelect( );
            }
            else if ( QueryContext.QueryType == QueryType.SelectAll )
            {
                return PrepareSelectAll( );
            }
            else if ( QueryContext.QueryType == QueryType.SelectByCriteria )
            {
                return PrepareSelectByCriteria( );
            }
            else if ( QueryContext.QueryType == QueryType.SelectAllInRange )
            {
                return PrepareSelectAllInRange( );
            }
            else if ( QueryContext.QueryType == QueryType.SelectCount )
            {
                return PrepareSelectCount( );
            }
            else if ( QueryContext.QueryType == QueryType.SelectFirst )
            {
                return PrepareFirst();
            }
            else if( QueryContext.QueryType == QueryType.SELECT_MANY_TO_MANY )
            {
                return PrepareManyToMany( );
            }

            return null;
        }

        private QueryContext PrepareManyToMany( )
        {
            return new From( QueryContext.EntityType ).As( "a" )
                            .Join( QueryContext.JoinTableType , "b" ).On( "a".Dot( QueryContext.Property.RightKeys ) ).EqualsTo( "b".Dot( QueryContext.Property.JoinColumns ) )                            
                            .Where( "b".Dot( QueryContext.Names ) , QueryContext.Values )
                            .Select( )
                            .ExecuteQuery( );
        }

        private QueryContext PrepareSelectByCriteria( )
        {
            return new From( QueryContext.EntityType ).As( "a" )
                                   .Where( QueryContext.Names , QueryContext.Values )
                                   .OrderBy( )
                                   .Select( )
                                   .ExecuteQuery( );
        }

        private QueryContext PrepareSelectCount( )
        {
             QueryContext.ScalarResult =  new From( QueryContext.EntityType ).As( "a" )
                                               .Where( QueryContext.Names , QueryContext.Values )
                                               .OrderBy( )
                                               .Select( )
                                               .Count( );

             return QueryContext;
        }

        private QueryContext PrepareFirst()
        {
            return new From( QueryContext.EntityType ).As( "a" )
                        .Where( QueryContext.Names , QueryContext.Values )
                        .Select( )
                        .FirstInternal( );    
        }

        private QueryContext PrepareSelectAll( )
        {
            return new From( QueryContext.EntityType ).As( "a" )
                        .Where( QueryContext.Names , QueryContext.Values )
                        .Select( )
                        .ExecuteQuery( );
        }

        private QueryContext PrepareSelectAllInRange( )
        {
            return new From( QueryContext.EntityType ).As( "a" )
                        .Where( QueryContext.Names , QueryContext.Values )
                        .Select( )
                        .LimitQuery( QueryContext.StartRange , QueryContext.EndRange );
        }

        private QueryContext PrepareSelect( )
        {
            return new From( QueryContext.EntityType ).As( "a" )
                                   .Where( QueryContext.Names , QueryContext.Values )
                                   .Select( )
                                   .ExecuteQuery( );
        }

        internal object ReturnManyLazy( Type type , bool isGeneric )
        {
            if ( isGeneric )
            {
                Type genericType = typeof( IList<> ).MakeGenericType( new Type[] { type } );
                Type returnType = ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor( genericType );

                return ConcreteCollectionDiscovery.MakeInstance( returnType , new LazyCollectionHandler( Interpret( QueryContext ) ) , type );
            }
            else
            {
                LazyCollectionHandler lazyHandler = new LazyCollectionHandler( Interpret( QueryContext ) );
                return new FrameworkList( lazyHandler , QueryContext.EntityType );
            }
        }

        internal IList ReturnManyByRange( int StartRange , int EndRange , QueryContext context )
        {
            context.StartRange = StartRange;
            context.EndRange = EndRange;

            context.DataReader = Interpret( context ).DataReader;

            return LoadMany( context );
        }

        internal IList ReturnMany( QueryContext context )
        {
            context.DataReader = Interpret( context ).DataReader;

            return LoadMany( context );
        }

        internal int Count( QueryContext context )
        {
            return (int) Interpret( context ).ScalarResult;
        }

        public static IList LoadMany( QueryContext context )
        {
            IList entities = new ArrayList( );

            while ( context.DataReader.Read( ) )
            {
                object entity = new object( );
                string cacheKey = KeyGenerator.GetKey( context.DataReader , context.MetaStructure.Type );
                CacheService persistentChache = CacheService.GetInstance( );

                if ( persistentChache.Contains( cacheKey ) )
                {
                    entities.Add( persistentChache[cacheKey] );
                    continue;
                }

                entity = LoadGhost( context.MetaStructure , context.DataReader );

                ICacheObject cached = persistentChache.Add( cacheKey , context.MetaStructure.Type , entity );

                MetaDataManager.LoadMultipleEntities( entity , context.DataReader , context.MetaStructure );

                entities.Add( entity );
                cached.AddPropertyChangeListener( );
            }

            context.DataReader.Close( ); //Figure out another form of closing
            return entities;
        }

        private static object LoadGhost( EntityMetadata metaData , IDataReader dataReader )
        {
            return GhostGenerator.LoadGhost( metaData , dataReader );
        }

        internal IDataReader ReturnDataReader( QueryContext context )
        {
            return Interpret( context ).DataReader;
        }

        internal object ReturnSingle( QueryContext queryContext )
        {
            IDataReader result = Interpret( queryContext ).DataReader;

            if ( result.IsNull( ) )
                return Null.NOTHING;

            object entity = Null.NOTHING;

            while ( result.Read( ) )
            {
                entity = LoadGhost( queryContext.MetaStructure , queryContext.DataReader );
                MetaDataManager.LoadSingleEntityWithoutRead( ref entity , result );

                break;
            }

            QueryContext.DataReader.Close( ); //Figure out another form of closing

            return entity;
        }

        internal object GetEntityByEntitiyAllProperties( object entity , Type type )
        {
            EntityMetadata metadata = MetaDataManager.PrepareMetadata( type );
            QueryContext context = new QueryContext( QueryType.Select );
            context.MetaStructure = metadata;
            context.EntityType = type;
            context = new From( type ).As( "a" ).
                            WhereEntity( entity ).
                            OrderByDescending( ArrayUtil.SuffixWith( "," , MetaDataManager.GetUniqueKeyNames( type ) , true ) ).
                            Select( ).
                            FirstInternal( );

            return SelectExpression.ReturnSingle( type , context.DataReader , true );
        }

        internal static object ReturnSingle( Type type , IDataReader result , bool canMoveNext )
        {
            if ( result.IsNull( ) )
                return Null.NOTHING;

            object entity = GhostGenerator.CreateGhostInstance( type , null );

            if ( canMoveNext )
            {
                MetaDataManager.LoadSingleEntity( ref entity , result );
            }
            else
            {
                MetaDataManager.LoadSingleEntityWithoutRead( ref entity , result );
            }

            GhostGenerator.UpdateLazyHandlers( entity , MetaDataManager.GetUniqueKeys( entity ) , type );

            return entity;
        }

        internal static object ReturnSingle( Type type , CompositeCriteria criteria , IDataReader dataReader )
        {
            if ( criteria.IsNotNull( ) )
            {
                InternalReader reader = new InternalReader( criteria.GetNameValue( dataReader ) , dataReader.GetSchemaTable( ) );

                while ( reader.Read( ) ) ;

                return ReturnSingle( type , reader as IDataReader , false );
            }

            return ReturnSingle( type , dataReader as IDataReader , false );
        }
    }
}
