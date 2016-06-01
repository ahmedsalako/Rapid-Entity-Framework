/**
 *  2005-03-13
 *  @Author : Ahmed Salako
 *  Company : ConsistentSoft
 *  use :  This Component is used to Model the Connection and Database execution
 * */
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using PersistentManager.Provider;
using PersistentManager.Descriptors;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.QueryEngine.Database;
using PersistentManager.Query;
using System.Linq;
using System.Collections.Specialized;

namespace PersistentManager
{
	internal class SqlServerProvider : DatabaseProvider
	{
        internal SqlServerProvider() : base( null , null )
		{

		}

        internal SqlServerProvider(DbTransaction transaction, DbConnection connection) : base(connection, transaction)
        {

        }

        public override string GetNamingStrategyString( )
        {
            return "[{0}]";
        }

        public override string GetParameterPrefix( )
        {
            return "@";
        }

        public override SQLTokenizer GetFilteredQuerySyntax( SQLTokenizer Tokens , int index )
        {
            Tokens.AddFormatted( QueryPart.SELECT_HEADER , string.Format(" TOP {0} ", index ) );

            return Tokens;
        }

        public override int ExecuteUpdate( string query , ref DbParameter returnedValue , string columnName , List<DbParameter> parameterCollection )
        {
            query = string.Concat(query, string.Format("; SET {0} = SCOPE_IDENTITY() ", returnedValue.ParameterName));
            parameterCollection.Add(returnedValue);

            return ExecuteUpdate(query, parameterCollection);
        }

        public override bool SupportsMultipleActiveReader
        {
            get
            {
                if (Connection.ConnectionString.ToUpper().Contains("MULTIPLEACTIVERESULTSETS=TRUE"))
                {
                    return base.SupportsMultipleActiveReader;
                }
                else
                {
                    return false;
                }
            }
        }

        internal void SetRowCount( int rowCount )
        {
            IDbCommand command = CreateCommand( string.Format( " SET ROWCOUNT {0} " , rowCount ) , new List<DbParameter>() );
            command.ExecuteNonQuery( );
        }

        internal void ResetRowCount( )
        {
            IDbCommand command = CreateCommand( string.Format( " SET ROWCOUNT 0 ") , new List<DbParameter>( ) );
            command.ExecuteNonQuery( );
        }

        public override DbDataReader Range( SQLTokenizer tokens , int StartRange , int Endrange , string query , List<DbParameter> parameters )
        {
            try
            {
                string ALIAS = tokens.EntityALIAS;
                string schema = tokens.EntityMetadata.SchemaName;

                string orderDirection = tokens.OrderByDirection( ).IsNullOrEmpty( ) ? " ASC " : tokens.OrderByDirection( ) ;              
                int pageSize = ( Endrange - StartRange ) == 0 ? 1 : ( Endrange - StartRange );

                StringBuilder RangeBuilder = new StringBuilder( );
                RangeBuilder.AppendFormat( " SET ROWCOUNT {0} " , pageSize );
                RangeBuilder.Append( " SELECT outerx.* FROM ( " );                

                tokens.Add( QueryPart.SELECT_HEADER , string.Format( " Top {0} " , Endrange ) );

                if ( tokens.HasOrderBy )
                {
                    RangeBuilder.AppendFormat( "SELECT TOP {0} {1} FROM ( " , pageSize , "mainx.*");
                    tokens.CopyFrom( QueryPart.ORDERBY , QueryPart.SELECT , true );
                }
                else if (!tokens.HasGroupBy)
                {
                    tokens.Add( tokens.EntityMetadata.Keys , ALIAS , QueryPart.ORDERBY );
                    tokens.Add( tokens.EntityMetadata.Keys , ALIAS , QueryPart.SELECT );

                    RangeBuilder.AppendFormat("SELECT TOP {0} {1} FROM ( ", pageSize, "mainx.*");
                }
                else if( tokens.HasGroupBy )
                {
                    RangeBuilder.AppendFormat("SELECT TOP {0} {1} FROM ( ", pageSize, "mainx.*");
                    tokens.CopyFrom( QueryPart.GroupBy , QueryPart.SELECT , true );
                    tokens.CopyFrom(QueryPart.GroupBy, QueryPart.ORDERBY, false);
                }

                RangeBuilder.AppendFormat( tokens.ToString( ) );
                RangeBuilder.AppendFormat( ") mainx " );
                RangeBuilder.AppendFormat( Dialect.OrderByMethod( tokens.GetOrderBys( "mainx" ) , orderDirection.Contains( "ASC" ) ? " DESC " : " ASC " ) );
                RangeBuilder.AppendFormat( ") outerx " );
                RangeBuilder.AppendFormat( " ORDER BY {0} {1} " , tokens.GetOrderBys( "outerx" ).ElementsToString( "," ) , orderDirection );

                return GetDataReader( RangeBuilder.ToString( ) , parameters );
            }
            finally
            {
                ResetRowCount( );
            }
        }

        public override DbParameter GetCommandParameter( string name , object value )
        {
            return new System.Data.SqlClient.SqlParameter( name , value );
        }
    }
}
