using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PersistentManager.Initializers.Interfaces;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Linq;
using PersistentManager.Initializers;

namespace PersistentManager.Collections
{
    [DebuggerDisplay( "Lazy Loaded" )]
    public class FrameworkList : ArrayList , IList , IInternalList
    {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        ILazyLoader lazyLoader;

        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        Type type;

        public FrameworkList( ILazyLoader lazyLoader , Type type )
        {
            this.lazyLoader = lazyLoader;
            this.type = type;
        }

        public bool IsQuerying { get; set; }

        #region IList Members

        public override int Add( object value )
        {
            lazyLoader.PersistChildObject( value );
            return Count;
        }

        public override void Clear( )
        {
            ( ( Lazy )lazyLoader ).ClearOrphans( );
            return;
        }

        public override bool Contains( object value )
        {
            return lazyLoader.ChildExist( value );
        }

        public override int IndexOf( object value )
        {
            throw new NotImplementedException( );
        }

        public override void Insert( int index , object value )
        {
            lazyLoader.PersistChildObject( value );
        }

        public override bool IsFixedSize
        {
            get { return false; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override void Remove( object value )
        {
            lazyLoader.RemoveChild( value );
        }

        public override void RemoveAt( int index )
        {
            lazyLoader.RemoveAt( index );
        }

        public override object this[int index]
        {
            get
            {
                return lazyLoader.GetIndex( index );
            }
            set
            {
                lazyLoader.PersistChildObject( value );
            }
        }

        #endregion

        #region ICollection Members

        public override void CopyTo( Array array , int index )
        {
            ArrayList list = new ArrayList();

            foreach (var value in this)
            {
                list.Add(value);
            }

            list.CopyTo( array , index);

            return;
        }

        public override int Count
        {
            get
            {
                int count = lazyLoader.Count;
                return ( count <= 0 ) ? 0 : count;
            }
        }

        public override bool IsSynchronized
        {
            get { return false; }
        }

        public override object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public override IEnumerator GetEnumerator( )
        {
            return lazyLoader.GetEntityReader<object>( ).GetEnumerator( );
        }

        #endregion
    }
}
