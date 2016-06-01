using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class WhereCommand : ExpressionCommand
    {
        internal QueryPart QueryPart { get; set; }
        internal WhereCommand( CommandContext context , params NotifyCommandListeners[] listeners ) 
            : base( context , listeners )
        {
            QueryPart = QueryPart.WHERE;
        }

        internal WhereCommand(CommandContext context, QueryPart queryPart )
            : this( context )
        {
            QueryPart = queryPart;
        }

        internal override void Execute()
        {
            if (!(Context.CurrentCall.Arguments[1] is ConstantExpression)) //We aint comparing constants. 5 == 5 && 6 == 7
            {
                Context.Factory = ExpressionHandler.EvaluateMethodCall(Context.CurrentCall, Context.Factory, QueryPart , this);
            }

            NotifyListeners();
        }

        internal override int Priority
        {
            get
            {
                return 3;
            }
        }

        internal override void ValidateExpression()
        {
            throw new NotImplementedException();
        }
    }
}
