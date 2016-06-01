using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Initializers.Interfaces;
using System.Collections;
using System.Data.Common;
using System.IO;
using PersistentManager.Cache;
using System.Reflection;
using System.Threading;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using PersistentManager.Query;
using PersistentManager.Query.Sql;
using PersistentManager.Ghosting;
using PersistentManager.Query.Keywords;
using PersistentManager;
using System.Data;
using System.Linq;
using PersistentManager.Metadata;

namespace PersistentManager.Initializers
{
    public class ManyToManyLazyHandler : Lazy , ILazyLoader
    {
        private GetAllMultipleCriteria GetAllMultipleCriteriaHandler;
        private Type lazyType;
        private string joinTable;
        private Type joinTableType;
        PropertyInfo ownerProperty;
        int childCount = 0;

        object frameWorkList = null;
        Type ownerType;

        public ManyToManyLazyHandler( Type entityType , PropertyInfo property , PropertyMetadata propertyMetadata , object[] entityKeys )
            : base( property.PropertyType )
        {
            this.joinTableType = propertyMetadata.JoinTableType;
            this.lazyType = propertyMetadata.RelationType;
            this.joinTable = propertyMetadata.JoinTable;
            this.propertyMetadata = propertyMetadata;
            this.OwnerKey = entityKeys;
            this.ownerProperty = property;
            this.ownerType = entityType;
        }

        public object[] OwnerKey { get; set; }

        public object[] ReferencedKey
        {
            get
            {
                return GetReferencedKeys( ).ToArray( );
            }
        }

        private IEnumerable<object> GetReferencedKeys( )
        {
            foreach ( JoinMetadata join in propertyMetadata.JoinDetails )
            {
                int keyIndex = EntityMetadata.GetMappingInfo( ownerType )
                    .GetKeyIndexByMapping( join.LeftKey );

                yield return OwnerKey[keyIndex];
            }
        }

        public object[] ReferencedRightKey( object[] keys )
        {
            List<object> values = new List<object>( );
            foreach ( JoinMetadata join in propertyMetadata.JoinDetails )
            {
                int keyIndex = EntityMetadata.GetMappingInfo( lazyType )
                    .GetKeyIndexByMapping( join.RightKey );

                values.Add( keys[keyIndex] );
            }

            return values.ToArray( );
        }

        #region //Public Members

        public object GetAllType( )
        {
            if ( IsPersisted )
            {
                GetAllMultipleCriteriaHandler = new GetAllMultipleCriteria( GetAllWithDataReader );
                return GetAllMultipleCriteriaHandler.Invoke( lazyType , joinTable , propertyMetadata.JoinColumns , propertyMetadata.OwningColumns , OwnerKey );
            }
            else
            {
                return Orphans;
            }
        }

        object ILazyLoader.GetType( )
        {
            return null;
        }

        public bool RemoveChild( object child )
        {
            if ( IsPersisted )
            {
                if ( Contains( child ) )
                {

                    RemoveChildEntityExplicit( child );

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

        private void RemoveChildEntityExplicit( object child )
        {
            SessionRuntime runtimeManager = SessionRuntime.GetInstance( );
            object[] keys = MetaDataManager.GetUniqueKeys( child );

            string[] criteriaNames = ArrayUtil.JoinArray( propertyMetadata.OwningColumns , JoinColumns );
            object[] criteriaValues = ArrayUtil.JoinArray( ReferencedKey , ReferencedRightKey( keys ) );

            runtimeManager.RemoveEntityByFieldName( joinTableType , criteriaNames , criteriaValues );
            runtimeManager.RemoveEntity( lazyType , child );
        }

        public void RemoveAllChildren( )
        {
            if ( IsPersisted )
            {
                foreach ( object entity in GetEntityReader<object>() )
                {
                    RemoveChildEntityExplicit( entity );
                }
            }

            ClearOrphans( );
        }

        public override void PersistChildObject( object child )
        {
            if ( IsPersisted )
            {
                if ( !MetaDataManager.IsEntityLoaded( child ) )
                {
                    child = SessionRuntime.GetInstance( ).CreateNewEntityReturnsEntity( child.GetType( ) , child );
                }
                else if ( Contains( child ) )
                {
                    SessionRuntime.GetInstance().SaveEntity(lazyType, child); return;                    
                }

                object[] keys = MetaDataManager.GetUniqueKeys( child );

                object joinTableInstance = MetaDataManager.MakeInstance( joinTableType );
                object[] leftKeys = ReferencedKey;
                object[] rightKeys = ReferencedRightKey( keys );

                foreach ( int index in propertyMetadata.JoinDetails.GetIndices( ) )
                {
                    JoinMetadata join = propertyMetadata.JoinDetails[index];

                    MetaDataManager.SetPropertyValue( join.OwnerColumn , joinTableInstance , leftKeys[index] );
                    MetaDataManager.SetPropertyValue( join.JoinColumn , joinTableInstance , rightKeys[index] );
                }

                SessionRuntime.GetInstance( ).CreateNewWithoutCascade( joinTableType , joinTableInstance );
            }
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
                    return ( int )new From( joinTableType ).As( "a" )
                                                        .Where( "a".Dot( propertyMetadata.OwningColumns ) , ReferencedKey )
                                                        .And( "a".Dot( propertyMetadata.JoinColumns ) , Condition.IsNotNull , null )
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

        public void RemoveAt( int index )
        {
            if ( IsPersisted )
            {
                object entity = GetIndex( index );

                if (OrphanExists(entity))
                {
                    Orphans.Remove(entity);
                }
                else
                {
                    RemoveChildEntityExplicit( entity );
                }
            }
        }

        public object GetIndex( int index )
        {
            if ( IsPersisted )
            {
                int count = CountDataStore;

                if ( count >= ( index + 1 ) )
                {
                    try
                    {
                        IDataReader dataReader = new From( lazyType ).As( "a" )
                                                            .Join( joinTableType , "b" ).On( "a".Dot( propertyMetadata.RightKeys ) ).EqualsTo( "b".Dot( propertyMetadata.JoinColumns ) )
                                                            .Where( "b".Dot( propertyMetadata.OwningColumns ) , ReferencedKey )
                                                            .Select( )
                                                            .TakeInternal( index + 1 ).DataReader;

                        return AddLoaded( SelectExpression.ReturnSingle( lazyType , dataReader , true ) );
                    }
                    catch ( Exception x )
                    {
                        throw new Exception( "Could not locate entity at this index" );
                    }
                }
                else if ( ( index + 1 ) > count )
                {
                    return GetOphanIndex( index , count );
                }
            }

            throw new Exception( "Could not locate entity at this index" );
        }

        public bool ChildExist( object child )
        {
            return Contains( child ) ? true : OrphanExists( child );
        }

        public bool Contains( object child )
        {
            if ( IsPersisted )
            {
                int exist = 0;

                try
                {
                    object[] keys = MetaDataManager.GetUniqueKeys( child );
                    string[] keyNames = MetaDataManager.GetUniqueKeyNames( child.GetType( ) );

                    string a = "a";
                    string b = "b";

                    exist = ( int )new From( ownerType ).As( a )
                                                    .Join( joinTableType , b ).On( a.Dot( propertyMetadata.LeftKeys ) ).EqualsTo( b.Dot( propertyMetadata.OwningColumns ) )
                                                    .Where( a.Dot( propertyMetadata.LeftKeys ) , ReferencedKey )
                                                    .And( b.Dot(  propertyMetadata.JoinColumns ) , Condition.Equals , ReferencedRightKey( keys ) )
                                                    .Select( )
                                                    .Count( );
                }
                catch ( Exception x )
                {
                    exist = 0;
                }

                return ( exist > 0 );
            }

            return false;
        }

        public object GetAllWithDataReader( Type type , string joinType , string[] joinColumn , string[] ownerColumn , object[] value )
        {
            if ( null != frameWorkList )
                return frameWorkList;

            Type returnType = ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor( propertyMetadata.PropertyType );
            frameWorkList = ConcreteCollectionDiscovery.MakeInstance( returnType , this , type );

            return frameWorkList;
        }

        public IDataReader GetDataReader( )
        {
            return new From( lazyType ).As( "a" ).Join( joinTableType , "b" ).On( "a".Dot( propertyMetadata.RightKeys ) ).EqualsTo( "b".Dot( propertyMetadata.JoinColumns ) )
                                            .Where( ArrayUtil.PrefixWith( "b." , propertyMetadata.OwningColumns ) , ReferencedKey )
                                            .And( "b".Dot( JoinColumns ) , Condition.Equals , "a".Dot( propertyMetadata.RightKeys ) )
                                            .Select( )
                                            .ExecuteQuery( )
                                            .DataReader;
        }

        public IEnumerable<T> GetEntityReader<T>( )
        {
            IDataReader dataReader = GetDataReader( );

            while ( dataReader.Read( ) )
            {
                yield return ( T ) AddLoaded( SelectExpression.ReturnSingle( lazyType , dataReader , false ) );
            }

            foreach ( object entity in Orphans )
            {
                yield return ( T )entity;
            }
        }

        #endregion
    }
}
