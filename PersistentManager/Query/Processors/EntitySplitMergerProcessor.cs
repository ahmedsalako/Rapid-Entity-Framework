using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal class EntitySplitMergerProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            List<PathExpression> deleted = new List<PathExpression>( );

            foreach ( PathExpression path in pathExpression.References.Values )
            {
                PropertyMetadata property = pathExpression.GetPropertyMetadata( path.Property );

                if ( property.IsEntitySplitJoin )
                {
                    Process( path );
                    deleted.Add( path );
                }
            }

            foreach ( PathExpression path in deleted )
            {
                pathExpression.AddCriterias( path.Criterias );
                pathExpression.References.Remove( path.Property );
            }
        }
    }
}
