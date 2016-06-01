using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Keywords
{
    public class StringConstraints<TEntity, T, P> : Constraints<TEntity, T, P> where T : Keyword
    {        
        internal StringConstraints( object property , QueryPart QueryPart , T Current  ) : base( property , QueryPart , Current )
        {
            this.Current = Current;
            this.QueryPart = QueryPart;
        }

        public T Trim(Action<StringConstraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.Trim );
        }

        public T Length(Action<Constraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.StringLength );
        }

        public T ToLower(Action<StringConstraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.LowerCase );
        }

        public T ToUpper(Action<StringConstraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.UpperCase );
        }

        internal T Invoke(Action<StringConstraints<TEntity, T, P>> expression, FunctionCall call)
        {
            AddFunction( call , null );
            expression.Invoke( this );
            return Current;
        }

        internal T Invoke(Action<Constraints<TEntity, T, P>> expression, FunctionCall call)
        {
            AddFunction( call , null );
            expression.Invoke( this );
            return Current;
        }

        public T StartsWith( string value )
        {
            return AddExpression( Condition.StartsWith , value );
        }

        public T EndsWith( string value )
        {
            return AddExpression( Condition.EndsWith , value );
        }

        public T Contains( string value )
        {
            return AddExpression( Condition.Contains , value );
        }

        public T ContainsOr( string[] values )
        {
            string name = Keyword.GetParameterName( Property );

            foreach ( string value in values )
            {
                CurrentCriteria = Keyword.AddConditionExpression( name , QueryPart.OR , Condition.Contains , value );
            }

            return Current;
        }

        public T ContainsAnd(string[] values)
        {
            string name = Keyword.GetParameterName(Property);

            foreach (string value in values)
            {
                CurrentCriteria = Keyword.AddConditionExpression(name, QueryPart.AND, Condition.Contains, value);
            }

            return Current;
        }
    }
}
