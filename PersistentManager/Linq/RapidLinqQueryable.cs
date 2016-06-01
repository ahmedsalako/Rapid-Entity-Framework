using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace PersistentManager.Linq
{
    public class RapidLinqQueryable<T> : IQueryable, IQueryable<T>, IEnumerable, IEnumerable<T> , IOrderedQueryable, IOrderedQueryable<T>
    {
        internal RapidQueryProviderBase provider;
        protected Expression expression;

        internal RapidLinqQueryable( RapidQueryProviderBase provider )
        {
            this.provider = provider;         
            expression = Expression.Constant( this );
        }

        internal RapidLinqQueryable( RapidQueryProviderBase provider , Expression expression )
        {
            this.provider = provider;
            this.expression = expression;
        }

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return expression; }
            private set { expression = value; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return this.provider.GetQueryText(this.expression);
        }
        
    }
}
