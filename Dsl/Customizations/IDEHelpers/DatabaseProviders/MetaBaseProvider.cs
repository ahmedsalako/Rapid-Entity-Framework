using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;
using System.Data;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.OracleClient;
using Microsoft.VisualStudio.Shell.Interop;
using consist.RapidEntity.Util;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public abstract class MetaBaseProvider
    {
        private DbConnection _connection;
        public virtual DbConnection Connection 
        {
            get
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public virtual void BuildColumns(TableDescriptor table, DbConnection connection)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM {0}", DecorateName(table.TableName));
            command.CommandType = CommandType.Text;
            command.Connection = connection;

            DbDataReader dataReader = command.ExecuteReader(CommandBehavior.KeyInfo);
            DataTable schemaInfo = dataReader.GetSchemaTable();

            foreach (DataRow row in schemaInfo.Rows)
            {
                ColumnDescriptor column = new ColumnDescriptor();
                column.ColunmName = row["ColumnName"].ToStringSpecial().Trim();

                //Oracle does not bind a sequence in metadata for a table
                //there is no confirmed way to detect an oracle sequence against columns yet.
                if (!(this is OracleMetabaseProvider))
                {
                    bool isAutoIncrement = false;
                    bool.TryParse(row["IsAutoIncrement"].ToStringSpecial().Trim(), out isAutoIncrement);

                    column.IsAutoIncrement = isAutoIncrement;
                }

                column.Precision = row["NumericPrecision"].ToStringSpecial().ParseSpecial();
                column.Scale = row["NumericScale"].ToStringSpecial( ).ParseSpecial( );
                //column.IntSQLType = int.Parse(row[13].ToString());
                column.LanguageDataType = FindMatchingType(this, row["DataType"].ToStringSpecial().Trim(), column.Precision, column.Scale);

                bool allowDbNull = false;
                bool.TryParse(row["AllowDBNull"].ToStringSpecial().Trim(), out allowDbNull);

                column.IsNullable = allowDbNull;

                bool isReadOnly = false;
                bool.TryParse(row["AllowDBNull"].ToStringSpecial().Trim(), out isReadOnly);
                column.IsReadOnly = isReadOnly;

                bool isUnique = false;
                bool.TryParse(row["IsUnique"].ToStringSpecial().Trim(), out isUnique);


                bool isKey = false;
                bool.TryParse(row["IsKey"].ToStringSpecial().Trim(), out isKey);
                column.IsPrimaryKey = isKey;

                table.Columns.Add(column);
            }

            dataReader.Close();
        }

        public IList<TableDescriptor> BuildTables( DbConnection connection )
        {
            List<TableDescriptor> tables = new List<TableDescriptor>( );
            foreach ( TableDescriptor table in GetAllUserTables( ).ToList() )
            {
                tables.Add( BuildTable( table , connection ) );
            }

            return tables;
        }

        public TableDescriptor BuildTable( TableDescriptor table , DbConnection connection )
        {
            BuildColumns( table , connection );
            BuildForeignKeys( table , connection );
            BuildPrimaryKeys( table , connection );

            return table;
        }

        public TableDescriptor BuildTable( string tableName , DbConnection connection )
        {
            return BuildTable( new TableDescriptor( ) { TableName = tableName } , connection );
        }

        public void BuildPrimaryKeys( TableDescriptor descriptor , DbConnection connection )
        {
            DataTable primaryKeyMeta = ExecutePrimaryKeyCommand( descriptor , connection );

            foreach ( DataRow row in primaryKeyMeta.Rows )
            {
                string primaryKeyColumn = row[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME].ToString( ).Trim( );
                ColumnDescriptor column = descriptor.Columns.Where( c => c.ColunmName.IsEquals( primaryKeyColumn ) ).FirstOrDefault( );

                if ( !column.IsNull( ) && column.IsPrimaryKey )
                {
                    column.PrimaryKeyName = row[MetabaseConstants.PRIMARY_KEY_NAME].ToString( ).Trim( );
                }
            }
        }

        public void BuildForeignKeys(TableDescriptor descriptor, DbConnection connection)
        {
            DataTable foreignKeyMeta = ExecuteForeignKeyCommand(descriptor, connection);

            foreach (DataRow row in foreignKeyMeta.Rows)
            {
                string foreigKeyColumn = row[MetabaseConstants.FOREIGN_KEY_COLUMN_NAME].ToString().Trim();
                ColumnDescriptor column = descriptor.Columns.Where(c => c.ColunmName.IsEquals(foreigKeyColumn)).FirstOrDefault();

                if (null != column)
                {
                    column.IsForeignKey = true;
                    column.ForeignKeyOwnerName = row[MetabaseConstants.PRIMARY_KEY_TABLE_NAME].ToString().Trim();
                    column.ForeignKeyOwnerColumn = row[MetabaseConstants.PRIMARY_KEY_COLUMN_NAME].ToString().Trim();
                    column.ForeignKeyName = row[MetabaseConstants.FOREIGN_KEY_NAME].ToString().Trim();
                    column.ForeignKeyTableName = row[MetabaseConstants.FOREIGN_KEY_TABLE_NAME].ToString().Trim();
                    column.ForeignKeyColumnName = row[MetabaseConstants.FOREIGN_KEY_COLUMN_NAME].ToString().Trim();
                }
            }
        }

        public static IEnumerable<DataProvider> GetDataProvider()
        {
            foreach ( DataProvider provider in ( DataProviderManager ) Package.GetGlobalService( typeof( DataProviderManager ) ) )
            {
                yield return provider;
            }
        }

        private static DataExplorerConnection FindDataConnection(string connectionString)
        {
            DataExplorerConnectionManager manager = (DataExplorerConnectionManager)Package.GetGlobalService(typeof(DataExplorerConnectionManager));

            foreach ( DataExplorerConnection connection in manager.GetConnections( ) )
            {
                try
                {
                    DataExplorerConnection selectedConnection = manager.FindConnection( connection.Provider , connectionString , false );

                    if ( !selectedConnection.IsNull( ) )
                    {
                        return selectedConnection;
                    }
                }
                catch ( Exception x )
                {
                    continue;
                }
            }

            return null;
        }

        private static DataConnection GetDataConnection(DataExplorerConnection selectedConnection, out Guid providerGuid)
        {
            DataConnectionManager explorerConnection = (DataConnectionManager)Package.GetGlobalService(typeof(DataConnectionManager));
            providerGuid = selectedConnection.Provider;

            return explorerConnection.GetDataConnection(selectedConnection.Provider, selectedConnection.EncryptedConnectionString, true);
        }

        public static DataConnection GetDataConnection( string connectionString , out Guid providerGuid , out string encryptedConnectionString )
        {
            DataExplorerConnection connection = FindDataConnection( connectionString );
            encryptedConnectionString = connection.EncryptedConnectionString;

            return GetDataConnection( connection , out providerGuid );
        }

        public static DataConnection GetDataConnection(DataConnectionManager explorerConnection, string connectionString, Guid providerGuid, bool isEncrypted)
        {
            try
            {
               return explorerConnection.GetDataConnection( providerGuid , connectionString , isEncrypted );              
            }
            catch
            {

            }

            DataExplorerConnectionManager manager = ( DataExplorerConnectionManager ) Package.GetGlobalService( typeof( DataExplorerConnectionManager ) );
            manager.AddConnection( "current" , providerGuid , connectionString , isEncrypted );

            return GetDataConnection( explorerConnection , connectionString , providerGuid , isEncrypted );
        }

        private static MetaBaseProvider GetMetabaseProvider(MetabaseType metabaseType, DbConnection connection)
        {
            switch (metabaseType)
            {
                case MetabaseType.OleDbMetaProvider:
                    return new OleDbMetabaseProvider(connection);
                case MetabaseType.SqlServerMetaProvider:
                    return new SqlServerMetabaseProvider(connection);
                case MetabaseType.OracleMetaProvider:
                    return new OracleMetabaseProvider(connection);
                case MetabaseType.MySqlMetaProvider:
                    return new MySqlMetabaseProvider(connection);
                case MetabaseType.DB2Provider:
                    return new Db2MetabaseProvider(connection);
                case MetabaseType.SQLLite3Provider:
                    return new SQLLiteMetabaseProvider( connection );
                default:
                    return new OleDbMetabaseProvider(connection);
            }
        }

        public static MetaBaseProvider GetMetabaseProvider(DbConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (connection is OleDbConnection)
            {
                return MetaBaseProvider.GetMetabaseProvider(MetabaseType.OleDbMetaProvider, connection);             
            }
            else if(connection is SqlConnection)
            {
                return MetaBaseProvider.GetMetabaseProvider(MetabaseType.SqlServerMetaProvider, connection);              
            }
            else if (connection is OracleConnection)
            {
                return MetaBaseProvider.GetMetabaseProvider(MetabaseType.OracleMetaProvider, connection); 
            }
            else if ( connection.GetType().FullName == "IBM.Data.DB2.DB2Connection" )
            {
                return MetaBaseProvider.GetMetabaseProvider(MetabaseType.DB2Provider, connection);
            }
            else if ( connection.GetType( ).FullName == "System.Data.SQLite.SQLiteConnection" )
            {
                return MetaBaseProvider.GetMetabaseProvider( MetabaseType.SQLLite3Provider , connection );
            }
            else //if (((IDbConnection)connection). is MySqlConnection)
            {
                return MetaBaseProvider.GetMetabaseProvider(MetabaseType.MySqlMetaProvider, connection);
            }

            throw new InvalidOperationException("Invalid database provider!");
        }

        public virtual void SyncronizeMetaData( ModelClass modelClass )
        {
            TableDescriptor table = BuildTable( modelClass.TableName , Connection );
            AlterColumnDifference( modelClass , table );
            AlterPrimaryKeyDifference( modelClass , table );
        }

        public virtual void SyncronizeForeignKeys( ModelClass modelClass )
        {
            TableDescriptor table = BuildTable(modelClass.TableName, Connection);
            AlterForeignKeyConstraints( modelClass , table );
        }

        public virtual void CreateTable( ModelClass modelClass )
        {
            if ( !TableExist( modelClass.TableName ) )
            {
                CreateTableStatement( modelClass );
            }
        }

        public virtual StringBuilder GenerateCreateScript( ModelClass modelClass )
        {
            StringBuilder createTableStatement = CreateTableStatementScriptOnly( modelClass );
            StringBuilder primaryKeyStatement = AddPrimaryKeyScriptOnly( modelClass );

            createTableStatement.AppendLine( string.Empty );

            if ( primaryKeyStatement.IsNotNull( ) )
            {
                createTableStatement.AppendLine( primaryKeyStatement.ToString( ) );
            }

            return createTableStatement;
        }

        public virtual StringBuilder GenerateRelationshipScript(ModelClass modelClass)
        {
            return AddForeignKeyConstraintScriptOnly(modelClass);
        }

        //MySQL / SQL Server / Oracle / MS Access:
        internal virtual void AlterForeignKeyConstraints( ModelClass modelClass , TableDescriptor tableMetadata )
        {
            foreach ( var relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
            {
                if ( tableMetadata.ForeignKeys.Count( f => f.ForeignKeyOwnerName.IsEquals( relationship.Source.TableName ) ) <= 0 )
                {
                    StringBuilder script = new StringBuilder( );
                    foreach ( ModelClass subclass in modelClass.GetSubclassesOrSelf( ) )
                    {
                        string constraintName = "FK_" + relationship.ReferenceColumn.MakeUnique( 6 );
                        string primaryKey = GetPrimaryKeyName( relationship , subclass );

                        StringBuilder body = new StringBuilder( string.Format( " ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY " , DecorateName( modelClass.TableName ) , DecorateName( constraintName ) ) );
                        body.AppendFormat( " ({0}) REFERENCES {1} ({2}) " ,
                                    DecorateName( relationship is OneToMany ? relationship.ReferenceColumn : relationship.ReferencedKey ) ,
                                    DecorateName( relationship.Source.TableName ) ,
                                    DecorateName( relationship is OneToMany ? primaryKey : relationship.ReferenceColumn ) );

                        StringBuilder constraint = new StringBuilder( string.Format( " ALTER TABLE {0} DROP CONSTRAINT {1} " , DecorateName( tableMetadata.TableName ) , DecorateName( constraintName ) ) );

                        ExecuteNonQueryText( body.ToString( ) );
                    }
                }
            }
        }

        //MySQL / SQL Server / Oracle / MS Access / DB2
        protected virtual void AlterPrimaryKeyDifference(ModelClass modelClass, TableDescriptor tableMetadata)
        {
            StringBuilder constraint = new StringBuilder(string.Format("  ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY ( ", 
               DecorateName( tableMetadata.TableName), DecorateName("PK_".MakeUnique(6))));

            bool? containsBody = false;
            int changeCount = 0;

            foreach (ModelAttribute field in modelClass.GetPropertiesIncludeInheritance())
            {
                ColumnDescriptor column = tableMetadata.Columns.Where(c => c.ColunmName.IsEquals(field.ColumnName)).FirstOrDefault();

                if (column.IsNull())
                    continue;

                    if (field is PersistentKey && !column.IsPrimaryKey)
                    {
                        constraint.Append(string.Format(" {0} {1} ", changeCount > 0 ? "," : "", DecorateName( field.ColumnName)));

                        containsBody = true;
                        changeCount += 1;
                    }               
            }

            constraint.Append(")");

            if (containsBody.Value)
            {
                ExecuteRemovePrimaryKeyConstraintCommand(tableMetadata, Connection); //Remove constraint
                ExecuteNonQueryText(constraint.ToString()); //Now create a new constraint
            }
        }

        //MySQL / SQL Server / Oracle / MS Access /DB2
        protected virtual StringBuilder AddPrimaryKeyScriptOnly(ModelClass modelClass)
        {
            if ( ScriptPrimaryKeySeperately( ) )
            {
                StringBuilder constraint = new StringBuilder( string.Format( "ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY (" ,
                   DecorateName( modelClass.TableName ) , DecorateName( "PK_".MakeUnique( 6 ) ) ) );

                bool? containsBody = false;
                int changeCount = 0;

                foreach ( ModelAttribute field in modelClass.GetPropertiesIncludeInheritance( ) )
                {
                    if ( field is PersistentKey )
                    {
                        constraint.Append( string.Format( "{0}{1}" , changeCount > 0 ? "," : "" , DecorateName( field.ColumnName ).Trim( ) ) );
                        containsBody = true;
                        changeCount += 1;
                    }
                }

                constraint.Append( ")" );

                return ( containsBody.Value ) ? SuffixStatement( constraint ) : null;
            }

            return null;
        }

        protected virtual void ExecuteNonQueryText(string sqlStatement)
        {
            if (sqlStatement.IsNullOrEmpty())
                return;

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            IDbCommand command = Connection.CreateCommand();
            command.CommandText = SuffixStatement( sqlStatement );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            command.ExecuteNonQuery();
        }

        protected virtual void ExecuteNonQueryText(string[] sqlStatements)
        {
            if (sqlStatements.IsNull())
                return;

            foreach (string statement in sqlStatements)
                ExecuteNonQueryText(statement);
        }

        protected virtual void AlterColumnDifference( ModelClass modelClass , TableDescriptor tableMetadata )
        {
            StringBuilder header = new StringBuilder( string.Format( " ALTER TABLE {0} ADD " , DecorateName( tableMetadata.TableName ) ) );
            List<string> alreadyAdded = new List<string>( );
            string clrType = string.Empty;

            foreach ( var relationship in modelClass.GetRelationshipsByTargetIncludeInheritance() )
            {
                if ( this is SQLLiteMetabaseProvider )
                    continue;

                ModelAttribute property = relationship.Source.GetByPropertyName( relationship.ReferencedKey );

                clrType = property.Type;

                if ( alreadyAdded.Contains( relationship.ReferenceColumn ) )
                continue;

                if ( tableMetadata.Columns.FirstOrDefault( c => c.ColunmName.Trim( ).ToLower( ) == relationship.ReferenceColumn.Trim().ToLower() ).IsNotNull() )
                {
                    continue;
                }

                if( tableMetadata.ForeignKeys.FirstOrDefault(  c => c.ColunmName.Trim( ).ToLower( ) == relationship.ReferenceColumn.Trim().ToLower() ).IsNotNull() ) 
                {
                    continue;
                }

                StringBuilder body = new StringBuilder( );
                body.Append( string.Format( " {0} {1} " , DecorateName( relationship.ReferenceColumn ) , GetMetadataType( clrType.Trim( ) ) , " NULL " ) );
                ExecuteNonQueryText( string.Format( "{0} {1}" , header.ToString( ) , body.ToString( ) ) );
                alreadyAdded.Add( relationship.ReferenceColumn );
            }

            foreach ( ModelAttribute field in modelClass.GetPropertiesIncludeInheritance( ) )
            {
                if ( tableMetadata.Columns.Count( c => c.ColunmName.IsEquals( field.ColumnName ) ) <= 0 )
                {
                    clrType = field.Type;

                    if ( alreadyAdded.Contains( field.ColumnName ) )
                        continue;

                    if ( tableMetadata.Columns.FirstOrDefault( c => c.ColunmName.Trim( ).ToLower( ) == field.ColumnName.Trim( ).ToLower( ) ).IsNotNull( ) )
                    {
                        continue;
                    }

                    StringBuilder body = new StringBuilder( );
                    body.Append( string.Format( " {0} {1} " , DecorateName( field.ColumnName ) , GetMetadataType( clrType.Trim( ) ) , " NULL " ) );
                    ExecuteNonQueryText( string.Format( "{0} {1}" , header.ToString( ) , body.ToString( ) ) );
                    alreadyAdded.Add( field.ColumnName );
                }
            }
        }

        protected virtual void CreateTableStatement( ModelClass modelClass )
        {
            StringBuilder query = new StringBuilder( );
            List<ModelAttribute> attributes = modelClass.GetPropertiesIncludeInheritance( ).ToList( );

            string autoKeyName = string.Empty;
            List<string> keys = new List<string>( );
            string tableName = modelClass.TableName.IsNullOrEmpty( ) ? modelClass.Name : modelClass.TableName;
            int elementCount = 1;

            query.Append( "CREATE TABLE  " );
            query.AppendLine( DecorateName( tableName ) + " ( " );

            foreach ( ModelAttribute property in attributes )
            {
                query.Append( DecorateName( property.ColumnName ) + " " );

                if ( property is PersistentKey )
                {
                    PersistentKey key = ( PersistentKey ) property;

                    if ( key.IsAutoKey )
                    {
                        keys.Add( key.ColumnName );
                        autoKeyName = key.ColumnName;
                        if ( this is SqlServerMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            query.Append( " IDENTITY(1,1) " );
                        }
                        else if ( this is OleDbMetabaseProvider )
                        {
                            query.Append( " COUNTER " );
                        }
                        else if ( this is MySqlMetabaseProvider )
                        {
                            query.Append( ( !key.ColumnName.IsNullOrEmpty( ) ) ? " INTEGER " : GetMetadataType( property.Type ) + " " );
                            query.Append( " AUTO_INCREMENT " );
                        }
                        else if ( this is OracleMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                        }
                        else if ( this is Db2MetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            query.Append( " GENERATED ALWAYS AS IDENTITY " );
                        }
                        else if ( this is SQLLiteMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            //query.Append( " AUTOINCREMENT " );
                        }
                    }
                    else
                    {
                        query.Append( GetMetadataType( property.Type ) + " " );
                        keys.Add( key.ColumnName );
                    }
                }
                else if ( property is Field )
                {
                    query.Append( GetMetadataType( property.Type ) + " " );
                }

                if ( property.AllowNull )
                {
                    query.Append( " NULL " );
                }
                else
                {
                    query.Append( " NOT NULL " );
                }

                if ( elementCount++ < attributes.Count )
                    query.AppendLine( "," );
            }

            foreach ( BaseRelationship relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
            {
                if ( !modelClass.ReferenceColumnIsFieldOrKey( relationship ) )
                {
                    query.AppendLine( "  ," + DecorateName( relationship.ReferenceColumn ) + " " );
                    query.Append( GetMetadataType( relationship.Type ) + " " );
                    query.Append( " NOT NULL " );
                }
            }

            if ( ( this is MySqlMetabaseProvider || this is SQLLiteMetabaseProvider ) && keys.Count > 0 )
            {
                query.Append( " , PRIMARY KEY (" );

                for ( int i = 0; i < keys.Count; i++ )
                {
                    query.AppendFormat( "{0} {1} " , i > 0 ? "," : "" , keys[i] );
                }

                query.Append( ") " );
            }

            if ( this is SQLLiteMetabaseProvider )
            {
                foreach ( BaseRelationship relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
                {
                    ModelClass referenceEntity = relationship.Source;
                    query.AppendFormat( ", FOREIGN KEY ({0}) REFERENCES {1} ({2})" , DecorateName( relationship.ReferenceColumn ) , DecorateName( relationship.Source.TableName ) , DecorateName( relationship.ReferencedKey ) );
                }
            }

            query.Append( ")" );

            ExecuteNonQueryText( query.ToString( ) );
            ExecuteNonQueryText( ExecuteAfterCreateStatement( tableName , autoKeyName ) );
        }

        protected virtual StringBuilder AddForeignKeyConstraintScriptOnly( ModelClass modelClass )
        {
            if ( this is SQLLiteMetabaseProvider )
                return null;

            StringBuilder script = new StringBuilder();

            foreach ( var relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
            {
                ModelClass referenceEntity = relationship.Source;
                {
                    foreach ( ModelClass subclass in modelClass.GetSubclassesOrSelf( ) )
                    {
                        string constraintName = "FK_" + relationship.ReferenceColumn.MakeUnique( 6 );
                        string primaryKey = GetPrimaryKeyName( relationship , subclass );

                        StringBuilder body = new StringBuilder( string.Format( "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY " , DecorateName( modelClass.TableName ) , DecorateName( constraintName ) ) );
                        body.AppendFormat( " ({0}) REFERENCES {1} ({2}) " ,
                            DecorateName( relationship is OneToMany ? relationship.ReferenceColumn : relationship.ReferenceColumn ) ,
                            DecorateName(referenceEntity.TableName),
                            DecorateName( relationship is OneToMany ? relationship.ReferencedKey : primaryKey ) );

                        body = SuffixStatement( body );
                        script.AppendLine( body.ToString( ) );                        
                    }
                }
            }

            return (script.Length > 0)?  script : null;
        }

        protected virtual string GetPrimaryKeyName( BaseRelationship relationship , ModelClass modelClass )
        {
            if ( relationship.ReferencedKey.IsNullOrEmpty( ) )
            {
                return modelClass.GetPropertiesIncludeInheritance( ).Where( p => p is PersistentKey ).FirstOrDefault( ).ColumnName;
            }

            return relationship.ReferencedKey;
        }

        protected virtual StringBuilder CreateTableStatementScriptOnly( ModelClass modelClass )
        {
            StringBuilder query = new StringBuilder( );
            List<ModelAttribute> attributes = modelClass.GetPropertiesIncludeInheritance( ).ToList( );

            List<string> keys = new List<string>( );
            string tableName = modelClass.TableName.IsNullOrEmpty( ) ? modelClass.Name : modelClass.TableName;
            int elementCount = 1;

            query.Append( "CREATE TABLE  " );
            query.Append( DecorateName( tableName ) );
            query.AppendLine( "( " );

            foreach ( ModelAttribute property in attributes )
            {
                query.Append( "  " + DecorateName( property.ColumnName ) + " " );

                if ( property is PersistentKey )
                {
                    PersistentKey key = ( PersistentKey ) property;

                    if ( key.IsAutoKey )
                    {
                        keys.Add( key.ColumnName );
                        if ( this is SqlServerMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            query.Append( " IDENTITY(1,1) " );
                        }
                        else if ( this is OleDbMetabaseProvider )
                        {
                            query.Append( " COUNTER " );
                        }
                        else if ( this is MySqlMetabaseProvider )
                        {
                            query.Append( ( !key.ColumnName.IsNullOrEmpty( ) ) ? " INTEGER " : GetMetadataType( property.Type ) + " " );
                            query.Append( " AUTO_INCREMENT " );
                        }
                        else if ( this is OracleMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                        }
                        else if ( this is Db2MetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            query.Append( " GENERATED ALWAYS AS IDENTITY " );
                        }
                        else if ( this is SQLLiteMetabaseProvider )
                        {
                            query.Append( GetMetadataType( property.Type ) + " " );
                            query.Append( " AUTOINCREMENT " );
                        }
                    }
                    else
                    {
                        query.Append( GetMetadataType( property.Type ) + " " );
                        keys.Add( key.ColumnName );                        
                    }
                }
                else if ( property is Field )
                {
                    query.Append( GetMetadataType( property.Type ) + " " );
                }

                if ( property.AllowNull )
                {
                    query.Append( " NULL " );
                }
                else
                {
                    query.Append( " NOT NULL " );
                }

                if ( elementCount++ < attributes.Count( a => ( a is Field ) || (a is PersistentKey )))
                    query.AppendLine( "," );
            }

            foreach( BaseRelationship relationship in modelClass.GetRelationshipsByTargetIncludeInheritance() )
            {
                if ( !modelClass.ReferenceColumnIsFieldOrKey( relationship ) )
                {
                    query.AppendLine( "," + DecorateName( relationship.ReferenceColumn ) + " " );
                    query.Append( GetMetadataType( relationship.Type ) + " " );
                    query.Append( " NOT NULL " );
                }
            }

            if ( ( this is MySqlMetabaseProvider || this is SQLLiteMetabaseProvider ) && keys.Count > 0  )
            {
                query.Append( " , PRIMARY KEY (" );

                for( int i = 0 ; i < keys.Count ; i++ )
                {
                    query.AppendFormat( "{0} {1} " , i > 0? "," : "" , keys[i] );
                }

                query.Append( ") " );
            }

            if ( this is SQLLiteMetabaseProvider )
            {
                foreach ( BaseRelationship relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
                {
                    ModelClass referenceEntity = relationship.Source;
                    query.AppendFormat( ", FOREIGN KEY ({0}) REFERENCES {1} ({2})" , DecorateName( relationship.ReferenceColumn ) , DecorateName( relationship.Source.TableName ) , DecorateName( relationship.ReferencedKey ) );
                }
            }

            query.AppendLine( ")" );

            return SuffixStatement( query );
        }

        protected virtual void ExecuteRemovePrimaryKeyConstraintCommand(TableDescriptor descriptor, DbConnection connection)
        {
            StringBuilder body = new StringBuilder();

            if (this is MySqlMetabaseProvider)
            {
                if (descriptor.Keys.Count(k => k.IsAutoIncrement) <= 0 && (descriptor.Keys.Count() > 0))
                {
                    body.AppendFormat("ALTER TABLE {0} DROP PRIMARY KEY", DecorateName(descriptor.TableName));
                    ExecuteNonQueryText(body.ToString());
                }
                else
                {
                    return;
                }
            }
            else
            {
                ColumnDescriptor column = descriptor.Columns.Where(c => c.IsPrimaryKey).FirstOrDefault();

                if (column.IsNull())
                    return;

                if (column.PrimaryKeyName.IsNullOrEmpty())
                    return;

                body = new StringBuilder(string.Format(" ALTER TABLE {0} DROP CONSTRAINT ", DecorateName(descriptor.TableName)));
                body.AppendFormat(" {0} ", DecorateName(column.PrimaryKeyName));
                ExecuteNonQueryText(body.ToString());
            }            
        }

        protected static MetabaseTypeMapping CreateMapping(string DatabaseType, int? precision, int? scale, string clrType)       
        {
            return new MetabaseTypeMapping
            {
                CLRType = clrType,
                DatabaseType = DatabaseType,
                Precision = precision,
                Scale = scale,
            };
        }

        protected static string FindMatchingType( MetaBaseProvider provider , string clrType , int precision , int scale )
        {
            if ( precision > 0 )
            {
                var results = GetCurrentTypeMappings( provider ).Where( m => m.Value.Precision == precision && m.Value.Scale == scale );

                foreach ( var typeMap in results )
                {
                    return typeMap.Value.CLRType;
                }
            }

            return clrType;
        }

        protected static IDictionary<DbType, MetabaseTypeMapping> GetCurrentTypeMappings(MetaBaseProvider provider)
        {
            if (provider is OleDbMetabaseProvider)
            {
                return OleDbMetabaseProvider.typeMappings;
            }
            else if (provider is SqlServerMetabaseProvider)
            {
                return SqlServerMetabaseProvider.typeMappings;
            }
            else if (provider is OracleMetabaseProvider)
            {
                return OracleMetabaseProvider.typeMappings;
            }
            else if (provider is MySqlMetabaseProvider)
            {
                return MySqlMetabaseProvider.typeMappings;
            }
            else if ( provider is Db2MetabaseProvider )
            {
                return Db2MetabaseProvider.typeMappings;
            }
            else if( provider is SQLLiteMetabaseProvider )
            {
                return SQLLiteMetabaseProvider.typeMappings;
            }

            return null;
        }

        protected virtual StringBuilder SuffixStatement( StringBuilder query )
        {
            return query;
        }

        protected virtual string SuffixStatement( string query )
        {
            return query;
        }

        protected virtual bool ScriptPrimaryKeySeperately( )
        {
            return true;
        }

        protected abstract string GetMetadataType( string clrType );
        public abstract DataTable ExecuteForeignKeyCommand(TableDescriptor descriptor, DbConnection connection);
        public abstract DataTable ExecutePrimaryKeyCommand(TableDescriptor descriptor, DbConnection connection);
        public abstract string[] ExecuteAfterCreateStatement(string tableName, string autoKeyName);
        public abstract string DecorateName(string fieldName);
        public abstract bool TableExist(string tableName);
        public abstract IEnumerable<TableDescriptor> GetAllUserTables( );
    }
}

