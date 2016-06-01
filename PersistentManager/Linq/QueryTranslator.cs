using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Query.Keywords;
using PersistentManager.Query;
using PersistentManager.Descriptors;
using System.Collections;
using PersistentManager.Util;
using PersistentManager.Query.Sql;
using System.Reflection;
using PersistentManager.Linq.ExpressionCommands;

namespace PersistentManager.Linq
{
    internal class QueryTranslator : ExpressionVisitor
    {
        private List<ExpressionCommand> commands = new List<ExpressionCommand>( );
        private List<ExpressionCommand> delayedCommands = new List<ExpressionCommand>( );

        private PathExpressionFactory factory;
        private bool IsGroupedBy = false;
        private Type entityType;

        internal QueryTranslator( Type entityType )
        {
            this.entityType = entityType;
            factory = new PathExpressionFactory( entityType );
        }

        internal SyntaxContainer Translate( Expression expression )
        {
            this.Visit( new Normalizer().Normalize( Evaluator.PartialEval( expression ) ) );
            this.TransformExpressions( );
            
            factory.PageSize = factory.Offset + factory.PageSize;
            return SetReturnType( expression , factory.IndexAll( ) );       
        }

        internal SyntaxContainer DelayedTranslation()
        {
            this.TransformExpressions( true );
            return factory.IndexAll( );
        }

        internal List<ExpressionCommand> GetTranslatedCommands( Expression expression )
        {
            this.Visit( new Normalizer().Normalize( Evaluator.PartialEval( expression ) ) );
            this.TransformExpressions( );

            return Commands;
        }

        internal ExpressionCommand GetCommand<T>()
        {
            return Commands.FirstOrDefault(c => c is T );
        }

        internal bool HasCommand<T>()
        {
            return GetCommand<T>().IsNotNull();
        }

        internal List<ExpressionCommand> Commands
        {
            get { return commands; }
            set { commands = value; }
        }

        internal PathExpressionFactory Factory
        {
            get { return factory; }
            set { factory = value; }
        }

        private static Expression StripQuotes( Expression e )
        {
            while ( e.NodeType == ExpressionType.Quote )
            {
                e = ( ( UnaryExpression )e ).Operand;
            }
            return e;
        }

        private static Type StripType( Type type )
        {
            while ( type.IsGenericType && !type.IsCompilerGenerated() )
            {
                type = type.GetGenericArguments( )[0];
            }

            return type;
        }

        private void TransformExpressions( bool addDelayed )
        {
            if ( addDelayed )
            {
                commands.AddRange( delayedCommands );
            }

            TransformExpressions( );
        }

        private ExpressionCommand GetProjectionCommands( )
        {
            SelectCommand selectCommand = ( SelectCommand ) commands.FirstOrDefault( s => s is SelectCommand );
            SelectManyCommand selectManyCommand = ( SelectManyCommand ) commands.FirstOrDefault( s => s is SelectManyCommand );

            if ( null != selectCommand )
            {
                return selectCommand;
            }
            else if ( null != selectManyCommand )
            {
                return selectManyCommand;
            }

            return null;
        }

        private SyntaxContainer SetReturnType( Expression mainExpression , SyntaxContainer syntaxContainer )
        {
            ExpressionCommand projectionCommand = GetProjectionCommands();
            Type projection = mainExpression.Type;

            if ( null != projectionCommand )
            {
                LambdaExpression expression = ( LambdaExpression ) StripQuotes( projectionCommand.Context.CurrentCall.Arguments.Last( ) );

                if ( expression.Body is ParameterExpression )
                {
                    projection = StripType( expression.Body.Type );
                    syntaxContainer.ReturnType = projection;
                }
                else if ( expression.Body is MemberExpression )
                {
                    MemberExpression memberExpression = ( MemberExpression ) expression.Body;
                    projection = memberExpression.Type;

                    if ( memberExpression.Type.IsCollection( ) )
                    {
                        syntaxContainer.ResultIsCollection = true;
                        syntaxContainer.ReturnType = memberExpression.Type;
                    }                   
                    else if ( memberExpression.Type.IsClassOrInterface( ) )
                    {
                        syntaxContainer.ResultIsEntityClass = true;
                        syntaxContainer.ReturnType = memberExpression.Type;
                    }
                    else
                    {
                        syntaxContainer.ReturnType = memberExpression.Type;
                    }
                }
                else
                {
                    projection = StripType( expression.Body.Type );
                    syntaxContainer.ReturnType = projection;
                }
            }
            else
            {
                syntaxContainer.ReturnType = entityType;
            }

            if ( projection.IsCompilerGenerated( ) )
            {
                syntaxContainer.CompilerGeneratedResultType = projection;
                syntaxContainer.ResultIsCompilerGenerated = true;
                syntaxContainer.ReturnType = projection;
            }           

            return syntaxContainer;
        }

        private void TransformExpressions( )
        {
            if ( commands.Exists( c => c is JoinCommand ) )
            {
                if ( ! commands.Exists( c => c is SelectCommand ) )
                {
                    ExpressionCommand joinCommand = commands.FirstOrDefault( c => c is JoinCommand );
                    CommandContext context = new CommandContext 
                    { 
                        Expression = joinCommand.Context.CurrentCall.Arguments[4] , 
                        Factory = factory 
                    };

                    commands.Add( new SelectCommand( context ) );
                }
            }

            var orderedCommands = commands.OrderBy( command => command.Priority ).ToList( );

            orderedCommands.ForEach( c => c.Execute( ) );
        }

        private void AddDelayedCommand( ProjectionFunction functionType , MethodCallExpression expression )
        {
            CommandContext context = new CommandContext { CurrentCall = expression , Factory = factory };
            delayedCommands.Add( new FunctionCommand( context , functionType ) );
        }

        private void AddCommand( FunctionCommand command )
        {
            commands.Add( command );
        }

        private void AddCommand( ProjectionFunction functionType , MethodCallExpression expression )
        {
            CommandContext context = new CommandContext { CurrentCall = expression , Factory = factory };
            commands.Add( new FunctionCommand( context , functionType ) );
        }

        private void AddCommand( QueryPart part , MethodCallExpression expression )
        {
            CommandContext context = new CommandContext { CurrentCall = expression , Factory = factory };
            ExpressionCommand command = ExpressionCommand.EmptyCommand;

            switch ( part )
            {
                case QueryPart.WHERE:
                    command = new WhereCommand( context );
                    break;
                case QueryPart.JOIN:
                    command = new JoinCommand( context );
                    break;
                case QueryPart.JOINED_SUBQUERY:
                    command = new SelectManyCommand( context );
                    break;
                case QueryPart.GroupBy:
                    command = new GroupByCommand( context );
                    break;
                case QueryPart.ORDERBY:
                case QueryPart.ORDERBY_ASC:
                case QueryPart.ORDERBY_DESC:
                    Factory.ORDERBY = ( part == QueryPart.ORDERBY || part == QueryPart.ORDERBY_ASC ) ? ORDERBY.ASC : ORDERBY.DESC;
                    command = new OrderByCommand( context );
                    break;
                case QueryPart.SELECT:
                    if (commands.Count(s => s is SelectCommand) <= 0)
                    {
                        command = new SelectCommand(context);
                    }
                    break;
                default:
                    command = null;
                    break;

            }

            if ( command.IsNotNull( ) )
            {
                commands.Add( command );
            }
        }

        protected override Expression VisitMethodCall( MethodCallExpression methodCall )
        {
            if (    methodCall.Method.DeclaringType == typeof( Queryable )   || 
                    methodCall.Method.DeclaringType == typeof( Enumerable )  ||
                    methodCall.Method.DeclaringType == typeof( Normalizer )  
                )
            {
                switch ( methodCall.Method.Name )
                {
                    case "Select":                    
                        AddCommand( QueryPart.SELECT , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "Where":
                    case "TakeWhile":
                        AddCommand( QueryPart.WHERE , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;

                    case "OrderBy":
                    case "OrderByDescending":
                    case "ThenBy":
                    case "ThenByDescending":
                        QueryPart queryPart = ( methodCall.Method.Name.Contains("ByDescending") ) ? QueryPart.ORDERBY_DESC : QueryPart.ORDERBY_ASC;
                        AddCommand( queryPart , methodCall );
                        Visit( methodCall.Arguments[0] );
                        break;
                    case "Take":
                        factory.PageSize = ( int )GetMethodAssignment( methodCall.Arguments[1] );                        
                        Visit( ( MethodCallExpression )methodCall.Arguments[0] );
                        break;
                    case "ElementAt":
                    case "ElementAtOrDefault":
                        factory.PageSize = (int)GetMethodAssignment(methodCall.Arguments[1]);
                        factory.Offset = factory.PageSize <= 1 ? 1 : factory.PageSize - 1;                     
                        Visit((MethodCallExpression)methodCall.Arguments[0]);
                        break;
                    case "Skip":
                        factory.Offset = ( int )GetMethodAssignment( methodCall.Arguments[1] );
                        Visit( ( MethodCallExpression )methodCall.Arguments[0] );
                        break;
                    case "GroupBy":
                        AddCommand( QueryPart.GroupBy , methodCall );
                        Visit( methodCall.Arguments[0] );
                        IsGroupedBy = !( IsGroupedBy ) ? true : IsGroupedBy;
                        break;
                    case "Join":
                        AddCommand( QueryPart.JOIN , methodCall );
                        //Visit( methodCall.Arguments.Last() );
                        break;
                    case "SelectMany":
                        AddCommand( QueryPart.JOINED_SUBQUERY , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "Count":
                    case "LongCount":
                        AddCommand( ProjectionFunction.COUNT , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "Any":
                        AddCommand( ProjectionFunction.ANY , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "First":
                    case "FirstOrDefault":
                    case "Single":
                    case "SingleOrDefault":
                        AddCommand( ProjectionFunction.TOP , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "Reverse":
                        Factory.IsReversable = true;                    
                        this.Visit(methodCall.Arguments[0]);
                        break;
                    case "Last":
                    case "LastOrDefault":
                        AddCommand( ProjectionFunction.Last , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    case "Distinct":
                        Factory.IsDistinct = true;
                        this.Visit(methodCall.Arguments[0]);
                        break;
                    case "SkipWhile":
                        WhereCommand whereCmmand = new WhereCommand( 
                            new CommandContext { CurrentCall = methodCall, Factory = factory }, 
                            QueryPart.AND_NOT);

                        commands.Add(whereCmmand);
                        this.Visit(methodCall.Arguments[0]);
                        break;
                    case "All":
                        AddDelayedCommand( ProjectionFunction.ALL , methodCall );
                        this.Visit( methodCall.Arguments[0] );
                        Factory.MainFunctionType = ProjectionFunction.ALL;
                        break;
                    case "Average":
                    case "Min":
                    case "Max":
                    case "Sum":
                        AddCommand( new AggregateFunctionCommand( 
                                            new CommandContext 
                                                { 
                                                    CurrentCall = methodCall , 
                                                    Factory = factory 
                                                } , 
                                                ExpressionReader.GetFunctionType( methodCall.Method.Name ) ) 
                                  );
                        this.Visit( methodCall.Arguments[0] );
                        break;
                    default:
                        methodCall = ( MethodCallExpression )base.VisitMethodCall( methodCall );
                        break;
                }
            }
            return methodCall;
        }
    }
}
