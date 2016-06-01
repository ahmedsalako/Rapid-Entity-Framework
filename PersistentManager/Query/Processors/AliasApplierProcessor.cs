using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal class AliasApplierProcessor : PathExpressionProcessor
    {        
        protected AliasBuilder AliasBuilder { set; get; }

        internal AliasApplierProcessor( AliasBuilder AliasBuilder )
        {
            this.AliasBuilder = AliasBuilder;
        }

        internal override void Process( PathExpression pathExpression )
        {
            pathExpression.ALIAS = AliasBuilder.GetNextAlias( );
            pathExpression.OuterALIAS = AliasBuilder.GetNextAlias( );

            foreach ( Criteria criteria in pathExpression.Criterias )
            {
                criteria.ApplyAlias( pathExpression );
            }
        }
    }
}
