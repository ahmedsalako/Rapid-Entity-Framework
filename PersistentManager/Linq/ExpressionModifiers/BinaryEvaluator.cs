using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.Collections;

namespace PersistentManager.Linq
{
    internal class ExpressionAndType
    {
        internal ExpressionType ExpressionType { get; set; }
        internal Expression Expression { get; set; }

        internal ExpressionAndType( ExpressionType ExpressionType , Expression Expression )
        {
            this.ExpressionType = ExpressionType;
            this.Expression = Expression;
        }
    }

    internal class BinaryEvaluator : ExpressionVisitor
    {
        internal List<ExpressionAndType> BinaryExpressions = new List<ExpressionAndType>( );
        internal ExpressionType ExpressionType { get; set; }

        protected override Expression VisitBinary( BinaryExpression expression )
        {            
            if ( ( expression.Right is BinaryExpression ).IsNotTrue( ) ||
                ( expression.Left is BinaryExpression ).IsNotTrue( ) )
            {
                BinaryExpressions.Add( new ExpressionAndType( ExpressionType , expression ) );
                return expression;
            }

            ExpressionType = expression.NodeType;

            Visit( expression.Left );
            Visit( expression.Right );

            return expression;
        }

        internal List<ExpressionAndType> GetBinaryExpressions( BinaryExpression expression )
        {
            Visit( expression );
            return BinaryExpressions;
        }
    }
}
