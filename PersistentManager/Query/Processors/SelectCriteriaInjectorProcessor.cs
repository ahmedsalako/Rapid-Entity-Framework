using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Query.Processors
{
    internal class SelectCriteriaInjectorProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            if ( pathExpression .IsMain && !HasSelectProjection( pathExpression ) )
            {
                pathExpression.CreateCompositeSelectAll
                ( 
                    string.Empty , 
                    pathExpression.Type , 
                    CompositeType.EntityClass , 
                    0 
                );
            }            
        }

        internal bool HasSelectProjection( PathExpression pathExpression )
        {
            foreach ( PathExpression path in AllExpressions( pathExpression ) )
            {
                if ( path.HasSelectProjections( ) )
                {
                    return true;
                }
            }

            return false;
        }

        protected IEnumerable<PathExpression> AllExpressions( PathExpression pathExpression )
        {
            if ( pathExpression.HasBase )
            {
                foreach ( var expression in AllExpressions( pathExpression.Base ) )
                {
                    yield return expression;
                }
            }

            foreach ( PathExpression path in pathExpression.References.Values.Where( p => p.UniqueId != pathExpression.UniqueId ) )
            {
                foreach ( var expression in AllExpressions( path ) )
                {
                    yield return expression;
                }
            }

            foreach ( PathExpression path in pathExpression.Joins.Values )
            {
                foreach ( var expression in AllExpressions( path ) )
                {
                    yield return expression;
                }
            }

            foreach ( PathExpression path in pathExpression.Embeddeds.Values )
            {
                foreach ( var expression in AllExpressions( path ) )
                {
                    yield return expression;
                }
            }

            yield return pathExpression;
        }
    }
}
