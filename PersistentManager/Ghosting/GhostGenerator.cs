using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PersistentManager.Mapping;
using System.Reflection.Emit;
using System.Reflection;
using System.Data.Common;
using System.Threading;
using PersistentManager.Descriptors;
using PersistentManager.Initializers;
using PersistentManager.Initializers.Interfaces;
using PersistentManager.Ghosting;
using PersistentManager.Util;
using System.Data;
using PersistentManager.Cache;
using System.Linq;
using PersistentManager.Contracts;

namespace PersistentManager
{
    public delegate object ReferenceTypeGetter( PropertyInfo propertyInfo );

    internal class GhostGenerator
    {
        internal static ReferenceTypeGetter GetGetter = null;
        static object lockable = new object( );
        static object transaction = new object( );

        internal static object CreateGhostInstance( Type livingType , object[] findByIds )
        {
            Type GhostType = CreateGhostType( livingType , true );

            IDictionary<string , ILazyLoader> dictionary = GetLazyLoadHandlers( livingType , findByIds );

            object ghost = CreateInstance( GhostType , dictionary );
            return DynamicCast.CastReflectively( ghost , livingType );
        }

        internal static Type CreateGhostType( Type livingType )
        {
            return CreateGhostType( livingType , true );
        }

        public static Type CreateGhostType( string tablename , string typename , Embedded[] keys , Embedded[] properties )
        {
            if ( !GhostAssemblyBuilder.HasGhost( typename ) )
            {
                lock ( lockable )
                {
                    if ( !GhostAssemblyBuilder.HasGhost( typename ) )
                    {
                        Ghost.CreateVirtualType( tablename , typename , keys , properties );
                    }
                }
            }

            return GhostAssemblyBuilder.Assembly.GetType( typename );
        }

        private static Type CreateGhostType( Type livingType , bool canDispatchPropertyChanged )
        {
            if ( !GhostAssemblyBuilder.HasGhost( livingType.Name ) )
            {
                lock ( lockable )
                {
                    if ( !GhostAssemblyBuilder.HasGhost( livingType.Name ) )
                    {
                        Ghost.Create( livingType , canDispatchPropertyChanged );
                    }
                }
            }

            return GhostAssemblyBuilder.Assembly.GetType( livingType.Name );
        }

        internal static object CreateTransactionalGhostType( Type livingType , object[] findByIds )
        {
            if ( !GhostAssemblyBuilder.HasGhost( livingType.Name + Constant.GHOST_TRANSACTION_NAME ) )
            {
                lock ( transaction )
                {
                    if ( !GhostAssemblyBuilder.HasGhost( livingType.Name + Constant.GHOST_TRANSACTION_NAME ) )
                    {
                        Ghost.Create( livingType , false );
                    }
                }
            }

            Type GhostType = GhostAssemblyBuilder.Assembly.GetType( livingType.Name + Constant.GHOST_TRANSACTION_NAME );

            IDictionary<string , ILazyLoader> dictionary = GetLazyLoadHandlers( livingType , findByIds );

            object ghost = Activator.CreateInstance( GhostType , dictionary ) as object; //Create and Return the Ghost
            return ghost;
        }

        public static Type GetType( object entity )
        {
            if ( IsAGhost( entity ) ) return entity.GetType( ).BaseType;

            return entity.GetType( );
        }

        public static object CopyState( object entity )
        {
            object copy = CreateGhostInstance( GetType( entity ) , MetaDataManager.GetUniqueKeys( entity ) );

            return CopyState( entity , copy );
        }

        public static object CopyState( object entity , object copy )
        {
            SetLoadState( ref copy , false );

            foreach ( PropertyInfo propertyInfo in entity.GetType( ).GetProperties( ) )
            {
                if ( !MetaDataManager.IsEntityField( propertyInfo ) )
                    continue;

                MetaDataManager.SetPropertyValue
                                ( 
                                    propertyInfo.Name , 
                                    copy , 
                                    MetaDataManager.GetPropertyValue( propertyInfo.Name , entity )
                                 );
            }

            SetLoadState( ref copy , true );

            return copy;
        }

        internal static object LoadGhost( EntityMetadata metaData , IDataReader dataReader )
        {
            object[] keys = new object[metaData.Keys.Count( )];
            int count = 0;

            foreach ( PropertyMetadata property in metaData.Keys )
            {
                keys[count++] = dataReader[property.MappingName] as object;
            }

            return CreateGhostInstance( metaData.Type , keys );
        }

        internal static IDictionary<string , ILazyLoader> GetLazyLoadHandlers( Type entityType , object[] entityKeys )
        {
            IDictionary<string , ILazyLoader> lazyHandlers = new Dictionary<string , ILazyLoader>( );

            foreach ( EntityRelation relation in EntityRelation.DeriveRelations( entityType ) )
            {
                lazyHandlers.Add( relation.Property.Name , EntityRelation.CreateLazyHandlers( relation , entityType , entityKeys ) );
            }

            return lazyHandlers;
        }

        internal static IDictionary<string , ILazyLoader> GetLazyLoadHandlers( object entity )
        {
            if ( IsAGhost( entity ) )
            {
                return ( IDictionary<string , ILazyLoader> )MetaDataManager.GetPropertyValue( "DelegateList" , entity );
            }

            return null;
        }

        internal static object Detach<T>( object entity )
        {
            if ( !typeof( T ).IsInterface )
            {
                return Detach( entity , typeof( T ) );
            }

            return entity;
        }

        internal static object Detach( object owner , Type type )
        {
            return Detach( owner , type , new Dictionary<string , object>( ) );
        }

        private static bool IsContainer( IDictionary<Type , List<Type>> criterias , Type type )
        {
            return ( criterias.ContainsKey( type ) );
        }

        private static bool Contains( IDictionary<Type , List<Type>> criterias , Type type1 , Type type2 )
        {
            if ( !IsContainer( criterias , type1 ) )
                return false;

            return criterias[type1].Contains( type2 );
        }

        private static object CreateInstance( Type entityGhostType , IDictionary<string , ILazyLoader> dictionary )
        {
            ConstructorInfo ctor = entityGhostType.GetConstructor( new[] { typeof( IDictionary<string , ILazyLoader> ) } );

            return ctor.Invoke( new[] { dictionary } );
        }

        internal static object Clone( object owner )
        {
            if ( IsAGhost( owner ) )
            {
                MethodInfo cloneMethod = owner.GetType( ).GetMethod( "RapidClone" );
                return cloneMethod.Invoke( owner , new[] { owner } );
            }

            return owner;
        }

        private static object Detach( object owner , Type type , IDictionary<string , object> objectCache )
        {
            if ( !IsAGhost( owner ) )
                return owner;

            object detached = Clone( owner );

            foreach ( PropertyInfo propertyInfo in owner.GetType( ).GetProperties( ) )
            {
                if ( MetaDataManager.IsLazyLoadableProperty( propertyInfo ) )
                {
                    object value = propertyInfo.GetGetMethod( ).Invoke( owner , null );

                    if ( value is IEnumerable )
                    {
                        Type listType = ConcreteCollectionDiscovery.GetConcreteImplementorWithGenericType( propertyInfo.PropertyType );
                        object listInstance = listType.GetConstructor( Type.EmptyTypes ).Invoke( null );
                        detached.GetType( ).GetProperty( propertyInfo.Name ).GetSetMethod( ).Invoke( detached , new[] { listInstance } );

                        IEnumerator enumerator = ( IEnumerator ) ( ( IEnumerable ) value ).GetEnumerator( );

                        while ( enumerator.MoveNext( ) )
                        {
                            ConcreteCollectionDiscovery.AddElement(
                                    listInstance ,
                                    Clone( enumerator.Current )
                                   );
                        }
                    }
                    else if ( value.IsNotNull( ) )
                    {
                        MetaDataManager.SetPropertyValue( propertyInfo.Name , detached , Clone( value ) );
                    }
                }
            }

            return detached;
        }

        internal static bool IsAGhost( object entity )
        {
            return entity is IGhostableProxy;
        }

        internal static Type GetEntityType( object entity )
        {
            if ( IsAGhost( entity ) )
            {
                return entity.GetType( ).BaseType;
            }
            else
            {
                return entity.GetType( );
            }
        }

        internal static bool IsNotGhost( object entity )
        {
            return !IsAGhost( entity );
        }

        internal static void SetLoadState( ref object entity , bool state )
        {
            if ( IsAGhost( entity ) )
            {
                MetaDataManager.SetFieldValue( "IsLoaded" , entity , state );
            }
        }

        internal static bool IsSelfTracking( object entity )
        {
            if ( IsAGhost( entity ) )
            {
                return MetaDataManager.GetFieldValue( Constant.SELF_TRACKING_LIST_NAME , entity ).IsNotNull( );
            }

            return false;
        }

        private static IDictionary<string , IChangeCatalog> GetChanges( object entity )
        {
            return MetaDataManager.GetFieldValue
                                                  (
                                                    Constant.SELF_TRACKING_LIST_NAME ,
                                                    entity
                                                  ) as IDictionary<string , IChangeCatalog>;
        }

        internal static IDictionary<string , IChangeCatalog> GetSelfChanges( object entity )
        {
            if ( !IsAGhost( entity ) ) return new Dictionary<string , IChangeCatalog>( );
            if ( GetChanges( entity ).IsNull( ) )
            {
                MetaDataManager.SetFieldValue( Constant.SELF_TRACKING_LIST_NAME , entity , new Dictionary<string , IChangeCatalog>( ) );
            }

            return GetChanges( entity );
        }

        internal static void ClearSelfChanges( object entity )
        {
            MetaDataManager.SetFieldValue( Constant.SELF_TRACKING_LIST_NAME , entity , new Dictionary<string , IChangeCatalog>( ) );
        }

        internal static bool EntityIsLoaded( object entity )
        {
            object value = MetaDataManager.GetPropertyValue( "IsLoaded" , entity );

            if ( value.IsNotNull( ) )
            {
                return ( bool ) value;
            }

            return false;
        }

        internal static void UpdateLazyHandlers( object entity , object[] ownerKeys , Type entityType )
        {
            IDictionary<string , ILazyLoader> lazyHandlers = GetLazyLoadHandlers( entity );

            foreach ( ILazyLoader lazy in lazyHandlers.Values )
            {
                if ( lazy is ManyToManyLazyHandler )
                    ( ( ManyToManyLazyHandler ) lazy ).OwnerKey = ownerKeys;
                else if ( lazy is OneToManyLazyHandler )
                    ( ( OneToManyLazyHandler ) lazy ).OwnerKey = ownerKeys;
                else if ( lazy is OneToOneLazyHandler )
                    ( ( OneToOneLazyHandler ) lazy ).OwnerKey = ownerKeys;
            }
        }

        internal static bool IsPersisted( PropertyMetadata columnInfo , IDictionary<string , ILazyLoader> lazyHandlers )
        {
            if ( lazyHandlers.IsNull( ) )
                return false;

            return ( ( Lazy ) lazyHandlers[columnInfo.ClassDefinationName] ).IsPersisted;
        }
    }
}
