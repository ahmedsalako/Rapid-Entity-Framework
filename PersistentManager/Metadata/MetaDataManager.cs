using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;
using PersistentManager.Mapping;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;
using PersistentManager.Util;
using System.Linq;
using PersistentManager.Cache;
using System.Data;

namespace PersistentManager
{
    internal class MetaDataManager
    {
        static object lockable = new object( );
        internal static IDictionary<string , EntityMetadata> TypeBag = new Dictionary<string , EntityMetadata>( );

        private static bool TypeIsInBag( string typeName )
        {
            return ( TypeBag.ContainsKey( typeName ) );
        }

        internal static EntityMetadata MetaInfo( Type entityType )
        {
            if (TypeBag.ContainsKey(entityType.Name))
                return TypeBag[entityType.Name];


            return  PrepareMetadata( entityType );
        }

        internal static EntityMetadata PrepareMetadata( Type type )
        {
            if ( !TypeIsInBag( type.Name ) )
            {
                EntityMetadata metadata = MappingBase.Introspect( type );

                if (null != metadata)
                {

                    MetadataValidation.ValidateMetadata(metadata);

                    lock (lockable)
                    {
                        if (!TypeIsInBag(type.Name))
                        {
                            TypeBag.Add(type.Name, metadata);
                        }
                    }

                    MappingBase.DeriveJoinKeysForRelatedProperties(type, metadata);
                }

                return metadata;
            }

            return TypeBag[type.Name];
        }

        internal static object LoadIntoEntity( EntityMetadata metadata , object entity , IDataReader dataReader )
        {
            foreach ( PropertyMetadata column in metadata.ColumnInfoBag.Where( c => c.IsImported != true && !c.IsDiscriminator == true && c.MappingName.IsNotNull( ) ) )
            {
                PropertyInfo property = entity.GetType( ).GetProperty( column.ClassDefinationName );
                object value = dataReader[column.MappingName];

                if ( !property.IsNull( ) && property.CanWrite )
                {
                    if ( !( value is System.DBNull ) )
                    {
                        value = DataType.ConvertValue( property.PropertyType , value );
                        SetPropertyValue( property , entity , value );
                    }
                }
            }

            LoadPlaceHolders( entity , metadata , dataReader );
            LoadEmbedded( entity , metadata , dataReader );

            return entity;
        }

        internal static void LoadSingleEntityWithoutRead( ref object entity , IDataReader dataReader )
        {
            EntityMetadata metadata = PrepareMetadata( entity.GetType( ) );
            string cacheKey = KeyGenerator.GetKey( dataReader , metadata.Type );
            CacheService persistentChache = CacheService.GetInstance( );

            if ( persistentChache.Contains( cacheKey ) )
            {
                entity = persistentChache[cacheKey];
                return;
            }

            foreach ( PropertyMetadata column in metadata.ColumnInfoBag.Where( c => c.IsImported != true && !c.IsDiscriminator == true && c.MappingName.IsNotNull( ) ) )
            {
                PropertyInfo property = entity.GetType( ).GetProperty( column.ClassDefinationName );
                object value = dataReader[column.MappingName];

                if ( !property.IsNull( ) && property.CanWrite )
                {
                    if ( !( value is System.DBNull ) )
                    {
                        value = DataType.ConvertValue( property.PropertyType , value );
                        property.SetValue( entity , value , null );
                    }
                }
            }

            LoadPlaceHolders( entity , metadata , dataReader );
            LoadBase( entity , metadata , dataReader );
            LoadEmbedded( entity , metadata , dataReader );
            SetIsLoaded( ref entity );

            ICacheObject cached = persistentChache.Add( cacheKey , metadata.Type , entity );
            cached.AddPropertyChangeListener( );
        }

        private static void LoadPlaceHolders( object entity , EntityMetadata metadata , IDataReader dataReader )
        {
            foreach ( PropertyMetadata property in metadata.Placeholders )
            {
                object placeholder = MakeInstance( property.PropertyType );

                foreach ( PropertyMetadata reference in metadata.GetAllIncludeBase( ).Where( p => p.CompositeId == p.CompositeId && p.IsCompositional ) )
                {
                    MetaDataManager.SetPropertyValue( reference.ClassDefinationName , placeholder , dataReader[reference.MappingName] );
                }

                MetaDataManager.SetPropertyValue( property.ClassDefinationName , entity , placeholder );
            }
        }

        internal static object LoadBase( object entity , EntityMetadata classMetadata , IDataReader dataReader )
        {
            if ( classMetadata.HasBaseEntity )
            {
                LoadIntoEntity( EntityMetadata.GetMappingInfo( classMetadata.BaseEntity ) , entity , dataReader );

                return LoadBase( entity , EntityMetadata.GetMappingInfo( classMetadata.BaseEntity ) , dataReader );
            }

            return entity;
        }

        internal static object LoadEmbedded( object entity , EntityMetadata classMetadata , IDataReader dataReader )
        {
            if ( classMetadata.HasEmbeddedEntities )
            {
                foreach ( var embedded in classMetadata.EmbeddedTypes )
                {
                    return LoadIntoEntity( EntityMetadata.GetMappingInfo( embedded.Type ) , entity , dataReader );
                }
            }

            return entity;
        }

        internal static void LoadSingleEntity( ref object entity , IDataReader dataReader )
        {
            EntityMetadata metadata = PrepareMetadata( entity.GetType( ) );

            while ( dataReader.Read( ) )
            {
                string cacheKey = KeyGenerator.GetKey( dataReader , metadata.Type );
                CacheService persistentChache = CacheService.GetInstance( );

                if ( persistentChache.Contains( cacheKey ) )
                {
                    entity = persistentChache[cacheKey];
                    break;
                }

                foreach ( PropertyMetadata column in metadata.ColumnInfoBag.Where( c => c.IsImported != true && !c.IsDiscriminator == true && c.MappingName.IsNotNull( ) ) )
                {
                    PropertyInfo property = entity.GetType( ).GetProperty( column.ClassDefinationName );
                    object value = dataReader[column.MappingName];
                    if ( !property.IsNull( ) && property.CanWrite )
                    {
                        if ( !( value is System.DBNull ) )
                        {
                            value = DataType.ConvertValue( property.PropertyType , value );
                            property.SetValue( entity , value , null );
                        }
                    }
                }

                LoadPlaceHolders( entity , metadata , dataReader );
                LoadBase( entity , metadata , dataReader );
                LoadEmbedded( entity , metadata , dataReader );
                SetIsLoaded( ref entity );
                ICacheObject cached = persistentChache.Add( cacheKey , metadata.Type , entity );
                cached.AddPropertyChangeListener( );
                break;
            }
        }

        internal static void LoadMultipleEntities( object entity , IDataReader dataReader , EntityMetadata metadata )
        {
            foreach ( PropertyMetadata column in metadata.ColumnInfoBag.Where( c => c.IsImported != true && !c.IsDiscriminator == true && c.MappingName.IsNotNull( ) ) )
            {
                PropertyInfo property = entity.GetType( ).GetProperty( column.ClassDefinationName );
                object value = dataReader[column.MappingName];

                if ( !property.IsNull( ) && property.CanWrite )
                {
                    if ( !( value is System.DBNull ) )
                    {
                        value = DataType.ConvertValue( property.PropertyType , value );
                        property.SetValue( entity , value , null );
                    }
                }
            }

            LoadPlaceHolders( entity , metadata , dataReader );
            LoadBase( entity , metadata , dataReader );
            LoadEmbedded( entity , metadata , dataReader );
            SetIsLoaded( ref entity );
        }

        internal static Type GetEntityType( object entity )
        {
            return GhostGenerator.GetEntityType( entity );
        }

        internal static void SetIsLoaded( ref object entity )
        {
            GhostGenerator.SetLoadState( ref entity , true );
        }

        internal static bool IsEntityLoaded( object entity )
        {
            return GhostGenerator.EntityIsLoaded( entity );
        }

        internal static T GetDefault<T>( T type )
        {
            return default( T );
        }

        internal static bool IsLazyLoadableProperty( PropertyInfo propertyInfo )
        {
            foreach ( PropertyMetadata propertyMapping in MetaInfo( propertyInfo.DeclaringType ).GetAll( ) )
            {
                if ( propertyMapping.IsMapped( propertyInfo ) && propertyMapping.IsRelationshipMapping )
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsNotPersistent( PropertyInfo propertyInfo )
        {
            return !IsPersistent( propertyInfo );
        }

        internal static bool IsPersistent( PropertyInfo propertyInfo )
        {
            EntityMetadata metadata = MetaInfo( propertyInfo.DeclaringType );

            if ( metadata == null ) return false;

            return metadata.GetPropertyMappingIncludeBase( propertyInfo.Name ).IsNotNull();
        }

        internal static bool IsEntityField( PropertyInfo propertyInfo )
        {
            if ( IsNotPersistent( propertyInfo ) )
                return false;

            PropertyMetadata property = MetaInfo( propertyInfo.DeclaringType ).PropertyMapping( propertyInfo );

            if ( property.IsNull( ) ) return false;

            return !property.IsRelationshipMapping;
        }

        internal static Dictionary<string , object> GetUniqueKeys( Type type , object entity )
        {
            Dictionary<string , object> uniqueKeys = new Dictionary<string , object>( );

            foreach ( PropertyMetadata mapping in MetaInfo( type ).Keys )
            {
                if ( mapping.IsCompositional )
                {
                    PropertyMetadata placeholder = mapping.DeclaringType.GetMetataData( )
                                                        .Placeholders.FirstOrDefault
                                                        ( p => p.IsPlaceHolding && p.CompositeId == mapping.CompositeId );

                    object compositeKey = GetPropertyValue( placeholder.ClassDefinationName , entity );
                    uniqueKeys.Add( mapping.ClassDefinationName , GetPropertyValue( mapping.ClassDefinationName , compositeKey ) );
                }
                else
                {
                    uniqueKeys.Add( mapping.ClassDefinationName , GetPropertyValue( mapping.ClassDefinationName , entity ) );
                }
            }

            return uniqueKeys;
        }

        internal static object[] GetUniqueKeys( object objectInstance )
        {
            return GetUniqueKeys( objectInstance.GetType( ) , objectInstance ).Values.ToArray( );
        }

        internal static string[] GetUniqueKeyNames(Type type)
        {
            return MetaInfo(type).Keys.Select(k => k.MappingName).ToArray();
        }

        internal static void SetUniqueKey( Type type , string name , object value , object objectInstance )
        {
            foreach ( PropertyMetadata mapping in MetaInfo( type ).Keys )
            {
                if ( mapping.IsCompositional && mapping.MappingName == name )
                {
                    PropertyMetadata placeholder = mapping.DeclaringType.GetMetataData( )
                                                        .Placeholders.FirstOrDefault
                                                        ( p => p.IsPlaceHolding && p.CompositeId == mapping.CompositeId );

                    object compositeKey = GetPropertyValue( placeholder.ClassDefinationName , objectInstance );
                    SetPropertyValue( mapping.ClassDefinationName , compositeKey , value );
                }
                else if( mapping.MappingName == name )
                {
                    SetPropertyValue( mapping.ClassDefinationName , objectInstance , value );
                }
            }
        }

        internal static object MakeInstance( Type type , params object[] parameters )
        {
            if ( null == parameters )
                return Activator.CreateInstance( type );

            return Activator.CreateInstance( type , parameters );
        }

        internal static bool KeyIsNotValid( object[] keys )
        {
            foreach ( object key in keys )
            {
                if ( key is string )
                {
                    if ( !String.IsNullOrEmpty( ( string ) key ) )
                        return false;
                }
                else if ( DataType.IsValueType( key ) )
                {
                    if ( !DataType.ValueEquals( key , 0 ) )
                    {
                        return false;
                    }
                    else if ( !DataType.ValueEquals( key , Guid.Empty ) )
                    {
                        return false;
                    }
                }
                else if ( !key.IsNull( ) )
                    return false;
            }

            return true;
        }        

        internal static object GetPropertyValue( string propertyName , object entity )
        {
            Type type = entity.GetType( );
            BindingFlags flags = BindingFlags.GetProperty | BindingFlags.GetField;
            Binder binder = null;
            object[] args = null;
            object value;

            try
            {
                value = type.InvokeMember( propertyName , flags , binder , entity , args );
            }
            catch ( Exception ex )
            {
                value = null;
            }

            return value;
        }

        internal static object GetFieldValue( string filedName , object entity )
        {
            Type type = entity.GetType( );
            BindingFlags flags = BindingFlags.GetField;
            Binder binder = null;
            object[] args = null;
            object value;

            try
            {
                value = type.InvokeMember( filedName , flags , binder , entity , args );
            }
            catch ( Exception ex )
            {
                value = null;
            }

            return value;
        }

        internal static void SetFieldValue( string propertyName , object entity , params object[] args )
        {
            Type type = entity.GetType( );
            Binder binder = null;

            type.InvokeMember( propertyName , BindingFlags.SetField , binder , entity , args );
        }

        internal static void SetPropertyValue( string propertyName , object entity , params object[] args )
        {
            Type type = entity.GetType( );
            Binder binder = null;

            type.InvokeMember( propertyName , BindingFlags.SetProperty , binder , entity , args );
        }

        internal static void SetPropertyValue( PropertyInfo property , object entity , object value )
        {
            if ( property.CanWrite )
            {
                property.SetValue( entity , value , null );
            }
        }

        internal static string GetSchemaName( Type type )
        {
            return MetaInfo( type ).SchemaName;
        }

        internal static bool IsPersistentable( Type type )
        {
            if ( !type.IsClassOrInterface( ) )
                return false;

            return MetaInfo( type ).IsNotNull( );
        }

        internal static bool HasPersistentProperty( Type type )
        {
            return ( type.GetProperties( ).Count( p => IsEntityField( p ) ) > 0 );
        }

        internal static bool IsSublassOf( Type child , Type parent )
        {
            return parent.IsAssignableFrom( child );
        }

        internal static bool IsUniqueIdentifier( string propertyName , Type type )
        {
            try
            {
                return MetaInfo( type ).PropertyMapping( type.GetProperty( propertyName ) ).IsUniqueIdentifier;
            }
            catch ( Exception x )
            {
                return false;
            }
        }
    }
}
