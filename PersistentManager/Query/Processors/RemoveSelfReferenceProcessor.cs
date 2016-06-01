using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Processors
{
    internal class RemoveSelfReferenceProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            KeyValuePair<string , PathExpression> reference = pathExpression.References.FirstOrDefault( c => c.Value.UniqueId == pathExpression.UniqueId );

            if ( reference.IsNotNull( ) && !reference.Key.IsNullOrEmpty( ) )
            {
                pathExpression.References.Remove( reference );
            }

            if ( pathExpression.Base.IsNotNull( ) )
            {
                Process( pathExpression.Base );
            }
        }
    }
}
