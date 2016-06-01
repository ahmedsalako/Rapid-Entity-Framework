using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class ReverseCommand : FunctionCommand
    {
        internal ReverseCommand( CommandContext context ) 
            : this( context , ProjectionFunction.Reverse )
        {

        }

        internal ReverseCommand( CommandContext context , ProjectionFunction functionType , params NotifyCommandListeners[] listeners )
            : base( context , ProjectionFunction.Reverse , listeners )
        {

        }

        internal override void Execute( )
        {
            Context.Factory.IsReversable = true;
        }

        internal override int Priority
        {
            get
            {
                return 18;
            }
        }
    }
}
