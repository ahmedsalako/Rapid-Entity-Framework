using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace PersistentManager.Linq
{
    internal abstract class RapidQueryProviderBase : IQueryProvider
    {
        protected RapidQueryProviderBase() { }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression) 
        {
            return new RapidLinqQueryable<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression) 
        {
            try 
            {
                return (IQueryable)Activator.CreateInstance(typeof(RapidLinqQueryable<>).MakeGenericType(expression.Type), new object[] { this, expression });
            }
            catch (TargetInvocationException ex) 
            {
                throw ex.InnerException;
            }
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression) 
        {
            return CallInternalExecute<TResult>(expression);
        }

        object IQueryProvider.Execute( Expression expression ) 
        {
            return this.Execute( expression );
        }

        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);
        public abstract TResult CallInternalExecute<TResult>(Expression expression);
    }
}
