using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using System.Diagnostics;
using System.Collections;
using System.Data;
using PersistentManager.Descriptors;

namespace PersistentManager.Collections
{
    public class FrameworkListEnumerator<T> : IEnumerator<T> , IEnumerator
    {
        private event LazyLoadingReadCompleted readCompleted;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private int current = 0;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private IDataReader dataReader;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private Type type;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private string[] uniqueName;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        EntityMetadata metaData;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        public Type Type
        {
            get { return type; }
            set
            {
                uniqueName = MetaDataManager.GetUniqueKeyNames( value );
                type = GhostGenerator.CreateGhostType( value );
                metaData = MetaDataManager.PrepareMetadata( type );
            }
        }

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        public IDataReader DataReader
        {
            get { return dataReader; }
            set { dataReader = value; }
        }

        public FrameworkListEnumerator( )
        {
            readCompleted += new LazyLoadingReadCompleted( FrameworkEnumerator_readCompleted );
        }

        private void OnDataReaderReadComplete( IDataReader dataReader )
        {
            if ( null != readCompleted )
            {
                readCompleted( dataReader );
            }
        }

        void FrameworkEnumerator_readCompleted( IDataReader dataReader )
        {
            if ( !dataReader.IsClosed )
                dataReader.Close( );
        }

        public void Reset( )
        {
            if ( current > -1 )
            {
                OnDataReaderReadComplete( dataReader );
            }

            current = -1;
        }

        public object Current
        {
            get
            {
                return GetCurrent( );
            }
        }

        public bool MoveNext( )
        {
            bool hasRow = dataReader.Read( );

            if ( !hasRow )
            {
                OnDataReaderReadComplete( dataReader );
            }

            return hasRow;
        }

        #region IEnumerator<T> Members

        T IEnumerator<T>.Current
        {
            get
            {
                return GetGenericCurrent( );
            }
        }

        private T GetGenericCurrent( )
        {
            return ( T )GetCurrent( );
        }

        private object GetCurrent( )
        {
            return SelectExpression.ReturnSingle( type , dataReader , false );
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose( )
        {
            if ( !dataReader.IsClosed )
            {
                while ( dataReader.Read( ) )
                {

                }

                dataReader.Close( );
            }
        }

        #endregion
    }
}
