using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Collections;

namespace PersistentManager.Linq.ExpressionModifiers
{
    internal class CriteriaStack<T> : IEnumerable<Criteria> where T : Criteria
    {
        internal CriteriaStack( )
        {
            Criterias = new Stack<Criteria>( );
        }

        internal Criteria this[int i]
        {
            get { return Criterias.ToList( )[i]; }
        }

        internal Criteria Current
        {
            get { return SeeCurrent( ); }
        }

        internal Stack<Criteria> Criterias { get; set; }

        internal Criteria SeeCurrent( )
        {
            return Criterias.Peek( );
        }

        internal Criteria GetCurrent( )
        {
            if ( Criterias.IsEmpty( ) ) return null;            
            return Criterias.Pop( );
        }

        internal void AddCriteria( Criteria criteria )
        {
            Criterias.Push( criteria );
        }

        public IEnumerator<Criteria> GetEnumerator( )
        {
            return Criterias.GetEnumerator( );
        }

        IEnumerator IEnumerable.GetEnumerator( )
        {
            return Criterias.GetEnumerator( );
        }
    }
}
