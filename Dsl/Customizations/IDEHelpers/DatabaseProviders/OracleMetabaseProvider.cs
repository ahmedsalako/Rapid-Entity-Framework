using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;
using consist.RapidEntity.Customizations.Extensions;
using consist.RapidEntity.Util;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public class OracleMetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType , MetabaseTypeMapping> typeMappings = new Dictionary<DbType , MetabaseTypeMapping>( );

        static OracleMetabaseProvider( )
        {
            typeMappings.Add( DbType.Binary , CreateMapping( "BLOB" , 0 , 0 , "byte[]" ) );
            typeMappings.Add( DbType.Boolean , CreateMapping( "NUMBER" , 1 , 0 , "System.Boolean" ) );
            typeMappings.Add( DbType.Byte , CreateMapping( "NUMBER" , 3 , 0 , "System.Byte" ) );
            typeMappings.Add( DbType.SByte , CreateMapping( "NUMBER" , 3 , 0 , "System.SByte" ) );
            typeMappings.Add( DbType.Decimal , CreateMapping( "NUMBER" , 19 , 5 , "System.Decimal" ) );
            typeMappings.Add( DbType.Double , CreateMapping( "DOUBLE PRECISION" , 0 , 0 , "System.Double" ) );
            typeMappings.Add( DbType.Guid , CreateMapping( "CLOB" , 0 , 0 , "System.Guid" ) );
            typeMappings.Add( DbType.Int32 , CreateMapping( "NUMBER" , 10 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.Int64 , CreateMapping( "NUMBER" , 20 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.UInt32 , CreateMapping( "NUMBER" , 10 , 0 , "int" ) );
            typeMappings.Add( DbType.UInt64 , CreateMapping( "NUMBER" , 20 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.String , CreateMapping( "VARCHAR2" , 2000 , -1 , "System.String" ) );
            typeMappings.Add( DbType.DateTime , CreateMapping( "TIMESTAMP" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Date , CreateMapping( "DATE" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Int16 , CreateMapping( "NUMBER" , 5 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.Single , CreateMapping( "DOUBLE PRECISION" , 0 , 0 , "System.Double" ) );
            typeMappings.Add( DbType.UInt16 , CreateMapping( "NUMBER" , 5 , 0 , "System.Single" ) );
        }

        internal OracleMetabaseProvider( DbConnection connection )
        {
            Connection = connection;
        }

        public override DataTable ExecuteForeignKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            StringBuilder foreignKeyQuery = new StringBuilder( );
            foreignKeyQuery.Append( "SELECT distinct pkcols.table_name as PRIMARY_KEY_TABLE_NAME, pkcols.column_name as PRIMARY_KEY_COLUMN_NAME, " );
            foreignKeyQuery.Append( "fkcols.table_name as FOREIGN_KEY_TABLE_NAME, fkcols.column_name as FOREIGN_KEY_COLUMN_NAME, " );
            foreignKeyQuery.Append( "fkcons.constraint_name as FOREIGN_NAME, pkcons.constraint_name as PRIMARY_NAME " );

            foreignKeyQuery.Append( "FROM user_constraints pkcons, user_cons_columns pkcols, user_cons_columns fkcols, user_constraints fkcons " );
            foreignKeyQuery.Append( "WHERE fkcols.table_name = :RapidOracleParamTable " );
            foreignKeyQuery.Append( "AND pkcons.constraint_type = 'P' " );
            foreignKeyQuery.Append( "AND fkcons.constraint_type = 'R' " );
            foreignKeyQuery.Append( "AND fkcons.constraint_name = fkcols.constraint_name " );
            foreignKeyQuery.Append( "AND fkcons.owner = fkcols.owner " );
            foreignKeyQuery.Append( "AND fkcons.status = 'ENABLED' " );
            foreignKeyQuery.Append( "AND fkcons.R_CONSTRAINT_NAME = pkcons.constraint_name " );
            foreignKeyQuery.Append( "AND pkcons.constraint_name = pkcols.constraint_name " );

            IDbCommand command = connection.CreateCommand( );
            command.CommandText = foreignKeyQuery.ToString( );
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            IDbDataParameter foreignKeyTableParameter = command.CreateParameter( );
            foreignKeyTableParameter.DbType = DbType.String;
            foreignKeyTableParameter.ParameterName = ":RapidOracleParamTable";
            foreignKeyTableParameter.Value = descriptor.TableName;
            command.Parameters.Add( foreignKeyTableParameter );

            IDataReader dataReader = command.ExecuteReader( );

            DataTable dataTable = new DataTable( );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.PRIMARY_KEY_TABLE_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.PRIMARY_KEY_COLUMN_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.FOREIGN_KEY_TABLE_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.FOREIGN_KEY_COLUMN_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.FOREIGN_KEY_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.PRIMARY_KEY_NAME ) );

            while ( dataReader.Read( ) )
            {
                dataTable.Rows.Add( dataReader[MetabaseConstants.PRIMARY_KEY_TABLE_NAME] ,
                    dataReader[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME] ,
                    dataReader[MetabaseConstants.FOREIGN_KEY_TABLE_NAME] ,
                    dataReader[MetabaseConstants.FOREIGN_KEY_COLUMN_NAME] ,
                    dataReader[MetabaseConstants.FOREIGN_KEY_NAME] ,
                    dataReader[MetabaseConstants.PRIMARY_KEY_NAME] );
            }

            dataReader.Close( );
            return dataTable;
        }

        public override DataTable ExecutePrimaryKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            StringBuilder primaryKeyQuery = new StringBuilder( );
            primaryKeyQuery.Append( " SELECT cols.constraint_name as PK_NAME, cols.column_name as COLUMN_NAME " );
            primaryKeyQuery.Append( " FROM user_constraints cons, user_cons_columns cols" );
            primaryKeyQuery.Append( " WHERE cols.table_name = :RapidOracleParamTable" );
            primaryKeyQuery.Append( " AND cons.constraint_type = 'P'" );
            primaryKeyQuery.Append( " AND cons.constraint_name = cols.constraint_name" );
            primaryKeyQuery.Append( " AND cons.owner = cols.owner" );
            primaryKeyQuery.Append( " AND cons.status = 'ENABLED'" );
            primaryKeyQuery.Append( " ORDER BY cols.table_name, cols.position" );

            IDbCommand command = connection.CreateCommand( );
            command.CommandText = primaryKeyQuery.ToString( );
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            IDbDataParameter primaryKeyTableParameter = command.CreateParameter( );
            primaryKeyTableParameter.DbType = DbType.String;
            primaryKeyTableParameter.ParameterName = ":RapidOracleParamTable";
            primaryKeyTableParameter.Value = descriptor.TableName;
            command.Parameters.Add( primaryKeyTableParameter );

            IDataReader dataReader = command.ExecuteReader( );

            DataTable dataTable = new DataTable( );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.PRIMARY_KEY_NAME ) );
            dataTable.Columns.Add( new DataColumn( MetabaseConstants.PRIMARY_KEY_COLUMN_NAME ) );

            while ( dataReader.Read( ) )
            {
                dataTable.Rows.Add
                    (
                    dataReader[SqlServerMetabaseConstants.PK_NAME] ,
                    dataReader[SqlServerMetabaseConstants.COLUMN_NAME]
                    );
            }

            dataReader.Close( );
            return dataTable;
        }

        /// <summary>
        /// Creates a sequence for an auto increment column in oracle.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="autoKeyName"></param>
        /// <returns></returns>
        public override string[] ExecuteAfterCreateStatement( string tableName , string autoKeyName )
        {
            if ( autoKeyName.IsNullOrEmpty( ) )
                return null;

            string[] statements = new string[2];
            string sequenceName = DecorateName( tableName.MakeUnique( 3 ).ToUpper( ) + "_SEQ" );
            statements[0] = string.Format( "CREATE SEQUENCE {0} INCREMENT BY 1 " , sequenceName );

            string triggerName = tableName.MakeUnique( 3 ).ToUpper( ) + "SEQ_TR";
            StringBuilder sequenceBuilder = new StringBuilder( );

            sequenceBuilder.AppendFormat( " CREATE OR REPLACE TRIGGER  \"{0}\" " , triggerName );
            sequenceBuilder.AppendFormat( " before insert on \"{0}\" \n" , tableName );
            sequenceBuilder.Append( " for each row " );
            sequenceBuilder.Append( " begin " );
            sequenceBuilder.AppendFormat( " select {0}.nextval into :NEW.\"{1}\" from dual; \n" , sequenceName , autoKeyName );
            sequenceBuilder.Append( " end;\n" );

            statements[1] = sequenceBuilder.ToString( );

            return statements;
        }

        public override bool TableExist( string tableName )
        {
            try
            {
                DbCommand command = Connection.CreateCommand( );
                command.CommandText = string.Format( "SELECT * FROM cat repos WHERE repos.table_name = '{0}'" , tableName );
                command.CommandType = CommandType.Text;
                command.Connection = Connection;

                DbDataReader dataReader = command.ExecuteReader( );
                bool exist = dataReader.Read( );
                dataReader.Close( );

                return exist;
            }
            catch ( Exception x )
            {
                return false;
            }
        }

        public override IEnumerable<TableDescriptor> GetAllUserTables( )
        {
            DbCommand command = Connection.CreateCommand( );
            command.CommandText = string.Format( "SELECT * FROM cat where TABLE_TYPE = 'TABLE'" );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            DbDataReader dataReader = command.ExecuteReader( );

            while ( dataReader.Read( ) )
            {
                yield return new TableDescriptor { TableName = dataReader[0].ToString( ) };
            }

            dataReader.Close( );
        }

        protected override void ExecuteRemovePrimaryKeyConstraintCommand( TableDescriptor descriptor , DbConnection connection )
        {
            base.ExecuteRemovePrimaryKeyConstraintCommand( descriptor , connection );
        }

        protected override string GetMetadataType( string clrType )
        {
            Type type = GlobalUtility.GetPrimitiveType( clrType );
            DbType dbType = DataTypeMapping.ConvertToSQLType( type );

            return typeMappings[dbType].GetDatabaseType( );

            //switch (dbType)
            //{
            //    case DbType.Binary:
            //        return "BLOB";
            //    case DbType.Boolean:
            //        return "NUMBER(1,0)";
            //    case DbType.Byte:
            //    case DbType.SByte:
            //        return "NUMBER(3,0)";
            //    case DbType.Decimal:
            //        return "NUMBER(19,5)";
            //    case DbType.Double:
            //        return "DOUBLE PRECISION";
            //    case DbType.Guid:
            //        return "CLOB";
            //    case DbType.Int32:
            //        return "NUMBER(10,0)";
            //    case DbType.Int64:
            //        return "NUMBER(20,0)";
            //    case DbType.UInt32:
            //        return "NUMBER(10,0)";
            //    case DbType.Int16:
            //        return "NUMBER(5,0)";
            //    case DbType.UInt64:
            //        return "NUMBER(20,0)";
            //    case DbType.String:
            //        return "VARCHAR2(2000)";
            //    case DbType.DateTime:
            //        return "TIMESTAMP";
            //    case DbType.Date:
            //        return "DATE";
            //    default:
            //        return "CLOB";
            //}
        }

        public override string DecorateName( string fieldName )
        {
            return string.Format( "\"{0}\"" , fieldName );
        }
    }
}
