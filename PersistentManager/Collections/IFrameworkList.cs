using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PersistentManager.Initializers.Interfaces;
using System.Data.Common;
using PersistentManager.Cache;
using System.Diagnostics;
using System.Data;
using System.Runtime.Remoting.Contexts;
using PersistentManager.Descriptors;
using System.Linq;
using PersistentManager.Query.Sql;
using System.Collections.ObjectModel;
using PersistentManager.Linq;
using System.Linq.Expressions;

namespace PersistentManager.Collections
{
    [DebuggerDisplay( "Lazy Loaded" )]
    public class IFrameworkList<T> : IInternalList , IList<T> , IEnumerable<T> where T : class
    {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        ILazyLoader lazyLoader;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        Type type;

        public IFrameworkList( ILazyLoader lazyLoader , Type type )
        {
            this.lazyLoader = lazyLoader;
            this.type = type;
        }

        #region IList<T> Members

        int IList<T>.IndexOf( T item )
        {
            throw new NotImplementedException( );
        }

        public new void Insert( int index , T item )
        {
            lazyLoader.Add( item );
        }

        void IList<T>.RemoveAt( int index )
        {
            lazyLoader.RemoveAt( index );
        }

        T IList<T>.this[int index]
        {
            get
            {
               return ( T ) lazyLoader.GetIndex( index );
            }
            set
            {
               lazyLoader.Add( value );
            }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add( T item )
        {
            lazyLoader.Add( item );
        }

        void ICollection<T>.Clear( )
        {
            lazyLoader.ClearOrphans( );
        }

        bool ICollection<T>.Contains( T item )
        {
           return lazyLoader.ChildExist( item );
        }

        void ICollection<T>.CopyTo( T[] array , int arrayIndex )
        {
            List<T> list = new List<T>();

            foreach( T value in this )
            {
                list.Add(value);
            }

            list.CopyTo( array , arrayIndex );
        }

        int ICollection<T>.Count
        {
            get
            {
                int count = lazyLoader.Count;
                return ( count <= 0 ) ? 0 : count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove( T item )
        {
            return lazyLoader.RemoveChild( item );
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator( )
        {
            return lazyLoader.GetEntityReader<T>( ).GetEnumerator( );
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator( )
        {
            return lazyLoader.GetEntityReader<T>( ).GetEnumerator( );
        }

        #endregion
        
    }
}
