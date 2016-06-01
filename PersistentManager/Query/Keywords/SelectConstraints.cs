using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace PersistentManager.Query.Keywords
{
    public sealed class SelectConstraints<TEntity> : Keyword
    {
        internal SelectConstraints( PathExpressionFactory path , AS As )
        {
            Path = path;
            Identifier = As;
        }

        public IList<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return SelectNew<TEntity, TResult>(selector, (TEntity)Path.QueryableItem).Execute();
        }

        public TResult Average<TResult>( Expression<Func<TEntity, TResult>> selector )
        {
            selector.Compile().Invoke((TEntity)Path.QueryableItem);

            AddProjectionExpression( 
                                QueryPart.SELECT , 
                                FunctionCall.Avg ,
                                GetParameterName( null ) 
                            );

            return new DeferedExecution<TResult>(GetSyntax()).First();
        }

        public int Count()
        {
            AddProjectionExpression(
                                QueryPart.SELECT,
                                FunctionCall.Count,
                                GetParameterName( null )
                            );

            return new DeferedExecution<int>(GetSyntax()).Count();
        }

        public TResult Min<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            selector.Compile().Invoke((TEntity)Path.QueryableItem);

            AddProjectionExpression(
                                QueryPart.SELECT,
                                FunctionCall.Min,
                                GetParameterName(null)
                            );

            return new DeferedExecution<TResult>(GetSyntax()).First();
        }

        public TResult Max<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            selector.Compile().Invoke((TEntity)Path.QueryableItem);

            AddProjectionExpression(
                                QueryPart.SELECT,
                                FunctionCall.Max,
                                GetParameterName(null)
                            );

            return new DeferedExecution<TResult>(GetSyntax()).First();
        }


    }
}
