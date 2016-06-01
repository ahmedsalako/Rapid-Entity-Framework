using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Linq.ExpressionCommands;

namespace PersistentManager.Descriptors
{
    internal class NameResolver
    {
        internal string CanonicalName { get; set; }
        internal Criteria Criteria { get; set; }
        internal Expression Projection { get; set; }
        internal string Name { get; set; }

        internal NameResolver( string CanonicalName , string Name , MemberExpression Projection )
        {
            this.CanonicalName = CanonicalName;
            this.Projection = Projection;
            this.Name = Name;
        }

        internal NameResolver( string CanonicalName , string Name , Expression expression )
        {
            this.CanonicalName = CanonicalName;
            this.Projection = expression;
            this.Name = Name;
        }

        internal Expression GetExpression( )
        {
            return Projection;
        }
    }
}
