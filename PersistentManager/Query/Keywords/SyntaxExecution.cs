using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PersistentManager.Ghosting;
using System.Runtime.CompilerServices;
using System.Data.Common;
using PersistentManager.Query.QueryEngine;
using PersistentManager.Descriptors;
using PersistentManager.Query.Sql;
using PersistentManager.Provider;

namespace PersistentManager.Query.Keywords
{
    public class SyntaxExecution
    {
        internal Type ReturnType { get; set; }
        internal SQLTokenizer tokens;

        internal bool IsScalar { get; set; }

        internal QueryContext QueryContext { get; set; }

        internal StringBuilder QueryString { get; set; }

        internal bool ResultIsCompilerGenerated { get; set; }

        internal bool ResultIsCollection { get; set; }

        internal bool ResultIsEntityClass { get; set; }

        internal Type CompilerGeneratedResultType { get; set; }

        internal List<IDeferedExecution> deferedSelect = new List<IDeferedExecution>( );

        internal List<IDeferedExecution> DeferedSelect
        {
            get { return deferedSelect; }
            set { deferedSelect = value; }
        }

        internal static bool IsCompilerGeneratedType( Type type )
        {
            var compilerGenerateds = type.GetCustomAttributes( typeof( CompilerGeneratedAttribute ) , true );

            return ( compilerGenerateds != null && compilerGenerateds.Length > 0 );
        }

        private DbDataReader ExecuteReader( )
        {
            IDatabaseProvider provider = SessionRuntime.GetInstance().DataBaseProvider;
            ContextData context = new ContextData( (SyntaxContainer)this , QueryContext );
            DataStoreQuery dataStore = DataStoreQuery.GetInstance<RDBMSDataStore>( context , provider );
            tokens = dataStore.ExecuteSelect( false ).SQLTokenizer;
            QueryString = tokens.Tokens;

            return provider.GetDataReader( QueryString.ToString( ) ,
                                    dataStore.ContextData.SQLTokenizer.Parameters );
        }

        private object ExecuteScalar( )
        {
            IDatabaseProvider provider = SessionRuntime.GetInstance( ).DataBaseProvider;
            ContextData context = new ContextData( (SyntaxContainer) this , QueryContext );
            DataStoreQuery dataStore = DataStoreQuery.GetInstance<RDBMSDataStore>( context , provider );
            tokens = dataStore.ExecuteSelect( true ).SQLTokenizer;
            QueryString = tokens.Tokens; 
            return provider.ExecuteScalar( tokens.Tokens.ToString( ) , tokens.Parameters );
        }

        internal QueryContext ExecuteRange( int StartRange , int EndRange )
        {
            IDatabaseProvider provider = SessionRuntime.GetInstance().DataBaseProvider;
            ContextData context = new ContextData( (SyntaxContainer) this , QueryContext );
            DataStoreQuery dataStore = DataStoreQuery.GetInstance<RDBMSDataStore>( context , provider );
            tokens = dataStore.ExecuteSelect( false ).SQLTokenizer;
            QueryString = tokens.Tokens;

            QueryContext.DataReader = provider
                                        .Range( tokens , StartRange , EndRange , QueryString.ToString( ) , dataStore.ContextData.SQLTokenizer.Parameters );

            return QueryContext;
        }

        internal QueryContext ExecuteQuery( )
        {
            if ( this.QueryContext.EndRange > 0 )
            {
                return ExecuteRange( QueryContext.StartRange , QueryContext.EndRange );
            }
            else
            {
                QueryContext.DataReader = ExecuteReader( );
            }
            return QueryContext;
        }

        internal QueryContext ExecuteScalarInternal( )
        {
            QueryContext.ScalarResult = ExecuteScalar( );
            return QueryContext;
        }

        internal IQueryResult SelectResult( )
        {
            if ( QueryContext.EndRange > 0 )
                return new QueryResult( (SyntaxContainer)this , ( DbDataReader )ExecuteRange( QueryContext.StartRange , QueryContext.EndRange ).DataReader , tokens );

            return new QueryResult( (SyntaxContainer)this , ExecuteReader( ) , tokens );
        }

        internal QueryContext ExecuteUpdateInternal( )
        {
            QueryContext.IsUpdated = ExecuteUpdate( );

            return QueryContext;
        }

        internal QueryContext ExecuteCreateStatementWithReturnKey( out object dbgeneratedKey , string autoKeyName )
        {
            SessionRuntime runtime = SessionRuntime.GetInstance( );
            PropertyMetadata column = QueryContext.MetaStructure.ColumnInfoBag.Where( c => c.MappingName == autoKeyName ).First( );

            DbParameter returnParameter = runtime.DataBaseProvider.GetCommandParameter( autoKeyName , MetaDataManager.MakeInstance( column.PropertyType ) );
            returnParameter.ParameterName = ProviderResourceAllocator
                .CreateNamedParameter( QueryContext.EntityType , QueryType.Insert , autoKeyName );
            returnParameter = GetParameterDirection( returnParameter );

            ContextData context = new ContextData( (SyntaxContainer)this , QueryContext );
            DataStoreQuery dataStore = DataStoreQuery.GetInstance<RDBMSDataStore>( context , runtime.DataBaseProvider );
            tokens = dataStore.ExecuteCreate( ).SQLTokenizer;
            QueryString = tokens.Tokens;

            QueryContext.IsUpdated = ExecuteQuery( QueryString.ToString( ) , tokens.Parameters , ref returnParameter , autoKeyName );

            dbgeneratedKey = returnParameter.Value;

            return QueryContext;
        }

        private static DbParameter GetParameterDirection( DbParameter returnParameter )
        {
            try
            {
                returnParameter.Direction = ParameterDirection.Output;
            }
            catch
            {
                return returnParameter;
            }

            return returnParameter;
        }

        internal bool ExecuteUpdate( )
        {
            ContextData context = new ContextData( (SyntaxContainer)this , QueryContext );
            DataStoreQuery dataStore = DataStoreQuery.GetInstance<RDBMSDataStore>( context , SessionRuntime.GetInstance().DataBaseProvider );

            switch ( QueryContext.QueryType )
            {
                case QueryType.Insert:
                    tokens = dataStore.ExecuteCreate( ).SQLTokenizer;
                    tokens.HasUpdatableStatement = true;
                    break;
                case QueryType.Update:
                    tokens = dataStore.ExecuteUpdate( ).SQLTokenizer;
                    break;
                case QueryType.Delete:
                    tokens = dataStore.ExecuteDelete( ).SQLTokenizer;
                    tokens.HasUpdatableStatement = true;
                    break;
                default:
                    return false;
            }

            if ( tokens.HasUpdatableStatement )
            {
                return ExecuteQuery( tokens.Tokens.ToString( ) , tokens.Parameters );
            }

            return true;
        }

        private bool ExecuteQuery( string queryString , List<DbParameter> parameterCollection )
        {
            int returnValue = SessionRuntime.GetInstance( )
                                    .DataBaseProvider
                                    .ExecuteUpdate( queryString , parameterCollection );

            return ( returnValue > 0 );
        }

        private bool ExecuteQuery( string queryString , List<DbParameter> parameterCollection , ref DbParameter parameter , string columnName )
        {
            int returnValue = SessionRuntime.GetInstance( )
                                    .DataBaseProvider
                                    .ExecuteUpdate( queryString , ref parameter , columnName , parameterCollection );

            return ( returnValue > 0 );
        }
    }
}
