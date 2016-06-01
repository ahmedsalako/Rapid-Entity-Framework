using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Collections;
using System.Linq.Expressions;
using PersistentManager.Linq.ExpressionCommands;
using PersistentManager.Linq;

namespace PersistentManager.Query.Keywords
{
    public class Constraints<TEntity,T,P>
    {
        private Queue<QueryFunction> queue = new Queue<QueryFunction>();
        internal Queue<QueryFunction> Functions { get { return queue; } }
        internal Criteria CurrentCriteria { get; set; }

        internal QueryPart QueryPart { get; set; }
        internal T Current { get; set; }

        internal Keyword Keyword
        {
            get { return Current as Keyword; }
        }

        internal object Property { get; set; }

        internal Constraints( object property , QueryPart QueryPart , T t )
        {
            this.QueryPart = QueryPart == QueryPart.NONE ? QueryPart.WHERE : QueryPart;
            this.Property = property;
            this.Current = t;
        }

        internal void AddFunction( FunctionCall call , object value )
        {
            Functions.Enqueue( new QueryFunction( call , value ) );
        }

        internal T AddExpression( Condition condition )
        {
            return AddExpression( condition , null );
        }

        internal T AddExpression( Condition condition , object value )
        {
            CurrentCriteria = Keyword.AddConditionExpression( Keyword.GetParameterName( Property ) , QueryPart , condition , value , Functions );
            return Current;
        }

        internal T AddExpression( Condition condition , CompositeValue value )
        {
          CurrentCriteria = Keyword.AddConditionExpression( Keyword.GetParameterName( Property ) ,
                                    QueryPart ,
                                    condition , value , Functions );

            return Current;
        }

        public T EqualsTo( P value )
        {
            return AddExpression( Condition.Equals , value );
        }

        public T LessThan( P value )
        {
            return AddExpression( Condition.LessThan , value );
        }

        public T GreaterThanOrEquals( P value )
        {
            return AddExpression( Condition.GreaterThanEqualsTo , value );
        }

        public T LessThanOrEquals( P value )
        {
            return AddExpression( Condition.LessThanEqualsTo , value );
        }

        public T GreaterThan( P value )
        {
            return AddExpression( Condition.GreaterThan , value );
        }

        public T NotEqualsTo( P value )
        {
            return AddExpression( Condition.NotEquals , value );
        }

        public T Between( P value1 , P value2 )
        {
            return AddExpression( Condition.Between , new CompositeValue( value1 , value2 ) );
        }

        public T NotIn( params P[] values )
        {
            return AddExpression( Condition.NOT_IN , new CompositeValue( new ArrayList( values ) ) );
        }

        public T In( params P[] values )
        {
            return AddExpression( Condition.IN , new CompositeValue( new ArrayList( values ) ) );
        }

        public T IsNotNull()
        {
            return AddExpression( Condition.IsNotNull );
        }

        public T IsNull()
        {
            return AddExpression( Condition.IsNull );
        }
    }
}
