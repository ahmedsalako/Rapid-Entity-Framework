using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Keywords
{
    public class ON
    {
        AS As { get; set; }
        PathExpression Relation { get; set; }

        internal ON( PathExpressionFactory Path , PathExpression Relation , AS As )
        {
            this.Relation = Relation;
            this.As = As;
        }

        public Equals On( object name )
        {
            return new Equals( As.Path , Relation , As , As.GetParameterName( name ) );
        }

        public Equals On( object[] names )
        {
            return new Equals(As.Path, Relation, As, As.GetParameterNames(names));
        }
    }
}
