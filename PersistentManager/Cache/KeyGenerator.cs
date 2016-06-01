using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using PersistentManager.Runtime;

namespace PersistentManager.Cache
{
    internal class KeyGenerator
    {
        internal static string GetKey( object entity )
        {
            try
            {
                object[] key = MetaDataManager.GetUniqueKeys( entity );
                return MakeEntityKey( entity.GetType( ).Name , GetValue( key ) );
            }
            catch ( Exception x )
            {
                return null;
            }
        }

        private static string GetValue( object[] keys )
        {
            string key = string.Empty;

            foreach ( object name in keys )
            {
                key += name.ToString( );
            }

            return key;
        }

        internal static string GetKeyValues( IDataReader dataReader , Type entity )
        {
            string key = string.Empty;

            foreach ( string name in MetaDataManager.GetUniqueKeyNames( entity ) )
            {
                key += dataReader[name].ToString( );
            }

            return key;
        }

        internal static string GetKey( object entity , Type type )
        {
            object[] key = MetaDataManager.GetUniqueKeys( entity );
            return MakeEntityKey( type.Name , GetValue( key ) );
        }

        internal static string GetKey( IDataReader dataReader , Type entity )
        {
            string key = string.Empty;
            string fullName = entity.Name;

            foreach ( string name in MetaDataManager.GetUniqueKeyNames( entity ) )
            {
                key += dataReader[name].ToString( );
            }

            return MakeEntityKey( fullName , key );
        }

        internal static string GetKey( object[] values , Type entity )
        {
            string key = string.Empty;
            string fullName = entity.Name;

            foreach ( object value in values )
            {
                key += value;
            }

            return MakeEntityKey( fullName , key );
        }

        private static string MakeEntityKey( string entityName , string key )
        {
            SessionRuntime runtime = SessionRuntime.GetInstance( );

            RuntimeRegistry registry = ConfigurationBase.GetRegistryByName( runtime.RuntimeRegistryName );

            return string.Format( Constant.CACHE_KEY_FORMAT , entityName , key , registry.RegistryName );
        }
    }
}
