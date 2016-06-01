using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class JoinCommand : ExpressionCommand
    {
        internal LambdaExpression Lambda { get; set; }
        internal ParameterExpression LeftParameter { get; set; }
        internal ParameterExpression RightParameter { get; set; }

        Expression LeftExpression { get; set; }
        Expression RigthExpression { get; set; }

        internal JoinCommand( CommandContext context , params NotifyCommandListeners[] listerners )
            : base( context , listerners )
        {
            //
        }

        internal override void Execute( )
        {
            Lambda = QueryHandler.GetLambdaExpression( Context.CurrentCall.Arguments.Last( ) );

            LeftParameter = Lambda.Parameters[0];
            RightParameter = Lambda.Parameters[1];

            LeftExpression = Context.CurrentCall.Arguments[2];
            RigthExpression = Context.CurrentCall.Arguments[3];

            ValidateExpression( );

            AddJoinClause( RetriveJoinCriteria( LeftExpression , QueryPart.JOIN_LEFT ) , RetriveJoinCriteria( RigthExpression , QueryPart.JOIN_RIGHT ) );

            NotifyListeners( );
        }

        internal object RetriveJoinCriteria( Expression expression , QueryPart queryPart )
        {
            return ExpressionHandler.EvaluateExpressionCriteria( expression , Context.Factory , queryPart , this );
        }

        internal void AddJoinClause( object value1 , object value2 )
        {
            if ( value1 is Criteria && value2 is Criteria )
            {
                ( ( Criteria )value1 ).JoinWith = ( ( Criteria )value2 ).Hash;
                ( ( Criteria )value1 ).JoinType = JoinType.LeftJoin;

                ( ( Criteria )value2 ).JoinWith = ( ( Criteria )value1 ).Hash;
                ( ( Criteria )value2 ).JoinType = JoinType.RightJoin;
            }
            else if ( value1 is Criteria )
            {
                ( ( Criteria )value1 ).Value = value2;
                ( ( Criteria )value1 ).Condition = Condition.Equals;
            }
            else if ( value2 is Criteria )
            {
                ( ( Criteria )value2 ).Value = value1;
                ( ( Criteria )value2 ).Condition = Condition.Equals;
            }
            else
            {
                return;
            }
        }

        internal override void ValidateExpression( )
        {
            if ( LeftExpression is ConstantExpression || RigthExpression is ConstantExpression )
            {
                throw new Exception( "A constant is not a valid join value " );
            }
        }

        internal override int Priority
        {
            get { return 1; }
        }
    }
}
