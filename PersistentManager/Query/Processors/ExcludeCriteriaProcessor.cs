using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal class ExcludeCriteriaProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {

            foreach( PropertyMetadata property in pathExpression.MetaData.GetAll( ) )
            {
                if( property.HasExcludedValue )
                {
                    Criteria criteria = CreateCriteria( QueryPart.WHERE ,
                                                        property.MappingName ,
                                                        property.DeclaringType ,
                                                        Condition.NotEquals ,
                                                        property.ExcludeValue
                                                      );

                    pathExpression.AddCriteria( criteria );
                    criteria.ApplyAlias( pathExpression );
                }
            }
        }
    }
}
