using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using consist.RapidEntity.Customizations.Descriptors;
using consist.RapidEntity.Util;
using consist.RapidEntity.Customizations.Extensions;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    internal class Db2MetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType , MetabaseTypeMapping> typeMappings = new Dictionary<DbType , MetabaseTypeMapping>( );

        static Db2MetabaseProvider( )
        {
            typeMappings.Add( DbType.Binary , CreateMapping( "BLOB" , 0 , 0 , "System.Byte[]" ) );
            typeMappings.Add( DbType.Int16 , CreateMapping( "SMALLINT" , 0 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.Boolean , CreateMapping( "SMALLINT" , 0 , 0 , "System.Boolean" ) ); //More research
            typeMappings.Add( DbType.Byte , CreateMapping( "SMALLINT" , 0 , 0 , "System.Byte" ) ); //More research
            typeMappings.Add( DbType.SByte , CreateMapping( "SMALLINT" , 0 , 0 , "System.SByte" ) ); //More research
            typeMappings.Add( DbType.Decimal , CreateMapping( "DECIMAL" , 9 , 2 , "System.Decimal" ) );
            typeMappings.Add( DbType.Double , CreateMapping( "DOUBLE PRECISION" , 0 , 0 , "System.Double" ) );
            typeMappings.Add( DbType.Guid , CreateMapping( "CLOB" , 0 , 0 , "System.Guid" ) );
            typeMappings.Add( DbType.Int32 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.Int64 , CreateMapping( "BIGINT" , 0 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.UInt32 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.UInt64 , CreateMapping( "BIGINT" , 0 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.String , CreateMapping( "VARCHAR" , 255 , null , "System.String" ) );
            typeMappings.Add( DbType.DateTime , CreateMapping( "DATE" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Date , CreateMapping( "DATE" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Time , CreateMapping( "TIME" , 0 , 0 , "System.TimeSpan" ) );
            typeMappings.Add( DbType.Single , CreateMapping( "REAL" , 0 , 0 , "System.Single" ) );
            typeMappings.Add( DbType.UInt16 , CreateMapping( "SMALLINT" , 0 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.StringFixedLength , CreateMapping( "CHAR" , 255 , 0 , "System.String" ) );
            typeMappings.Add( DbType.Xml , CreateMapping( "XML" , 0 , 0 , "System.Byte[]" ) );
            typeMappings.Add( DbType.Object , CreateMapping( "VARCHAR" , 255 , null , "System.String" ) );
        }

        internal Db2MetabaseProvider( DbConnection connection )
        {
            Connection = connection;
        }

        protected override void ExecuteRemovePrimaryKeyConstraintCommand( TableDescriptor descriptor , DbConnection connection )
        {
            base.ExecuteRemovePrimaryKeyConstraintCommand( descriptor , connection );
        }

        public override DataTable ExecuteForeignKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            StringBuilder foreignKeyQuery = new StringBuilder( );
            foreignKeyQuery.AppendFormat( " SELECT REFTABNAME AS {0} ," , MetabaseConstants.PRIMARY_KEY_TABLE_NAME );
            foreignKeyQuery.AppendFormat( " PK_COLNAMES AS {0} , " , MetabaseConstants.PRIMARY_KEY_COLUMN_NAME );
            foreignKeyQuery.AppendFormat( " TABNAME AS {0} , " , MetabaseConstants.FOREIGN_KEY_TABLE_NAME );
            foreignKeyQuery.AppendFormat( " FK_COLNAMES AS {0} , " , MetabaseConstants.FOREIGN_KEY_COLUMN_NAME );
            foreignKeyQuery.AppendFormat( " CONSTNAME AS {0} , " , MetabaseConstants.FOREIGN_KEY_NAME );
            foreignKeyQuery.AppendFormat( " REFKEYNAME AS {0} " , MetabaseConstants.PRIMARY_KEY_NAME );
            foreignKeyQuery.AppendFormat( " FROM syscat.references where LOWER(tabname) = LOWER('{0}') " , descriptor.TableName );

            IDbCommand command = connection.CreateCommand( );
            command.CommandText = foreignKeyQuery.ToString( );
            command.CommandType = CommandType.Text;
            command.Connection = connection;

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
            //StringBuilder primaryKeyQuery = new StringBuilder();
            //primaryKeyQuery.Append(" SELECT distinct a.CONSTRAINT_NAME as PK_NAME, a.COLUMN_NAME as COLUMN_NAME ");
            //primaryKeyQuery.Append(" FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE a, INFORMATION_SCHEMA.TABLE_CONSTRAINTS b ");
            //primaryKeyQuery.AppendFormat(" WHERE a.TABLE_NAME = '{0}' ", descriptor.TableName);
            //primaryKeyQuery.Append(" AND b.CONSTRAINT_NAME = a.CONSTRAINT_NAME ");
            //primaryKeyQuery.Append(" AND b.TABLE_NAME = a.TABLE_NAME ");
            //primaryKeyQuery.Append(" AND b.CONSTRAINT_TYPE = 'PRIMARY KEY' ");

            //IDbCommand command = connection.CreateCommand();
            //command.CommandText = primaryKeyQuery.ToString();
            //command.CommandType = CommandType.Text;
            //command.Connection = connection;

            //IDataReader dataReader = command.ExecuteReader();

            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_NAME));
            //dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_COLUMN_NAME));

            //while (dataReader.Read())
            //{
            //    dataTable.Rows.Add
            //        (
            //        dataReader[SqlServerMetabaseConstants.PK_NAME],
            //        dataReader[SqlServerMetabaseConstants.COLUMN_NAME]
            //        );
            //}

            //dataReader.Close();
            //return dataTable;             

            return new DataTable( );
        }

        public override string[] ExecuteAfterCreateStatement( string tableName , string autoKeyName )
        {
            return null;
        }

        public override string DecorateName( string fieldName )
        {
            return string.Format( "{0}" , fieldName );
        }

        public override bool TableExist( string tableName )
        {
            try
            {
                DbCommand command = Connection.CreateCommand( );
                command.CommandText = string.Format( " SELECT * FROM SYSCAT.TABLES WHERE LOWER(TABNAME) = LOWER('{0}') AND TYPE = 'T'" ,
                                tableName );

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
            command.CommandText = string.Format( "SELECT TABNAME FROM SYSCAT.TABLES WHERE ownertype = 'U' AND TYPE = 'T' " );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            DbDataReader dataReader = command.ExecuteReader( );

            while ( dataReader.Read( ) )
            {
                yield return new TableDescriptor { TableName = dataReader[0].ToString( ) };
            }

            dataReader.Close( );
        }

        protected override string GetMetadataType( string clrType )
        {
            Type type = GlobalUtility.GetPrimitiveType( clrType );

            DbType dbType = DataTypeMapping.ConvertToSQLType( type );

            return typeMappings[dbType].GetDatabaseType( );
        }

        public override void BuildColumns( TableDescriptor table , DbConnection connection )
        {
            StringBuilder query = new StringBuilder( );
            query.AppendFormat( " SELECT COLNAME as ColumnName , IDENTITY as IsAutoIncrement , LENGTH as NumericPrecision , SCALE as NumericScale , TYPENAME as DataType , " );
            query.AppendFormat( " NULLS as AllowDBNull , (CASE WHEN KEYSEQ is null THEN 'false' ELSE 'true' END) as IsKey " );
            query.AppendFormat( " FROM SYSCAT.COLUMNS where LOWER(TABNAME) = LOWER('{0}') " , table.TableName );

            DbCommand command = connection.CreateCommand( );
            command.CommandText = query.ToString( );
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            DbDataReader dataReader = command.ExecuteReader( );


            while ( dataReader.Read( ) )
            {
                ColumnDescriptor column = new ColumnDescriptor( );
                column.ColunmName = dataReader["ColumnName"].ToString( ).Trim( );

                column.IsAutoIncrement = dataReader["IsAutoIncrement"].ToString( ).Trim( ) == "N" ? false : true;

                column.Precision = int.Parse( dataReader["NumericPrecision"].ToString( ) );
                column.Scale = int.Parse( dataReader["NumericScale"].ToString( ) );

                string dataType = dataReader["DataType"].ToString( ).Trim( );
                MetabaseTypeMapping mapping = typeMappings.FirstOrDefault( m => m.Value.DatabaseType == dataType ).Value;

                column.LanguageDataType = mapping.IsNull( ) ? "System.Object" : mapping.CLRType;

                column.IsNullable = dataReader["AllowDBNull"].ToString( ).Trim( ) == "N" ? false : true;
                column.IsPrimaryKey = dataReader["IsKey"].ToString( ).Trim( ) == "true" ? true : false;

                table.Columns.Add( column );
            }

            dataReader.Close( );
        }

        protected override string SuffixStatement( string query )
        {
            return string.Format( " {0}{1} " , query , "; " );
        }

        protected override StringBuilder SuffixStatement( StringBuilder query )
        {
            return query.Append( "; " );
        }
    }
}
