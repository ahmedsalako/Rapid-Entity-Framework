using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class OrderByCommand : ExpressionCommand
    {
        internal OrderByCommand( CommandContext context , params NotifyCommandListeners[] listerners )
            : base( context , listerners )
        {
            //
        }

        internal override void Execute( )
        {
            if ( !( Context.CurrentCall.Arguments[1] is ConstantExpression ) ) //We aint ordering by constant like 5
            {
                Context.Factory = ExpressionHandler.EvaluateMethodCall( Context.CurrentCall , Context.Factory , QueryPart.ORDERBY , this );
            }
            NotifyListeners( );
        }

        internal override int Priority
        {
            get { return 5; }
        }

        internal override void ValidateExpression( )
        {
            throw new NotImplementedException( );
        }
    }
}
