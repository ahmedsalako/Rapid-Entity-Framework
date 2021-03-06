﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace PersistentManager.Linq
{
    /// <summary>
    /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
    /// </summary>
    internal class SubtreeEvaluator : ExpressionVisitor
    {
        HashSet<Expression> candidates;

        internal SubtreeEvaluator( HashSet<Expression> candidates )
        {
            this.candidates = candidates;
        }

        internal Expression Eval( Expression exp )
        {
            return this.Visit( exp );
        }

        protected override Expression Visit( Expression exp )
        {
            if ( exp == null )
            {
                return null;
            }
            if ( this.candidates.Contains( exp ) )
            {
                return this.Evaluate( exp );
            }
            return base.Visit( exp );
        }

        private Expression Evaluate( Expression e )
        {
            if ( e.NodeType == ExpressionType.Constant )
            {
                return e;
            }

            if ( e.NodeType == ExpressionType.Convert )
            {
                return ( e as UnaryExpression ).Operand;
            }
            LambdaExpression lambda = Expression.Lambda( e );
            Delegate fn = lambda.Compile( );
            return Expression.Constant( fn.DynamicInvoke( null ) , e.Type );
        }
    }
}
