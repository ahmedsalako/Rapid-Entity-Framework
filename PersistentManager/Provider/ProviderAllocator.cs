using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using PersistentManager.Provider;
using System.Data.OracleClient;
using PersistentManager.Query;
using PersistentManager.GAC;
using PersistentManager.Query.Sql;


namespace PersistentManager
{
    public sealed class ProviderResourceAllocator
    {
        internal static string CreateNamedParameter( Type entity , QueryType queryType , string fieldName )
        {
            IDatabaseProvider provider = SessionRuntime.GetInstance( ).DataBaseProvider;

            if ( fieldName.Contains( provider.GetParameterPrefix( ) ) )
                return fieldName;

            return string.Concat( provider.GetParameterPrefix( ) , CreateName( entity , queryType , fieldName ) );
        }

        internal static string CreateName( Type entityType , QueryType queryType , string commandName )
        {
            string value = new string( Guid.NewGuid( ).ToString( ).Substring( 0 , 5 ).ToCharArray( ) );

            return string.Format( "{0}_{1}" , commandName , value );
        }
    }
}