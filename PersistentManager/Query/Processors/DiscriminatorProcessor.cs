using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    /// <summary>
    /// This class is part of the many processors which manipulates the PathExpressions and its Criterias.
    /// This class injects Dicriminator clauses into the PathExpression, and this will be injected into the
    /// generated SQL query.
    /// </summary>
    internal class DiscriminatorProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression MainPathExpression )
        {
            if ( MainPathExpression.MetaData.HasDiscriminator )
            {
                PropertyMetadata property = MainPathExpression.MetaData
                                           .GetDiscriminatorProperty( );

                Criteria criteria = CreateCriteria( QueryPart.WHERE , 
                                                    property.MappingName , 
                                                    property.DeclaringType ,
                                                    Condition.Equals ,
                                                    property.FieldValue
                                                  );
               
                MainPathExpression.AddCriteria( criteria );
                criteria.ApplyAlias( MainPathExpression );
            }
        }
    }
}
