using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;
using System.Data.OleDb;
using consist.RapidEntity.Util;
using consist.RapidEntity.Customizations.Extensions;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public class OleDbMetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType, MetabaseTypeMapping> typeMappings = new Dictionary<DbType, MetabaseTypeMapping>();

        static OleDbMetabaseProvider()
        {
            typeMappings.Add(DbType.Binary, CreateMapping("IMAGE", 0, 0, "byte[]"));
            typeMappings.Add(DbType.Boolean, CreateMapping("BIT", 0, 0, "System.Boolean"));
            typeMappings.Add(DbType.Byte, CreateMapping("tinyint", 0, 0, "System.Byte"));
            typeMappings.Add(DbType.SByte, CreateMapping("tinyint", 0, 0, "System.SByte"));
            typeMappings.Add(DbType.Decimal, CreateMapping("FLOAT", 0, 0, "System.Decimal"));
            typeMappings.Add(DbType.Double, CreateMapping("Double", 0, 0, "System.Double"));
            typeMappings.Add(DbType.Guid, CreateMapping("text", 0, 0, "System.Guid"));
            typeMappings.Add(DbType.Int16, CreateMapping("SMALLINT", 0, 0, "System.Int16"));
            typeMappings.Add(DbType.Int32, CreateMapping("INTEGER", 0, 0, "System.Int32"));
            typeMappings.Add(DbType.Int64, CreateMapping("number", 0, 0, "System.Int64"));
            typeMappings.Add(DbType.UInt32, CreateMapping("INTEGER", 0, 0, "System.Int32"));
            typeMappings.Add(DbType.UInt64, CreateMapping("number", 0, 0, "System.Int64"));
            typeMappings.Add(DbType.String, CreateMapping("TEXT", 0, 0, "System.String"));
            typeMappings.Add(DbType.DateTime, CreateMapping("DATETIME", 0, 0, "System.DateTime"));
            typeMappings.Add(DbType.Date, CreateMapping("DATETIME", 0, 0, "System.DateTime"));
            typeMappings.Add(DbType.UInt16, CreateMapping("SMALLINT", 0, 0, "System.Int16"));
            typeMappings.Add(DbType.Single, CreateMapping("Double", 0, 0, "System.Single"));
        }

        internal OleDbMetabaseProvider(DbConnection connection)
        {
            Connection = connection;
        }

        protected override void ExecuteRemovePrimaryKeyConstraintCommand(TableDescriptor descriptor, DbConnection connection)
        {
            base.ExecuteRemovePrimaryKeyConstraintCommand(descriptor, connection);
        }

        public override DataTable ExecutePrimaryKeyCommand(TableDescriptor descriptor, DbConnection connection)
        {
            string[] restrictions = new string[3] { null, null, descriptor.TableName };

            DataTable schemaInfo = ((OleDbConnection)connection).GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, restrictions);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_COLUMN_NAME));

            foreach (DataRow row in schemaInfo.Rows)
            {
                DataRow newRow = dataTable.NewRow();
                newRow[MetabaseConstants.PRIMARY_KEY_NAME] = row[OleDbMetabaseConstants.PK_NAME];
                newRow[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME] = row[OleDbMetabaseConstants.COLUMN_NAME];

                dataTable.Rows.Add(newRow);
            }

            return dataTable;
        }

        public override DataTable ExecuteForeignKeyCommand(TableDescriptor descriptor, DbConnection connection)
        {
            string[] restrictions = new string[4] { null, null, descriptor.TableName, null};

            DataTable schemaInfo = ((OleDbConnection)connection).GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, null);

            DataTable dataTable = new DataTable("FOREIGN_KEYS");
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_TABLE_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_COLUMN_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_TABLE_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_COLUMN_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.FOREIGN_KEY_NAME));
            dataTable.Columns.Add(new DataColumn(MetabaseConstants.PRIMARY_KEY_NAME));


            foreach (DataRow row in schemaInfo.Rows)
            {
                string tableName = row[OleDbMetabaseConstants.FKTABLE_NAME].ToString();

                if (tableName == descriptor.TableName)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow[MetabaseConstants.PRIMARY_KEY_TABLE_NAME] = row[OleDbMetabaseConstants.PKTABLE_NAME];
                    newRow[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME] = row[OleDbMetabaseConstants.PKCOLUMN_NAME];
                    newRow[MetabaseConstants.FOREIGN_KEY_TABLE_NAME] = tableName;//row[OleDbMetabaseConstants.FKTABLE_NAME];
                    newRow[MetabaseConstants.FOREIGN_KEY_COLUMN_NAME] = row[OleDbMetabaseConstants.FKCOLUMN_NAME];
                    newRow[MetabaseConstants.FOREIGN_KEY_NAME] = row[OleDbMetabaseConstants.FK_NAME];
                    newRow[MetabaseConstants.PRIMARY_KEY_NAME] = row[OleDbMetabaseConstants.PK_NAME];

                    dataTable.Rows.Add(newRow);
                }
            }

            return dataTable;
        }

        public override string[] ExecuteAfterCreateStatement(string tableName, string autoKeyName)
        {
            return null;
        }

        public override bool TableExist(string tableName)
        {
            try
            {
                DataTable dataTable = null;
                DbDataReader dataReader = null;
                DbCommand command = Connection.CreateCommand();
                command.CommandText = string.Format("SELECT Top 1 {0}.* FROM {0} ", tableName);
                command.CommandType = CommandType.Text;
                command.Connection = Connection;

                try
                {
                    dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly);
                }
                catch (Exception x)
                {
                    //HACK: Table does not exist
                    return false;
                }

                dataTable = dataReader.GetSchemaTable();

                if (dataTable.IsNull())
                    return false;

                bool exist = (dataTable.Rows.Count > 0);
                dataReader.Close();

                return exist;
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        public override IEnumerable<TableDescriptor> GetAllUserTables( )
        {
            DataTable schemaTable = ((OleDbConnection)Connection).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                yield return new TableDescriptor { TableName = schemaTable.Rows[i].ItemArray[2].ToString() };              
            }        
        }

        protected override string GetMetadataType(string clrType)
        {
            Type type = GlobalUtility.GetPrimitiveType(clrType);

            DbType dbType = DataTypeMapping.ConvertToSQLType(type);

            return typeMappings[dbType].GetDatabaseType();
        }

        public override string DecorateName(string fieldName)
        {
            return string.Format("[{0}]", fieldName);
        }
    }
}
