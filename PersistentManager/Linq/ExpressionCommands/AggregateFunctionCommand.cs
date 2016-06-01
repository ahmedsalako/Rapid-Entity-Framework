using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Query;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class AggregateFunctionCommand : FunctionCommand
    {
        internal AggregateFunctionCommand( CommandContext context , ProjectionFunction functionType , params NotifyCommandListeners[] listeners ) 
            : base( context , functionType , listeners )
        {
            OperationType = functionType;
        }

        internal override void Execute( )
        {
            if ( Context.CurrentCall.Arguments.Count > 1 )
            {
                if ( !( Context.CurrentCall.Arguments[1] is ConstantExpression ) )
                {
                    Context.Factory.SelectArguments.Clear( );
                    Context.Factory.Main.RemoveFromTree( QueryPart.SELECT ); 

                    Criteria criteria = ExpressionHandler.EvaluateExpressionCriteria
                                            ( 
                                                Context.CurrentCall.Arguments[1] , 
                                                Context.Factory , 
                                                QueryPart.SELECT , this 
                                             );

                    criteria.RemoveAggregateFunctions( );
                                                     
                    criteria.AddFunction( new QueryFunction( ExpressionReader.GetFunctionType( OperationType ) , Null.NOTHING ) );                    
                }
            }

            Context.Factory.ProjectionBinder = null;
            Context.Factory.MainFunctionType = OperationType;

            NotifyListeners( );
        }

        internal override int Priority
        {
            get
            {
                return 9;
            }
        }

        internal override void ValidateExpression()
        {
            throw new NotImplementedException();
        }
    }
}
