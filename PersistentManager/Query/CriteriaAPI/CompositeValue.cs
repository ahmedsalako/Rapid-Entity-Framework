using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Util;

namespace PersistentManager.Descriptors
{
    internal class CompositeValue : IEnumerable
    {
        ArrayList compositeValues;

        internal object this[int i]
        {
            get { return compositeValues[i]; }
        }

        internal CompositeValue( params object[] values ) : this()
        {
            AddValues( values );
        }

        internal CompositeValue( ArrayList list )
        {
            compositeValues = list;
        }

        internal CompositeValue( )
        {
            compositeValues = new ArrayList( );
        }

        internal CompositeValue AddValues( object[] values )
        {
            foreach( var value in values )
            {
                compositeValues.Add( value );
            }

            return this;
        }

        internal void RemoveAtIndex( int index )
        {
            compositeValues.RemoveAt( index );
        }

        internal object[] ToArray( )
        {
            return compositeValues.OfType<object>( ).ToArray( );
        }

        public IEnumerator GetEnumerator( )
        {
            return compositeValues.GetEnumerator( );
        }

        public IEnumerable<int> GetIndices( )
        {
            return compositeValues.GetIndices( );
        }

        internal int Count
        {
            get { return compositeValues.Count ; }
        }
    }
}
