using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;
using PersistentManager.Descriptors;
using PersistentManager.Linq.ExpressionCommands;

namespace PersistentManager.Linq
{
    internal class ExpressionHandler : QueryHandler
    {
        private ExpressionHandler( MethodCallExpression Method )
        {
            Lambda = GetLambdaExpression( Method.Arguments[1] );
            ReturnType = Lambda.Type.GetGenericArguments()[1];
            EntityType = Lambda.Parameters[0].Type;

            NormalizedLambda = (LambdaExpression)new Normalizer().Normalize(Lambda);
            
        }

        private ExpressionHandler( Expression expression )
        {
            Lambda = GetLambdaExpression( expression );

            ReturnType = Lambda.Type.GetGenericArguments()[1];
            EntityType = Lambda.Parameters[0].Type;

            NormalizedLambda = (LambdaExpression)new Normalizer().Normalize( Lambda );
        }

        internal static PathExpressionFactory EvaluateMethodCall( MethodCallExpression Method , PathExpressionFactory query , QueryPart queryPart , ExpressionCommand command)
        {
            return new ExpressionHandler( Method ).Evaluate( query , queryPart , command);
        }

        internal static Criteria EvaluateExpressionCriteria( Expression expression , PathExpressionFactory query , QueryPart queryPart , ExpressionCommand command )
        {
            return new ExpressionHandler( expression ).EvaluateCriteria( query , queryPart , command ).CurrentCriteria;
        }

        internal static PathExpressionFactory EvaluateExpression( Expression expression , PathExpressionFactory query , QueryPart queryPart , ExpressionCommand command )
        {
            new ExpressionHandler( expression ).EvaluateCriteria( query , queryPart , command );

            return query;
        }
    }
}
