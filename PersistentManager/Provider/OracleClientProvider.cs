using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.OracleClient;
using PersistentManager.Query.Sql;
using PersistentManager.Query;

namespace PersistentManager.Provider
{
    internal class OracleClientProvider : DatabaseProvider
    {
        internal OracleClientProvider() : base(null, null)
		{

		}

        internal OracleClientProvider(DbTransaction transaction, DbConnection connection) : base(connection, transaction)
        {

        }

        public override string GetNamingStrategyString( )
        {
            return "\"{0}\"";
        }
        public override string GetParameterPrefix( )
        {
            return ":";
        }

        public override int ExecuteUpdate( string query , ref DbParameter returnedValue , string columnName , List<DbParameter> parameterCollection )
        {
            string namedParameter = returnedValue.ParameterName;
            parameterCollection.Add(returnedValue);
            query = string.Concat(query, string.Format(" returning \"{0}\" into {1} ", columnName, namedParameter));

            return ExecuteUpdate(query, parameterCollection);
        }

        public override SQLTokenizer GetFilteredQuerySyntax( SQLTokenizer Tokens , int index )
        {
            Tokens.AddFormatted( QueryPart.WHERE , string.Format(" ROWNUM = {0} ", index ) );
            Tokens.HasWhereClause = true;

            return Tokens;
        }

        public override DbDataReader Range( SQLTokenizer tokens , int StartRange , int Endrange , string query , List<DbParameter> parameters )
        {
            StringBuilder rangeQuery = new StringBuilder();
            rangeQuery.Append(" SELECT * ");
            rangeQuery.Append(" FROM ( SELECT aora.*, rownum rnum ");
            rangeQuery.AppendFormat(" FROM ( {0} ) aora", query);
            rangeQuery.AppendFormat(" WHERE rownum <= {0} ) ", Endrange );
            rangeQuery.AppendFormat(" WHERE rnum >= {0} ", StartRange);

            return GetDataReader( rangeQuery.ToString() , parameters );
        }

        public override DbParameter GetCommandParameter( string name , object value )
        {
            return new OracleParameter( name , value );
        }
    }
}
