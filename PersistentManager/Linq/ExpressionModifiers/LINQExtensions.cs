using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Descriptors;
using PersistentManager.Query;

namespace PersistentManager.Linq
{
    internal static class LINQExtensions
    {
        internal static bool IsParameter( this Expression expression )
        {
            return expression is ParameterExpression;
        }

        internal static bool IsMethodCallExpression( this Expression expression )
        {
            return expression is MethodCallExpression;
        }

        internal static bool IsConstantOrUnary( this Expression one )
        {
            return ( one.IsConstantExpression( ) || one.IsUnaryExpression( ) );
        }

        internal static IEnumerable<Expression> Enumerate( params Expression[] expressions )
        {
            foreach ( Expression current in expressions )
            {
                yield return current;
            }
        }

        internal static bool IsUnaryExpression( this Expression one )
        {
            return ( one is UnaryExpression );
        }

        internal static bool IsConstantExpression( this Expression one )
        {
            return ( one is ConstantExpression );
        }

        internal static bool IsMemberExpression( this Expression one )
        {
            return one is MemberExpression;
        }

        internal static bool AreMemberExpressions( this Expression one , Expression two )
        {
            return ( one.IsMemberExpression( ) && two.IsMemberExpression( ) );
        }

        internal static bool IsLeftAndRightBinaryExpression(this BinaryExpression expr)
        {
            return (expr.Left is BinaryExpression && expr.Right is BinaryExpression) ;
        }

        internal static FunctionCall GetCombiningFunction( this ExpressionType expressionType )
        {
            if ( expressionType.IsCombinigOperator( ) )
            {
                switch ( expressionType )
                {
                    case ExpressionType.Add:
                        return FunctionCall.Add;
                    case ExpressionType.Subtract:
                        return FunctionCall.Subtract;
                    case ExpressionType.Multiply:
                        return FunctionCall.Multiply;
                    case ExpressionType.Divide:
                        return FunctionCall.Division;
                    default:
                        return FunctionCall.None;
                }
            }

            return FunctionCall.None;
        }

        internal static bool IsCombinigOperator( this ExpressionType expressionType )
        {
            return ( expressionType == ExpressionType.Add ||
                    expressionType == ExpressionType.Multiply ||
                    expressionType == ExpressionType.Subtract || 
                    expressionType == ExpressionType.Divide ||
                    expressionType == ExpressionType.Coalesce );
        }

        internal static bool IsCompoundOperator( this ExpressionType expressionType )
        {
            return ( expressionType == ExpressionType.And ||
                    expressionType == ExpressionType.AndAlso ||
                    expressionType == ExpressionType.Or || 
                    expressionType == ExpressionType.OrElse );
        }
    }
}
