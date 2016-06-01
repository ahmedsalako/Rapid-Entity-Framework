using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class SelectManyCommand : ExpressionCommand
    {
        internal LambdaExpression Lambda { get; set; }
        internal ParameterExpression LeftParameter { get; set; }
        internal ParameterExpression RightParameter { get; set; }

        Expression LeftExpression { get; set; }
        Expression RigthExpression { get; set; }

        internal SelectManyCommand(CommandContext context, params NotifyCommandListeners[] listeners)
            : base( context , listeners )
        {

        }

        internal override void Execute()
        {
            Lambda = QueryHandler.GetLambdaExpression(Context.CurrentCall.Arguments.Last());

            LeftParameter = Lambda.Parameters[0];
            RightParameter = Lambda.Parameters[1];

            if ( Context.CurrentCall.Arguments[1] is UnaryExpression )
            {
                Context.Factory = ExpressionHandler.EvaluateExpression( Context.CurrentCall.Arguments[1] , Context.Factory , QueryPart.SELECT , this );

                Criteria criteria = LastCriteriaExpression( );

                if ( criteria.IsNotNull( ) )
                {
                    criteria.QueryPart = QueryPart.NONE;
                }
            }

            Context.Factory = ExpressionHandler.EvaluateExpression( Lambda , Context.Factory , QueryPart.SELECT , this );

            NotifyListeners();
        }

        internal override Expression VisitParameter( ParameterExpression parameterExpression )
        {
            if ( MetaDataManager.IsPersistentable( parameterExpression.Type ) )
            {

            }

            return parameterExpression;
        }

        internal override int Priority
        {
            get { return 2; }
        }

        internal override void ValidateExpression()
        {
            throw new NotImplementedException();
        }
    }
}
