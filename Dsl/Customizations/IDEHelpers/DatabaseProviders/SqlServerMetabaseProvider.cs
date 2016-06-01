using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;
using consist.RapidEntity.Util;
using consist.RapidEntity.Customizations.Extensions;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public class SqlServerMetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType , MetabaseTypeMapping> typeMappings = new Dictionary<DbType , MetabaseTypeMapping>( );

        static SqlServerMetabaseProvider( )
        {
            typeMappings.Add( DbType.Binary , CreateMapping( "binary" , 0 , 0 , "byte[]" ) );
            typeMappings.Add( DbType.Boolean , CreateMapping( "bit" , 0 , 0 , "System.Boolean" ) );
            typeMappings.Add( DbType.Byte , CreateMapping( "tinyint" , 0 , 0 , "System.Byte" ) );
            typeMappings.Add( DbType.SByte , CreateMapping( "tinyint" , 0 , 0 , "System.SByte" ) );
            typeMappings.Add( DbType.Decimal , CreateMapping( "decimal" , 0 , 0 , "System.Decimal" ) );
            typeMappings.Add( DbType.Double , CreateMapping( "double" , 0 , 0 , "System.Double" ) );
            typeMappings.Add( DbType.Guid , CreateMapping( "uniqueidentifier" , 0 , 0 , "System.Guid" ) );
            typeMappings.Add( DbType.Int16 , CreateMapping( "smallint" , 0 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.Int32 , CreateMapping( "int" , 0 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.Int64 , CreateMapping( "bigint" , 0 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.UInt16 , CreateMapping( "smallint" , 0 , 0 , "System.UInt16" ) );
            typeMappings.Add( DbType.UInt32 , CreateMapping( "int" , 0 , 0 , "System.UInt32" ) );
            typeMappings.Add( DbType.UInt64 , CreateMapping( "bigint" , 0 , 0 , "System.UInt64" ) );
            typeMappings.Add( DbType.String , CreateMapping( "nvarchar" , 255 , -1 , "System.String" ) );
            typeMappings.Add( DbType.DateTime , CreateMapping( "datetime" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Single , CreateMapping( "float" , 0 , 0 , "System.Single" ) );
        }

        internal SqlServerMetabaseProvider( DbConnection connection )
        {
            Connection = connection;
        }

        protected override void ExecuteRemovePrimaryKeyConstraintCommand( TableDescriptor descriptor , DbConnection connection )
        {
            base.ExecuteRemovePrimaryKeyConstraintCommand( descriptor , connection );
        }

        public override DataTable ExecutePrimaryKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            IDbCommand command = connection.CreateCommand( );
            command.CommandText = "sp_pkeys";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;

            IDbDataParameter primaryKeyTableParameter = command.CreateParameter( );
            primaryKeyTableParameter.DbType = DbType.String;
            primaryKeyTableParameter.ParameterName = "@table_name";
            primaryKeyTableParameter.Size = 128;
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

        public override DataTable ExecuteForeignKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            IDbCommand command = connection.CreateCommand( );
            command.CommandText = "sp_fkeys";
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;

            //IDbDataParameter pkTableParameter = command.CreateParameter();
            //pkTableParameter.DbType = DbType.String;
            //pkTableParameter.ParameterName = "@pktable_name";
            //pkTableParameter.Size = 128;
            //pkTableParameter.Value = "Employees";
            //command.Parameters.Add(pkTableParameter);

            IDbDataParameter foreignKeyTableParameter = command.CreateParameter( );
            foreignKeyTableParameter.DbType = DbType.String;
            foreignKeyTableParameter.ParameterName = "@fktable_name";
            foreignKeyTableParameter.Size = 128;
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
                dataTable.Rows.Add( dataReader[SqlServerMetabaseConstants.PKTABLE_NAME] ,
                    dataReader[SqlServerMetabaseConstants.PKCOLUMN_NAME] ,
                    dataReader[SqlServerMetabaseConstants.FKTABLE_NAME] ,
                    dataReader[SqlServerMetabaseConstants.FKCOLUMN_NAME] ,
                    dataReader[SqlServerMetabaseConstants.FK_NAME] ,
                    dataReader[SqlServerMetabaseConstants.PK_NAME] );
            }

            dataReader.Close( );
            return dataTable;
        }

        public override string[] ExecuteAfterCreateStatement( string tableName , string autoKeyName )
        {
            return null;
        }

        public override IEnumerable<TableDescriptor> GetAllUserTables( )
        {
            DbCommand command = Connection.CreateCommand( );
            command.CommandText = string.Format( "SELECT * FROM sysobjects WHERE OBJECTPROPERTY(id, N'IsUserTable') = 1" );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            DbDataReader dataReader = command.ExecuteReader( );

            while ( dataReader.Read( ) )
            {
                yield return new TableDescriptor { TableName = dataReader[0].ToString( ) };
            }

            dataReader.Close( );
        }

        public override bool TableExist( string tableName )
        {
            try
            {
                DbCommand command = Connection.CreateCommand( );
                command.CommandText = string.Format( "SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[{0}]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1" , tableName );
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

        protected override string GetMetadataType( string clrType )
        {
            try
            {
                Type type = GlobalUtility.GetPrimitiveType( clrType );

                DbType dbType = DataTypeMapping.ConvertToSQLType( type );

                return typeMappings[dbType].GetDatabaseType( );
            }
            catch ( Exception x )
            {
                throw x;
            }

            //switch (dbType)
            //{
            //    case DbType.Binary:
            //        return "binary";
            //    case DbType.Boolean:
            //        return "bit";
            //    case DbType.Byte:                    
            //    case DbType.SByte:
            //        return "tinyint";
            //    case DbType.Decimal:
            //        return "decimal";
            //    case DbType.Double:
            //        return "double";
            //    case DbType.Guid:
            //        return "uniqueidentifier";
            //    case DbType.Int32:
            //        return "int";
            //    case DbType.Int64:
            //        return "bigint";
            //    case DbType.UInt32:
            //        return "int";
            //    case DbType.UInt64:
            //        return "bigint";
            //    case DbType.String:
            //        return "nvarchar(255)";
            //    case DbType.DateTime:
            //        return "datetime";
            //    default:
            //        return "nvarchar";
            //}
        }

        public override string DecorateName( string fieldName )
        {
            return string.Format( "[{0}]" , fieldName );
        }
    }
}
