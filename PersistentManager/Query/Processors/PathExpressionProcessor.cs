using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal abstract class PathExpressionProcessor
    {        
        protected Criteria CreateCriteria( QueryPart queryPart , string name , Type declaringType , Condition condition , object value )
        {
            return Criteria.CreateCriteria( queryPart , name , condition , value , ProjectionFunction.NOTSET , declaringType );
        }

        internal abstract void Process( PathExpression pathExpression );
    }
}
