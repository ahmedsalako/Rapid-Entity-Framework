using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Services.Interfaces;
using PersistentManager.Cache;
using PersistentManager.Provider;
using System.Data.Common;
using System.Data.OracleClient;
using PersistentManager.GAC;

namespace PersistentManager
{
    public class DefaultConfiguration
    {
        static DefaultConfiguration( )
        {
            ServiceLocator.AddService<ISharedMemoryStore>( new InternalSharedMemory( ) );
        }

        internal void ChangeService<T>( T service )
        {
            ServiceLocator.ChangeService<T>( service );
        }

        internal static IDatabaseProvider GetProviderImplementation( ProviderDialect dialect , DbConnection connection , DbTransaction transaction )
        {
            switch ( dialect )
            {
                case ProviderDialect.SqlProvider:
                    return new SqlServerProvider( transaction , connection );
                case ProviderDialect.OracleProvider:
                    return new OracleClientProvider( transaction , connection );
                case ProviderDialect.OleDbProvider:
                    return new OleDbProvider( transaction , connection );
                case ProviderDialect.MySQLProvider:
                    return new MySqlProvider( transaction , connection );
                case ProviderDialect.Db2Provider:
                    return new Db2DatabaseProvider( connection , transaction );
                case ProviderDialect.SQLLite3:
                    return new SQLLiteDatabaseProvider( connection , transaction );
                default:
                    return new OleDbProvider( transaction , connection );
            }
        }

        internal static DbConnection GetConnectionByDialect( ProviderDialect dialect , string connectionString )
        {
            switch ( dialect )
            {
                case ProviderDialect.SqlProvider:
                    return new System.Data.SqlClient.SqlConnection( connectionString );
                case ProviderDialect.OleDbProvider:
                    return new System.Data.OleDb.OleDbConnection( connectionString );
                case ProviderDialect.OracleProvider:
                    return new OracleConnection( connectionString );
                case ProviderDialect.MySQLProvider:
                    return ( DbConnection )GacLoader.GetConnection( new Guid( EmbeddedProviderGUID.MYSQL_GUID ) , connectionString , true );
                case ProviderDialect.Db2Provider:
                    return ( DbConnection )GacLoader.GetConnection( new Guid( EmbeddedProviderGUID.DB2_GUID ) , connectionString , false );
                case ProviderDialect.SQLLite3:
                    return ( DbConnection )GacLoader.GetConnection( new Guid( EmbeddedProviderGUID.SQLLite_GUID ) , connectionString , false );
                default:
                    return new System.Data.OleDb.OleDbConnection( connectionString );
            }
        }
    }
}
