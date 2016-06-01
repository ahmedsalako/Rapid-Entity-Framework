using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using PersistentManager.Query.Sql;
using PersistentManager.Query;
using System.Data;
using System.Collections.Specialized;
using PersistentManager.GAC;

namespace PersistentManager.Provider
{
    internal class Db2DatabaseProvider : DatabaseProvider
    {
        internal Db2DatabaseProvider( IDbConnection connection , IDbTransaction transaction )
            : base( connection , transaction )
		{

		}

        public override DbDataReader Range( SQLTokenizer tokens , int StartRange , int Endrange , string query , List<DbParameter> parameters )
        {
            string ALIAS = tokens.EntityALIAS;
            string schema = tokens.EntityMetadata.SchemaName;

            string orderDirection = tokens.OrderByDirection( ).IsNullOrEmpty( ) ? " ASC " : tokens.OrderByDirection( );
            int pageSize = ( Endrange - StartRange ) == 0 ? 1 : ( Endrange - StartRange );

            StringBuilder RangeBuilder = new StringBuilder( );
            RangeBuilder.AppendFormat( " SELECT {0} FROM ( " , tokens.GetSelectArguments( "mainx" ).ElementsToString(",") );                        

            if ( ! tokens.HasOrderBy )
            {
                tokens.Add( tokens.EntityMetadata.Keys , ALIAS , QueryPart.ORDERBY );
            }

            tokens.Add( QueryPart.SELECT_HEADER , string.Format( " ROW_NUMBER() OVER ( {0} ) AS rownum , " , Dialect.OrderByMethod( tokens.sqlAssembler.orderByClause , orderDirection ) ) );
            RangeBuilder.AppendFormat( tokens.ToString( ) );
            RangeBuilder.AppendFormat( ") mainx WHERE mainx.rownum between {0} and {1} " , pageSize , Endrange );

            return GetDataReader( RangeBuilder.ToString( ) , parameters );
            
        }

        public override SQLTokenizer GetFilteredQuerySyntax( SQLTokenizer Tokens , int index )
        {
            Tokens.AddFormatted( QueryPart.Appender , string.Format( " FETCH FIRST {0} ROWS ONLY OPTIMIZE FOR {0} ROWS " , index ) );

            return Tokens;
        }

        public override int ExecuteUpdate( string query , ref DbParameter returnedValue , string columnName , List<DbParameter> parameters )
        {
            StringBuilder sqlStatement = new StringBuilder( );
            sqlStatement.AppendFormat( " SELECT {0} FROM FINAL TABLE ( {1} ) " , columnName , query );

            parameters.Add( returnedValue );

            return ExecuteUpdate( sqlStatement.ToString() , parameters );
        }

        public override string GetNamingStrategyString( )
        {
            return "{0}";
        }

        public override bool SupportsMultipleActiveReader
        {
            get
            {
                return true;
            }
        }

        public override string GetParameterPrefix( )
        {
            return "@";
        }

        public override DbParameter GetCommandParameter( string name , object value )
        {
            return ( DbParameter )GacLoader.GetParameter( new Guid( EmbeddedProviderGUID.DB2_GUID ) , name , value , false );
        }
    }
}
