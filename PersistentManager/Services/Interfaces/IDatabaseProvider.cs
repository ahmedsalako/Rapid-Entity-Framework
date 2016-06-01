using System;
using System.Data;
using System.Data.Common;
using System.Text;
namespace PersistentManager
{
    interface IDatabaseProvider : IDiscoverable<IDatabaseProvider>
    {
        IDbTransaction Transaction { get; set; }
        IDbConnection Connection { get; set; }

        void AddParameters( System.Data.IDbCommand command , System.Collections.Generic.List<System.Data.Common.DbParameter> parameters );
        object ExecuteScalar( string query );
        object ExecuteScalar( string query , System.Collections.Generic.List<System.Data.Common.DbParameter> parameterCollection );
        System.Data.IDataReader ExecuteStoredProcedure( string procedureName , System.Collections.Generic.List<System.Data.Common.DbParameter> parameters );
        int ExecuteUpdate( string query , System.Collections.Generic.List<System.Data.Common.DbParameter> parameterCollection );
        int ExecuteUpdate( string query , ref System.Data.Common.DbParameter returnedValue , string columnName , System.Collections.Generic.List<System.Data.Common.DbParameter> parameters );
        int ExecuteUpdate( string query );
        int ExecuteUpdateProcedure( string procedureName , System.Collections.Generic.List<System.Data.Common.DbParameter> parameters );
        System.Data.Common.DbDataReader GetDataReader( string query );
        System.Data.Common.DbDataReader GetDataReader( string query , System.Collections.Generic.List<System.Data.Common.DbParameter> parameterCollection );
        PersistentManager.Query.Sql.SQLTokenizer GetFilteredQuerySyntax( PersistentManager.Query.Sql.SQLTokenizer Tokens , int index );
        string GetNamingStrategyString( );
        string GetParameterPrefix( );
        PersistentManager.Provider.Functions.ProviderFunctions GetProviderFunctions( );
        System.Data.Common.DbDataReader RaiseRegisterDataReaderEvent( System.Data.IDataReader dataReader );
        System.Data.Common.DbDataReader Range( PersistentManager.Query.Sql.SQLTokenizer tokens , int StartRange , int Endrange , string query , System.Collections.Generic.List<System.Data.Common.DbParameter> parameters );
        event PersistentManager.RegisterDataReader RegisterReader;
        bool SupportsMultipleActiveReader { get; }
        DbParameter GetCommandParameter( string name , object value );
        string QueryLog { get; set; }
    }
}
