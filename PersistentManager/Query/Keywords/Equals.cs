using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Keywords
{
    public class Equals
    {
        PathExpression Relation { get; set; }
        string[] Left { get; set; }
        AS As { get; set; }

        internal Equals( PathExpressionFactory Path , PathExpression Relation , AS As , string left )
        {            
            this.Relation = Relation;
            this.Left = new [] { left };
            this.As = As;
        }

        internal Equals(PathExpressionFactory Path, PathExpression Relation, AS As, string[] left)
        {
            this.Relation = Relation;
            this.Left = left;
            this.As = As;
        }

        public AS EqualsTo( object right )
        {
            As.AddConditionExpression( Left[0] , QueryPart.WHERE , Condition.Equals , As.GetParameterName( right ) );         

            return As;
        }

        public AS EqualsTo( object[] right )
        {

            As.AddConditionExpression(Left, QueryPart.WHERE, Condition.Equals, As.GetParameterNames(right));

            return As;
        }
    }
}
