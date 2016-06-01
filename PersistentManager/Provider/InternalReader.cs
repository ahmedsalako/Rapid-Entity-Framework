using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;

namespace PersistentManager.Provider
{
    internal class InternalReader : DbDataReader , IDisposable
    {
        private ResultSet[] results;
        private bool isClosed = false;
        private int currentRowIndex = -1;
        private int currentResultIndex = 0;

        internal InternalReader( IDictionary<string,object> dataReader , DataTable schema )
        {
            ArrayList resultsets = new ArrayList( );

            try
            {
                resultsets.Add( new ResultSet( dataReader , schema ) );
                results = ( ResultSet[] )resultsets.ToArray( typeof( ResultSet ) );
            }
            catch ( Exception ex )
            {
                throw ex;
            }
        }

        internal InternalReader( IDataReader dataReader )
        {
            ArrayList resultsets = new ArrayList( );

            try
            {
                resultsets.Add( new ResultSet( dataReader ) );
                results = ( ResultSet[] ) resultsets.ToArray( typeof( ResultSet ) );
            }
            catch ( Exception ex )
            {
                throw ex;
            }
            finally
            {
                dataReader.Close( );
            }
        }

        public override IEnumerator GetEnumerator( )
        {
            return results.GetEnumerator( );
        }

        private ResultSet Current
        {
            get { return results[currentResultIndex]; }
        }

        private object GetValue( string name )
        {
            return Current.GetValue( currentRowIndex , name );
        }

        public override bool HasRows
        {
            get { return RecordsAffected > 0; }
        }

        #region IDataReader Members

        public override DataTable GetSchemaTable( )
        {
            return Current.SchemaTable;
        }

        public override int RecordsAffected
        {
            get
            {
                return results.Count( );
            }
        }

        public override bool IsClosed
        {
            get { return isClosed; }
        }

        public override bool NextResult( )
        {
            return false;
        }

        public override void Close( )
        {
            isClosed = true;
        }

        public override bool Read( )
        {
            currentRowIndex++;

            if ( currentRowIndex >= results[currentResultIndex].RowCount )
            {
                // reset it back to the last row
                currentRowIndex--;
                return false;
            }
            return true;
        }

        public override int Depth
        {
            get { return currentResultIndex; }
        }

        #endregion

        #region IDisposable Members

        public new void Dispose( )
        {
            isClosed = true;
            results = null;
        }

        #endregion

        #region IDataRecord Members

        public override int GetInt32( int i )
        {
            return Convert.ToInt32( GetValue( i ) );
        }

        public override object this[string name]
        {
            get { return GetValue( name ); }
        }

        public override object this[int i]
        {
            get { return GetValue( i ); }
        }

        public override object GetValue( int i )
        {
            return Current.GetValue( currentRowIndex , i );
        }

        public override bool IsDBNull( int i )
        {
            return GetValue( i ).Equals( DBNull.Value );
        }

        public override long GetBytes( int i , long fieldOffset , byte[] buffer , int bufferOffset , int length )
        {
            throw new NotImplementedException( "GetBytes is not implemented." );
        }

        public override byte GetByte( int i )
        {
            return Convert.ToByte( GetValue( i ) );
        }

        public override System.Type GetFieldType( int i )
        {
            return Current[i];
        }

        public override decimal GetDecimal( int i )
        {
            return Convert.ToDecimal( GetValue( i ) );
        }

        public override int GetValues( object[] values )
        {
            return Current.GetValues( currentRowIndex , values );
        }

        public override string GetName( int i )
        {
            return Current.GetName( i );
        }

        public override int FieldCount
        {
            get { return Current.FieldCount; }
        }

        public override long GetInt64( int i )
        {
            return Convert.ToInt64( GetValue( i ) );
        }

        public override double GetDouble( int i )
        {
            return Convert.ToDouble( GetValue( i ) );
        }

        public override bool GetBoolean( int i )
        {
            return Convert.ToBoolean( GetValue( i ) );
        }

        public override Guid GetGuid( int i )
        {
            return ( Guid ) GetValue( i );
        }

        public override DateTime GetDateTime( int i )
        {
            return Convert.ToDateTime( GetValue( i ) );
        }

        public override int GetOrdinal( string name )
        {
            return Current.GetOrdinal( name );
        }

        public override string GetDataTypeName( int i )
        {
            return Current[i].Name;
        }

        public override float GetFloat( int i )
        {
            return Convert.ToSingle( GetValue( i ) );
        }

        public override long GetChars( int i , long fieldOffset , char[] buffer , int bufferOffset , int length )
        {
            throw new NotImplementedException( "No Implementation for GetChars." );
        }

        public override string GetString( int i )
        {
            return Convert.ToString( GetValue( i ) );
        }

        public override char GetChar( int i )
        {
            return Convert.ToChar( GetValue( i ) );
        }

        public override short GetInt16( int i )
        {
            return Convert.ToInt16( GetValue( i ) );
        }

        #endregion

        private class ResultSet
        {
            private readonly object[][] records;
            private bool columnMetaPopulated = false;
            internal DataTable SchemaTable { get; set; }

            private IDictionary<int , KeyValuePair<string , Type>> fields =
                new Dictionary<int , KeyValuePair<string , Type>>( );

            internal int FieldCount
            {
                get;
                set;
            }

            internal string GetName( int index )
            {
                return fields.Where( v => v.Key == index ).FirstOrDefault( )
                    .Value.Key;
            }

            internal Type this[string name]
            {
                get
                {
                    KeyValuePair<string , Type> value = fields.Where( v => v.Value.Key == name ).FirstOrDefault( ).Value;
                    return value.Value;
                }
            }

            internal Type this[int index]
            {
                get
                {
                    KeyValuePair<string , Type> value = fields.Where( v => v.Key == index ).FirstOrDefault( ).Value;
                    return value.Value;
                }
            }

            internal object GetValue( int row , int col )
            {
                return records[row][col];
            }

            public object GetValue( int row , string columnName )
            {
                int columnIndex = fields.Where( v => v.Value.Key.ToLower( ) == columnName.ToLower( ) ).FirstOrDefault( ).Key;
                return GetValue( row , columnIndex );
            }

            public int GetValues( int row , object[] values )
            {
                Array.Copy( records[row] , 0 , values , 0 , FieldCount );
                return FieldCount;
            }

            public int GetOrdinal( string name )
            {
                return fields.Where( v => v.Value.Key.ToLower( ) == name.ToLower( ) ).FirstOrDefault( ).Key;
            }

            public int RowCount
            {
                get { return records.Length; }
            }

            internal ResultSet( IDictionary<string,object> dataReader , DataTable schemaTable )
            {
                SchemaTable = schemaTable;

                ArrayList resultsets = new ArrayList( );
                int count = 0;

                foreach( KeyValuePair<string,object> data in dataReader )
                {
                    KeyValuePair<string , Type> columnInfo = new KeyValuePair<string , Type>( data.Key , data.Value.GetType( ) );
                    fields.Add( count++ , columnInfo );
                }

                object[] values = dataReader.Values.ToArray( );
                resultsets.Add( values );

                FieldCount = dataReader.Count;
                records = ( object[][] )resultsets.ToArray( typeof( object[] ) );
            }

            internal ResultSet( IDataReader dataReader )
            {
                SchemaTable = dataReader.GetSchemaTable( );

                ArrayList resultsets = new ArrayList( );

                while ( dataReader.Read( ) )
                {
                    if ( !columnMetaPopulated )
                    {
                        for ( int i = 0; i < dataReader.FieldCount; i++ )
                        {
                            KeyValuePair<string , Type> columnInfo = new KeyValuePair<string , Type>( dataReader.GetName( i ) , dataReader.GetFieldType( i ) );
                            fields.Add( i , columnInfo );
                        }
                        columnMetaPopulated = true;
                    }

                    object[] values = new object[dataReader.FieldCount];
                    dataReader.GetValues( values );
                    resultsets.Add( values );
                }

                FieldCount = dataReader.FieldCount;
                records = ( object[][] ) resultsets.ToArray( typeof( object[] ) );
            }
        }
    }
}