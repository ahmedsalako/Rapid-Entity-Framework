using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Linq.Expressions;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class ExpressionCriteria
    {
        internal Criteria Criteria { get; set; }
        internal Expression Expression { get; set; }

        internal ExpressionCriteria( Expression Expression , Criteria Criteria )
        {
            this.Expression = Expression;
            this.Criteria = Criteria;
        }
    }
}
