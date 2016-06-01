using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using consist.RapidEntity.Util;
using consist.RapidEntity.Customizations.Extensions;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    internal delegate void CreateTableDelegate( ModelClass source );
    internal class SQLLiteMetabaseProvider : MetaBaseProvider
    {
        public static IDictionary<DbType , MetabaseTypeMapping> typeMappings = new Dictionary<DbType , MetabaseTypeMapping>( );

        internal SQLLiteMetabaseProvider( DbConnection connection )
        {
            Connection = connection;
        }

        static SQLLiteMetabaseProvider( )
        {
            typeMappings.Add( DbType.Binary , CreateMapping( "BLOB" , 0 , 0 , "System.Byte[]" ) );
            typeMappings.Add( DbType.Int16 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.Boolean , CreateMapping( "INTEGER" , 0 , 0 , "System.Boolean" ) ); //More research
            typeMappings.Add( DbType.Byte , CreateMapping( "INTEGER" , 0 , 0 , "System.Byte" ) ); //More research
            typeMappings.Add( DbType.SByte , CreateMapping( "INTEGER" , 0 , 0 , "System.SByte" ) ); //More research
            typeMappings.Add( DbType.Decimal , CreateMapping( "NUMERIC" , 9 , 2 , "System.Decimal" ) );
            typeMappings.Add( DbType.Double , CreateMapping( "NUMERIC" , 0 , 0 , "System.Double" ) );
            typeMappings.Add( DbType.Guid , CreateMapping( "UNIQUEIDENTIFIER" , 0 , 0 , "System.Guid" ) );
            typeMappings.Add( DbType.Int32 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.Int64 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.UInt32 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int32" ) );
            typeMappings.Add( DbType.UInt64 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int64" ) );
            typeMappings.Add( DbType.String , CreateMapping( "TEXT" , 0 , 0 , "System.String" ) );
            typeMappings.Add( DbType.DateTime , CreateMapping( "DATETIME" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Date , CreateMapping( "DATETIME" , 0 , 0 , "System.DateTime" ) );
            typeMappings.Add( DbType.Time , CreateMapping( "DATETIME" , 0 , 0 , "System.TimeSpan" ) );
            typeMappings.Add( DbType.Single , CreateMapping( "NUMERIC" , 0 , 0 , "System.Single" ) );
            typeMappings.Add( DbType.UInt16 , CreateMapping( "INTEGER" , 0 , 0 , "System.Int16" ) );
            typeMappings.Add( DbType.StringFixedLength , CreateMapping( "TEXT" , 0 , 0 , "System.String" ) );
            typeMappings.Add( DbType.Xml , CreateMapping( "BLOB" , 0 , 0 , "System.Byte[]" ) );
            typeMappings.Add( DbType.Object , CreateMapping( "TEXT" , 0 , 0 , "System.String" ) );
        }

        protected override string GetMetadataType( string clrType )
        {
            Type type = GlobalUtility.GetPrimitiveType( clrType );

            DbType dbType = DataTypeMapping.ConvertToSQLType( type );

            return typeMappings[dbType].GetDatabaseType( );
        }

        public override DataTable ExecuteForeignKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            StringBuilder foreignKeyQuery = new StringBuilder( );
            foreignKeyQuery.AppendFormat( "PRAGMA foreign_key_list({0});" , DecorateName( descriptor.TableName ) );

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
                dataTable.Rows.Add( dataReader["table"] ,
                    dataReader["to"] ,
                    descriptor.TableName ,
                    dataReader["from"] ,
                    string.Empty ,
                    string.Empty );
            }

            dataReader.Close( );
            return dataTable;
        }

        public override DataTable ExecutePrimaryKeyCommand( TableDescriptor descriptor , DbConnection connection )
        {
            return new DataTable( );
        }

        public override string[] ExecuteAfterCreateStatement( string tableName , string autoKeyName )
        {
            return null;
        }

        public override string DecorateName( string fieldName )
        {
            return string.Format( "[{0}]" , fieldName );
        }

        public override bool TableExist( string tableName )
        {
            try
            {
                DbCommand command = Connection.CreateCommand( );
                command.CommandText = string.Format( "SELECT * FROM sqlite_master WHERE type='table' AND name = '{0}';" , tableName.Trim() );
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
            command.CommandText = string.Format( "SELECT name FROM sqlite_master WHERE type='table'" );
            command.CommandType = CommandType.Text;
            command.Connection = Connection;

            DbDataReader dataReader = command.ExecuteReader( );

            while ( dataReader.Read( ) )
            {
                yield return new TableDescriptor { TableName = dataReader[0].ToString( ) };
            }

            dataReader.Close( );
        }

        internal override void AlterForeignKeyConstraints( ModelClass modelClass , TableDescriptor tableMetadata )
        {
            foreach ( var relationship in modelClass.GetRelationshipsByTargetIncludeInheritance( ) )
            {
                if ( tableMetadata.ForeignKeys.Count( f => f.ForeignKeyOwnerName.IsEquals( relationship.Source.TableName ) ) <= 0 )
                {
                    StringBuilder script = new StringBuilder( );
                    foreach ( ModelClass subclass in modelClass.GetSubclassesOrSelf( ) )
                    {
                        string primaryKey = GetPrimaryKeyName( relationship , subclass );

                        StringBuilder body = new StringBuilder( string.Format( " ALTER TABLE {0} ADD COLUMN {1} {2} " , DecorateName( modelClass.TableName ) , DecorateName( relationship.ReferenceColumn ) , GetMetadataType( relationship.Type ) ) );
                        body.AppendFormat( " REFERENCES {0} ({1}) " ,
                                    DecorateName( relationship.Source.TableName ) ,
                                    DecorateName( relationship is OneToMany ? primaryKey : relationship.ReferenceColumn ) );

                        ExecuteNonQueryText( body.ToString( ) );
                    }
                }
            }
        }

        protected override string SuffixStatement( string query )
        {
            return string.Format( " {0}{1} " , query , "; " );
        }

        protected override StringBuilder SuffixStatement( StringBuilder query )
        {
            return query.Append( "; " );
        }

        protected override bool ScriptPrimaryKeySeperately( )
        {
            return false;
        }
    }
}
