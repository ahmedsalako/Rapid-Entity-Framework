using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Keywords
{
    public class DateTimeConstraints<TEntity,T,P> : Constraints<TEntity,T,P> where T : Keyword
    {
        internal DateTimeConstraints( object property , QueryPart QueryPart , T Current )
            : base( property , QueryPart , Current )
        {

        }

        public T Year( Action<Constraints<TEntity,T,int>> expression )
        {
            return Invoke( expression , FunctionCall.Year );
        }

        public T Month(Action<Constraints<TEntity, T, int>> expression)
        {
            return Invoke( expression , FunctionCall.Month );
        }

        public T Day(Action<Constraints<TEntity, T, int>> expression)
        {
            return Invoke( expression , FunctionCall.Day );
        }

        public T Hour(Action<Constraints<TEntity, T, int>> expression)
        {
            return Invoke( expression , FunctionCall.Hour );
        }

        public T Minute(Action<Constraints<TEntity, T, int>> expression)
        {
            return Invoke( expression , FunctionCall.Minute );
        }

        public T Second(Action<DateTimeConstraints<TEntity, T, int>> expression)
        {
            return Invoke( expression , FunctionCall.Second );
        }

        internal T Invoke(Action<Constraints<TEntity, T, int>> expression, FunctionCall call)
        {
            DateTimeConstraints<TEntity, T, int> instance = new DateTimeConstraints<TEntity, T, int>(Property, QueryPart, Current);
            instance.AddFunction( call , null );
            expression.Invoke( instance );

            return Current;
        }

        internal T Invoke(Action<DateTimeConstraints<TEntity, T, int>> expression, FunctionCall call)
        {
            DateTimeConstraints<TEntity, T, int> instance = new DateTimeConstraints<TEntity, T, int>(Property, QueryPart, Current);
            instance.AddFunction(call, null);
            expression.Invoke(instance);

            return Current;
        }
    }
}
