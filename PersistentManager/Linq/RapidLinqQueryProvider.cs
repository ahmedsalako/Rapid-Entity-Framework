using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Query.Keywords;
using PersistentManager.Query;
using PersistentManager.Query.Sql;
using System.Collections;
using PersistentManager.Cache;
using System.Data.Common;
using System.Data;
using PersistentManager.Linq.ExpressionCommands;
using System.Reflection;
using PersistentManager.Metadata;
using PersistentManager.Descriptors;
using System.Runtime.CompilerServices;

namespace PersistentManager.Linq
{
    internal class RapidLinqQueryProvider<T> : RapidQueryProviderBase
    {
        internal Type projection;
        internal SyntaxContainer syntaxContainer;
        internal QueryTranslator translator;
        bool hasGroupBy = false;

        public override string GetQueryText(Expression expression)
        {
            return syntaxContainer.QueryString.ToString();
        }

        public override TResult CallInternalExecute<TResult>(Expression expression)
        {
            return (TResult) GetSingleResult<TResult>( ExecuteInternal( expression ) );
        }

        internal TResult GetSingleResult<TResult>( object result )
        {
            if ( result is IEnumerable && result.IsNotNull() && ! ( result is string ) )
            {
                if (((IList)result).Count > 0)
                {
                    return (TResult) DataType.ConvertValue(typeof(TResult) , ((IList)result)[0] );
                }

                return default(TResult);
            }

            return (TResult) DataType.ConvertValue(typeof(TResult) ,  result );
        }

        public override object Execute( Expression expression )
        {
            IList results = ExecuteInternal(expression) as IList;

            if ( syntaxContainer.Factory.IsReversable )
            {
                Type type = DynamicCast.MakeGenericType( typeof(IQueryable<>) , projection );
                IQueryable query = results.AsQueryable( );          

                MethodCallExpression reverseCallExpression = Expression.Call(
                    typeof( Queryable ) ,
                    "Reverse" ,
                    new Type[] { query.ElementType } ,
                    query.Expression );

                query = query.Provider.CreateQuery( reverseCallExpression );

                return query;
            }

            return results;
        }

        private  object ExecuteInternal( Expression expression )
        {           
            translator = GetTranslator( expression );
            syntaxContainer = translator.Translate( expression );
            projection = syntaxContainer.ReturnType;

            if ( syntaxContainer.Factory.ProjectionBinder.IsNotNull( ) && projection.IsClassOrInterface( ) )
            {
                syntaxContainer.Factory.ProjectionBinder.LeftAssignment = projection;
            }

            IList entities = ( IList ) ConcreteCollectionDiscovery
                .GenericCreate( typeof( List<> ) , projection );

            if ( syntaxContainer.FunctionType == ProjectionFunction.MAX )
            {
                return new DeferedExecution<T>( syntaxContainer ).FirstEntity( entities );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.MIN )
            {
                return new DeferedExecution<T>( syntaxContainer ).FirstEntity( entities );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.SUM )
            {
                return new DeferedExecution<T>( syntaxContainer ).FirstEntity( entities );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.AVG )
            {
                return new DeferedExecution<T>( syntaxContainer ).FirstEntity( entities );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.COUNT )
            {
                return new DeferedExecution<T>( syntaxContainer ).Count( );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.ANY )
            {
                return new DeferedExecution<T>( syntaxContainer ).Any( );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.TOP )
            {
                return new DeferedExecution<T>( syntaxContainer ).FirstEntity( entities );
            }
            else if ( syntaxContainer.FunctionType == ProjectionFunction.ALL )
            {
                int sequenceCount = new DeferedExecution<T>( syntaxContainer ).Count( );

                syntaxContainer = translator.DelayedTranslation();

                int conditionedCount = new DeferedExecution<T>( syntaxContainer ).Count( );

                return sequenceCount == conditionedCount;
            }
            else if (syntaxContainer.FunctionType == ProjectionFunction.Last )
            {
                int count = new DeferedExecution<T>(syntaxContainer).Count();

                SyntaxContainer syntax = GetTranslator(expression).Translate( expression );
                syntax.Range( count - 1 , count );
                syntax.ResultIsCompilerGenerated = syntaxContainer.ResultIsCompilerGenerated;
                syntax.CompilerGeneratedResultType = syntaxContainer.CompilerGeneratedResultType;

                return new DeferedExecution<T>( syntax ).ExecuteCompilerGenerated(entities);
            }
            else if ( projection.IsCompilerGenerated( ) )
            {
                return new DeferedExecution<T>( syntaxContainer ).ExecuteCompilerGenerated( entities );
            }
            else if ( projection.IsValueType || projection == typeof( string ) )
            {
                return new DeferedExecution<T>( syntaxContainer ).ExecuteCompilerGenerated( entities );
            }
            else if ( syntaxContainer.HasProjectionBinding )
            {
                return new DeferedExecution<T>( syntaxContainer ).ExecuteCompilerGenerated( entities );
            }
            else if ( projection == typeof( T ) )
            {
                return new DeferedExecution<T>( syntaxContainer ).Execute( );
            }
            else
            {
                return new DeferedExecution<T>( syntaxContainer ).ExecuteCompilerGenerated( entities );
            }
        }

        private QueryTranslator GetTranslator( Expression expression )
        {
            return new QueryTranslator(typeof(T));
        }
    }
}
