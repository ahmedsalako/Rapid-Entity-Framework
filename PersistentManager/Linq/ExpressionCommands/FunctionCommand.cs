using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Query;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class FunctionCommand :  ExpressionCommand
    {
        internal virtual ProjectionFunction OperationType { get; set; }        

        internal FunctionCommand( CommandContext context , ProjectionFunction functionType , params NotifyCommandListeners[] listeners ) 
            : base( context , listeners )
        {
            OperationType = functionType;
            Context.Factory.MainFunctionType = OperationType;
        }

        internal override void Execute( )
        {
            if ( Context.CurrentCall.Arguments.Count > 1 )
            {
                if ( !( Context.CurrentCall.Arguments[1] is ConstantExpression ) )
                {
                    Context.Factory = ExpressionHandler.EvaluateMethodCall( Context.CurrentCall , Context.Factory , QueryPart.WHERE , this );
                }
            }            

            NotifyListeners( );
        }

        internal override int Priority
        {
            get
            {
                return 20;
            }
        }

        internal override void ValidateExpression()
        {
            throw new NotImplementedException();
        }
    }
}
