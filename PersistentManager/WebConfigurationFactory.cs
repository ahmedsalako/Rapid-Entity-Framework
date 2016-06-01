using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using PersistentManager.Runtime;
using PersistentManager.Services.Interfaces;

namespace PersistentManager
{
    public class WebConfigurationFactory : ConfigurationBase , IConfiguration
    {
        private static bool IsInstantiated = false;
        private static string connectionString = string.Empty;
        private ProviderDialect _providerDialect;

        public static WebConfigurationFactory GetInstance( string connectionStr )
        {
            if ( !IsInstantiated )
            {
                connectionString = connectionStr;
                configuration = new WebConfigurationFactory( connectionString );
                IsInstantiated = true;
            }

            return configuration.As<WebConfigurationFactory>( );
        }

        public ProviderDialect ProviderDialect
        {
            get { return _providerDialect; }
            set
            {
                RuntimeRegistry mainRegistry = GetRegistryByName( Constant.DEFAULT_CONNECTION_KEY );

                if ( mainRegistry.IsNotNull( ) )
                    mainRegistry.ProviderDialect = value;

                _providerDialect = value;
            }
        }

        public static WebConfigurationFactory GetInstance( )
        {
            if ( !IsInstantiated )
            {
                configuration = new WebConfigurationFactory( connectionString );
                IsInstantiated = true;
            }

            return configuration.As<WebConfigurationFactory>( );
        }

        private WebConfigurationFactory( string connectionStr )
        {
            RuntimeRegistry registry = RuntimeRegistry.GetRuntimeRegistry( );
            registry.ConnectionString = connectionStr;
            registry.IsMainConnection = true;
            registry.RegistryName = Constant.DEFAULT_CONNECTION_KEY;
            AddConnectionStringToRegistry( registry );
        }

        public DbConnection GetCurrentConnection( )
        {
            return base.GetConnection( Constant.DEFAULT_CONNECTION_KEY );
        }

        public DbConnection GetNamedConnection( string name )
        {
            return base.GetConnection( name );
        }

        public new void DestroyCurrentConnection( )
        {
            base.DestroyCurrentConnection( );
        }

        public new void DestroyNamedConnection( string name )
        {
            base.DestroyNamedConnection( name );
        }

        public void CacheSettings( long stayAliveMinutes , long cacheSize )
        {
            CacheSize = cacheSize;
            CacheDuration = stayAliveMinutes;
        }

        public void Prefetch( params Type[] entities )
        {
            using ( EntityManager manager = new EntityManager( ) )
            {
                manager.OpenDatabaseSession( );

                foreach ( Type entityType in entities )
                {
                    manager.GetAll( entityType );
                }
            }
        }

        public new void Compile( string assemblyName )
        {
            ConfigurationBase.Compile(assemblyName);
        }

        public void RegisterCacheProvider( ISharedMemoryStore storage )
        {
            ChangeService<ISharedMemoryStore>( storage );
        }


        public new RuntimeRegistry GetRegistry( string name )
        {
            return base.GetRegistry( name );
        }
    }
}
