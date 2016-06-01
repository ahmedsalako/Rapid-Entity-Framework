using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Data.Common;
using System.Data;
using System.Reflection;
using PersistentManager.Runtime;
using PersistentManager.Metadata;
using PersistentManager.Mapping.Events;
using PersistentManager.Services.Interfaces;
using PersistentManager.Cache;
using PersistentManager.Descriptors;

namespace PersistentManager
{
    public abstract class ConfigurationBase : DefaultConfiguration
    {
        private static List<RuntimeRegistry> registry = new List<RuntimeRegistry>( );
        protected static IConfiguration configuration = null;
        private static object globalLock = new object( );
        private static long cacheDuration = 0;
        private static long cacheSize = 0;

        protected DbConnection GetConnection( string registryName )
        {
            DbConnection connection = GetCurrentConnection( registryName );

            if ( connection.IsNull( ) )
            {
                RuntimeRegistry registry = GetRegistryByName( registryName );
                connection = GetConnectionByDialect( registry.ProviderDialect , registry.ConnectionString );
                
                ScopeContext.SetData( registryName , connection );
            }

            return connection;
        }

        internal static void AddConnectionStringToRegistry( RuntimeRegistry runtimeRegistry )
        {
            registry.Add( runtimeRegistry );
        }

        public static RuntimeRegistry GetRegistryByName( string name )
        {
            return registry.Where( r => r.RegistryName == name ).FirstOrDefault( );
        }

        public static RuntimeRegistry GetRegistryByConnectionString( string connectionString )
        {
            return registry.Where( r => r.ConnectionString.Contains( connectionString ) ).FirstOrDefault( );
        }

        public static List<RuntimeRegistry> Registry
        {
            get { return ConfigurationBase.registry; }
            set { ConfigurationBase.registry = value; }
        }

        private DbConnection GetCurrentConnection( string name )
        {
            return ScopeContext.GetData<DbConnection>( name ).As<DbConnection>( );
        }

        internal static Stack<Guid> GetOrCreateScopes( )
        {
            Stack<Guid> scopes = ScopeContext.GetData<Stack<Guid>>( Constant.SESSION_STACK );

            if ( scopes.IsNull( ) )
            {
                scopes = new Stack<Guid>( );
                ScopeContext.SetData( Constant.SESSION_STACK , scopes );
            }

            return scopes;
        }

        internal static void SetCurrentScope( Guid scopeIdentifier )
        {
            Stack<Guid> scopes = GetOrCreateScopes( );
            scopes.Push( scopeIdentifier );
        }

        internal static void RemoveCurrentScope( )
        {
            Stack<Guid> scopes = GetOrCreateScopes( );
            scopes.Pop( );
        }

        internal static Guid GetCurrentScope( )
        {
            Stack<Guid> scopes = GetOrCreateScopes( );

            if (scopes.Count > 0)
            {
                return scopes.Peek();
            }

            return Guid.Empty;
        }

        private void RemoveCurrentConnection( )
        {
            ScopeContext.SetData( Constant.DEFAULT_CONNECTION_KEY , null );
        }

        private void RemoveNamedConnection( string name )
        {
            ScopeContext.SetData( name , null );
        }

        protected void DestroyCurrentConnection( )
        {
            try
            {
                DbConnection connection = GetConnection( Constant.DEFAULT_CONNECTION_KEY );

                if ( connection == null )
                {
                    return;
                }

                if ( connection.State == ConnectionState.Open )
                    connection.Close( );

                RemoveCurrentConnection( );
            }
            catch ( Exception ex )
            {
                throw new Exception( "Could not destroy current connection object" , ex );
            }
        }

        protected void DestroyNamedConnection( string name )
        {
            try
            {
                DbConnection connection = GetConnection( name );

                if ( connection == null )
                {
                    return;
                }

                if ( connection.State == ConnectionState.Open )
                    connection.Close( );

                RemoveNamedConnection( name );
            }
            catch ( Exception ex )
            {
                throw new Exception( "Could not destroy current connection object" , ex );
            }
        }

        internal static IConfiguration GetCurrentConfiguration( )
        {
            return configuration;
        }

        internal static object GlobalLock
        {
            get { return globalLock; }
        }

        public static long CacheDuration
        {
            get { return ConfigurationBase.cacheDuration; }
            set { ConfigurationBase.cacheDuration = ( value <= 0 ) ? cacheDuration : value; }
        }

        public static long CacheSize
        {
            get { return ConfigurationBase.cacheSize; }
            set { ConfigurationBase.cacheSize = ( value <= 0 ) ? cacheSize : value; }
        }

        public RuntimeRegistry GetRegistry( string name )
        {
            if ( name.IsNullOrEmpty( ) )
                return GetRegistryByName( Constant.DEFAULT_CONNECTION_KEY );

            return GetRegistryByName( name );
        }

        public void RegisterSiblingConnection( string alias , ProviderDialect providerDialect , string connectionString )
        {
            if ( GetRegistryByName( alias ).IsNull( ) )
            {
                RuntimeRegistry registry = RuntimeRegistry.GetRuntimeRegistry( );
                registry.RegistryName = alias;
                registry.ProviderDialect = providerDialect;
                registry.ConnectionString = connectionString;
                AddConnectionStringToRegistry( registry );
            }
        }

        internal static void Compile( string assemblyName )
        {
            Assembly metaDataAssembly = Assembly.Load( assemblyName );
            foreach ( Type type in metaDataAssembly.GetTypes( ) )
            {
                EntityMetadata metadata = MetaDataManager.MetaInfo( type );                

                if (metadata != null)
                {
                    if (!type.IsAbstract)
                    {
                        GhostGenerator.CreateGhostType(type);
                    }
                }
            }
        }

        public virtual TransactionCacheBoundary TransactionCacheBoundary { get; set; }

        internal static SessionRuntime CreateSession( string name )
        {
            RuntimeRegistry registry = configuration.GetRegistry( name );
            DbConnection connection = configuration.GetNamedConnection( registry.RegistryName );

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DbTransaction adoTransaction = connection.BeginTransaction();

            IDatabaseProvider provider = GetProviderImplementation
            (
                registry.ProviderDialect,
                connection,
                adoTransaction
             );

            SessionRuntime session = new SessionRuntime(provider, registry.RegistryName);

            return session;
        }
    }
}
