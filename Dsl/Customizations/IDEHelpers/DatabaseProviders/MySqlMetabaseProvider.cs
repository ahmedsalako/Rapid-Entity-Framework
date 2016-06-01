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
    internal class MySqlMetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType, MetabaseTypeMapping> typeMappings = new Dictionary<DbType, MetabaseTypeMapping>();

        static MySqlMetabaseProvider()
        {            
            typeMappings.Add(DbType.Binary, CreateMapping("BLOB", 0, 0, "byte[]"));
            typeMappings.Add(DbType.Boolean, CreateMapping("TINYINT", 1, -1, "System.Boolean"));
            typeMappings.Add(DbType.Byte, CreateMapping("TINYINT UNSIGNED", 0, 0, "System.Byte"));
            typeMappings.Add(DbType.SByte, CreateMapping("TINYINT UNSIGNED", 0, 0, "System.SByte"));
            typeMappings.Add(DbType.Decimal, CreateMapping("NUMERIC", 19, 5, "System.Decimal"));
            typeMappings.Add(DbType.Double, CreateMapping("double", 0, 0, "System.Double"));
            typeMappings.Add(DbType.Guid, CreateMapping("VARCHAR", 40, -1, "System.Guid"));
            typeMappings.Add(DbType.Int32, CreateMapping("INTEGER", 0, 0, "System.Int32"));
            typeMappings.Add(DbType.Int64, CreateMapping("bigint", 0, 0, "System.Int64"));
            typeMappings.Add(DbType.UInt32, CreateMapping("int", 0, 0, "System.Int32"));
            typeMappings.Add(DbType.UInt64, CreateMapping("bigint", 0, 0, "System.Int64"));
            typeMappings.Add(DbType.String, CreateMapping("nvarchar", 255, -1, "System.String"));
            typeMappings.Add(DbType.DateTime, CreateMapping("DATETIME", 0, 0, "System.DateTime"));
            typeMappings.Add(DbType.Date, CreateMapping("DATE", 0, 0, "System.DateTime"));
            typeMappings.Add(DbType.Int16, CreateMapping("SMALLINT", 0, 0, "System.Int16"));
            typeMappings.Add(DbType.Single, CreateMapping("FLOAT", 0, 0, "System.Single"));
            typeMappings.Add(DbType.UInt16, CreateMapping("SMALLINT", 0, 0, "System.Int16"));
        }

        internal MySqlMetabaseProvider(DbConnection connection)
        {
            Connection = connection;
        }

        protected override void ExecuteRemovePrimaryKeyConstraintCommand(TableDescriptor descriptor, DbConnection connection)
        {
            base.ExecuteRemovePrimaryKeyConstraintCommand(descriptor, connection);
        }

        public override DataTable ExecuteForeignKeyCommand(TableDescriptor descriptor, DbConnection connection)
        {
                StringBuilder foreignKeyQuery = new StringBuilder();
                foreignKeyQuery.Append("SELECT distinct a.TABLE_NAME as FOREIGN_KEY_TABLE_NAME, a.COLUMN_NAME as FOREIGN_KEY_COLUMN_NAME,  ");
                foreignKeyQuery.Append(" a.REFERENCED_TABLE_NAME as PRIMARY_KEY_TABLE_NAME,  ");
                foreignKeyQuery.Append(" a.REFERENCED_COLUMN_NAME as PRIMARY_KEY_COLUMN_NAME,  ");
                foreignKeyQuery.Append(" a.CONSTRAINT_NAME as FOREIGN_NAME,  ");
                foreignKeyQuery.Append(" b.CONSTRAINT_NAME as PRIMARY_NAME  ");
                foreignKeyQuery.Append(" FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE a, INFORMATION_SCHEMA.KEY_COLUMN_USAGE b  ");
                foreignKeyQuery.AppendFormat(" WHERE a.TABLE_NAME = '{0}'  ", descriptor.TableName);
                foreignKeyQuery.Append(" AND a.REFERENCED_TABLE_NAME is not null  ");
                foreignKeyQuery.Append(" AND a.REFERENCED_COLUMN_NAME = b.COLUMN_NAME  ");
                foreignKeyQuery.Append(" AND b.TABLE_NAME = a.REFERENCED_TABLE_NAME  ");

                IDbCommand command = connection.CreateCommand();
                command.CommandText = foreignKeyQuery.ToString();
                command.CommandType = CommandType.Text;
                command.Connection = connection;

                IDataReader dataReader = command.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_TABLE_NAME));
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_COLUMN_NAME));
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_TABLE_NAME));
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_COLUMN_NAME));
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_NAME));
                dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_NAME));

                while (dataReader.Read())
                {
                    dataTable.Rows.Add(dataReader[MetabaseConstants.PRIMARY_KEY_TABLE_NAME],
                        dataReader[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME],
                        dataReader[MetabaseConstants.FOREIGN_KEY_TABLE_NAME],
                        dataReader[MetabaseConstants.FOREIGN_KEY_COLUMN_NAME],
                        dataReader[MetabaseConstants.FOREIGN_KEY_NAME],
                        dataReader[MetabaseConstants.PRIMARY_KEY_NAME]);
                }

                dataReader.Close();
                return dataTable;
        }

        public override DataTable ExecutePrimaryKeyCommand(TableDescriptor descriptor, DbConnection connection)
        {
            StringBuilder primaryKeyQuery = new StringBuilder();
            primaryKeyQuery.Append(" SELECT distinct a.CONSTRAINT_NAME as PK_NAME, a.COLUMN_NAME as COLUMN_NAME ");
            primaryKeyQuery.Append(" FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE a, INFORMATION_SCHEMA.TABLE_CONSTRAINTS b ");
            primaryKeyQuery.AppendFormat(" WHERE a.TABLE_NAME = '{0}' ", descriptor.TableName);
            primaryKeyQuery.Append(" AND b.CONSTRAINT_NAME = a.CONSTRAINT_NAME ");
            primaryKeyQuery.Append(" AND b.TABLE_NAME = a.TABLE_NAME ");
            primaryKeyQuery.Append(" AND b.CONSTRAINT_TYPE = 'PRIMARY KEY' ");

            IDbCommand command = connection.CreateCommand();
            command.CommandText = primaryKeyQuery.ToString();
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            IDataReader dataReader = command.ExecuteReader();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_COLUMN_NAME));

            while (dataReader.Read())
            {
                dataTable.Rows.Add
                    (
                    dataReader[SqlServerMetabaseConstants.PK_NAME],
                    dataReader[SqlServerMetabaseConstants.COLUMN_NAME]
                    );
            }

            dataReader.Close();
            return dataTable;                
        }

        public override string[] ExecuteAfterCreateStatement(string tableName, string autoKeyName)
        {
            return null;
        }

        public override string DecorateName(string fieldName)
        {
            return string.Format("`{0}`", fieldName);
        }

        public override bool TableExist(string tableName)
        {
            try
            {
                DbCommand command = Connection.CreateCommand();
                command.CommandText = string.Format("SELECT * FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '{1}';",
                                Connection.Database, tableName);

                command.CommandType = CommandType.Text;
                command.Connection = Connection;

                DbDataReader dataReader = command.ExecuteReader();
                bool exist = dataReader.Read();
                dataReader.Close();

                return exist;
            }
            catch (Exception x)
            {
                return false;
            }
        }

        public override IEnumerable<TableDescriptor> GetAllUserTables( )
        {
            DbCommand command = Connection.CreateCommand( );
            command.CommandText = string.Format( "show tables;" );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            DbDataReader dataReader = command.ExecuteReader( );

            while ( dataReader.Read( ) )
            {
                yield return new TableDescriptor { TableName = dataReader[0].ToString( ) };
            }

            dataReader.Close( );
        }

        protected override string GetMetadataType(string clrType)
        {
            Type type = GlobalUtility.GetPrimitiveType(clrType);

            DbType dbType = DataTypeMapping.ConvertToSQLType(type);

            return typeMappings[dbType].GetDatabaseType();

            //switch (dbType)
            //{
            //    case DbType.Binary:
            //        return "BLOB";
            //    case DbType.Boolean:
            //        return "TINYINT(1)";
            //    case DbType.Byte:
            //    case DbType.SByte:
            //        return "TINYINT UNSIGNED";
            //    case DbType.Decimal:
            //        return "NUMERIC(19,5)";
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
    }
}
