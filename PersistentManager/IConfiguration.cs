using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using PersistentManager.Runtime;
using PersistentManager.Services.Interfaces;

namespace PersistentManager
{
    public interface IConfiguration
    {
        DbConnection GetCurrentConnection( );
        DbConnection GetNamedConnection( string name );
        RuntimeRegistry GetRegistry( string name );

        void DestroyCurrentConnection( );
        void DestroyNamedConnection( string name );
        ProviderDialect ProviderDialect { get; set; }
        TransactionCacheBoundary TransactionCacheBoundary { get; set; }
        void CacheSettings( long maxAge , long cacheSize );
        void RegisterSiblingConnection( string alias , ProviderDialect providerDialect , string connectionString );
        void Compile( string assemblyName );
        void RegisterCacheProvider( ISharedMemoryStore storage );  
    }
}
