using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using PersistentManager.Linq.ExpressionCommands;

namespace PersistentManager.Linq
{
    internal class LinqToObjectSimulator : ExpressionVisitor
    {
        private IQueryable queryables;

        internal LinqToObjectSimulator( IQueryable queryables )
        {
            this.queryables = queryables;
        }

        internal Expression CopyAndModify( Expression expression )
        {
            return this.Visit( expression );
        }

        internal static object GetMethodAssignment( ExpressionCommand command , IList results )
        {
            MethodCallExpression methodCall = command.Context.CurrentCall;
            IQueryable queryables = results.AsQueryable( );

            if ( methodCall.Arguments.Count > 1 )
            {
                UnaryExpression arg1 = ( UnaryExpression )methodCall.Arguments.Last( );
                return methodCall.Method.Invoke( queryables , new object[] { queryables , arg1.Operand } );
            }

            return methodCall.Method.Invoke( queryables , new object[] { queryables } );
        }

        protected override Expression VisitMethodCall( MethodCallExpression m )
        {
            if ( m.Method.Name == "GroupBy" )
            {

            }

            return m;
        }

        protected override NewExpression VisitNew( NewExpression nex )
        {
            IEnumerable<Expression> args = this.VisitExpressionList( nex.Arguments );
            if ( args != nex.Arguments )
            {
                if ( nex.Members != null )
                    return Expression.New( nex.Constructor , args , nex.Members );
                else
                    return Expression.New( nex.Constructor , args );
            }

            return nex;
        }

        protected override Expression VisitConstant( ConstantExpression c )
        {
            return Expression.Constant( this.queryables );
        }

        internal override Expression VisitParameter( ParameterExpression p )
        {
            return p;
        }
    }
}
