using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace PersistentManager.Linq
{
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval( Expression expression , Func<Expression , bool> fnCanBeEvaluated )
        {
            return new SubtreeEvaluator( new Nominator( fnCanBeEvaluated ).Nominate( expression ) ).Eval( expression );
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval( Expression expression )
        {
            var result = PartialEval( expression , Evaluator.CanBeEvaluatedLocally );
            return result;
        }

        private static bool CanBeEvaluatedLocally( Expression expression )
        {
            if ( expression is MethodCallExpression )
            {
                MethodCallExpression methodCall = expression as MethodCallExpression;
                if ( (methodCall.Method.Name == "FirstOrDefault" || 
                    methodCall.Method.Name == "First") && methodCall.Arguments.Count <= 1 )
                {
                    return false;
                }
            }

            switch ( expression.NodeType )
            {
                case ExpressionType.Convert:
                case ExpressionType.Parameter:
                    return false;
                default:
                    return true;
            }
        }
    }
}
