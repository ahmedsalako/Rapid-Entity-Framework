using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using PersistentManager.Runtime;
using PersistentManager.Services.Interfaces;

namespace PersistentManager
{
    /// <summary>
    /// This class is a singleton class that creates the database connection string once
    /// Should be called once during the application lifetime
    /// </summary>
    public class ConfigurationFactory : ConfigurationBase, IConfiguration
    {

        private static string connectionString = string.Empty;
        private static bool IsInstantiated = false;
        private ProviderDialect _providerDialect;

        public static string ConnectionString
        {
            get { return ConfigurationFactory.connectionString; }
            set 
            {
                RuntimeRegistry mainRegistry = GetRegistryByName(Constant.DEFAULT_CONNECTION_KEY);

                if (mainRegistry.IsNotNull())
                    mainRegistry.ConnectionString = value;

                ConfigurationFactory.connectionString = value; 
            }
        }

        [Obsolete( "Will be removed next release!. Use parameterless EntityManager" )]
        public static ConfigurationFactory GetInstance( )
        {
            if ( !IsInstantiated )
            {
                configuration = new ConfigurationFactory( connectionString );
                IsInstantiated = true;
            }

            return configuration.As<ConfigurationFactory>( );
        }

        public static ConfigurationFactory GetInstance(string connectionStr)
        {
            if (!IsInstantiated)
            {                
                configuration = new ConfigurationFactory(connectionStr);
                IsInstantiated = true;
            }

            return configuration.As<ConfigurationFactory>();
        }

        private ConfigurationFactory(string connectionStr)
        {
            RuntimeRegistry registry = RuntimeRegistry.GetRuntimeRegistry();
            registry.RegistryName = Constant.DEFAULT_CONNECTION_KEY;
            registry.ConnectionString = connectionStr;
            registry.IsMainConnection = true;            
            AddConnectionStringToRegistry(registry);
        }

        public ProviderDialect ProviderDialect
        {
            get { return _providerDialect; }
            set 
            {
                RuntimeRegistry mainRegistry = GetRegistryByName(Constant.DEFAULT_CONNECTION_KEY);

                if (mainRegistry.IsNotNull())
                    mainRegistry.ProviderDialect = value;

                _providerDialect = value;
            }
        }        

        public DbConnection GetNamedConnection(string name)
        {
            return base.GetConnection(name);
        }

        public DbConnection GetCurrentConnection()
        {
            return base.GetConnection(Constant.DEFAULT_CONNECTION_KEY);
        }

        public new void DestroyCurrentConnection()
        {
            base.DestroyCurrentConnection();
        }

        public new void DestroyNamedConnection(string name)
        {
            base.DestroyNamedConnection(name);
        }

        public void CacheSettings(long stayAliveMinutes, long cacheSize)
        {
            CacheSize = cacheSize;
            CacheDuration = stayAliveMinutes;
        }

        public void Prefetch(params Type[] entities)
        {
            using (IEntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                foreach (Type entityType in entities)
                {
                    manager.GetAll(entityType);
                }
            }
        }

        public new void Compile(string assemblyName)
        {
            ConfigurationBase.Compile(assemblyName);
        }

        public void RegisterCacheProvider( ISharedMemoryStore storage )
        {
            ChangeService<ISharedMemoryStore>( storage );
        }

        public new RuntimeRegistry GetRegistry(string name)
        {
            return base.GetRegistry(name);
        }
    }
}
