using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using PersistentManager.Query;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;
using PersistentManager.Query.Keywords;
using PersistentManager.Linq.ExpressionCommands;
using PersistentManager.Query.Projections;
using System.Collections.ObjectModel;
using PersistentManager.Linq.ExpressionModifiers;
using PersistentManager.Metadata;

namespace PersistentManager.Linq
{
    internal class QueryHandler : ExpressionVisitor
    {
        private CriteriaStack<Criteria> CriteriaStack = new CriteriaStack<Criteria>( );
        private Condition CurrentCondition = Condition.NOTSET;
        private bool OriginalCheck { get; set; }
        private PathExpressionFactory factory;
        private QueryPart CurrentQueryPart;                

        internal ExpressionCommand ExpressionCommand { get; set; }
        internal LambdaExpression NormalizedLambda { get; set; }
        internal PathExpressionFactory Current { get; set; }
        internal Criteria CurrentCriteria { get; set; }
        internal LambdaExpression Lambda { get; set; }
        internal object CurrentConstant { get; set; }
        internal bool IsCorreleted { get; set; }
        public Type ReturnType { get; set; }
        public Type EntityType { get; set; }

        internal static LambdaExpression GetLambdaExpression( Expression expression )
        {
            return ( LambdaExpression ) ExpressionReader.StripQuotes( expression );
        }

        protected override Expression VisitMethodCall( MethodCallExpression methodCall )
        {
            if ( methodCall.Method.Name == "GetDateValue" )
            {
                string argument = ( ( ConstantExpression ) methodCall.Arguments[1] ).Value as string;
                FunctionCall functionCall = ExpressionReader.GetFunction( argument );

                ExpressionCommand.Functions.Enqueue( new QueryFunction( functionCall , argument ) );

                Visit( methodCall.Arguments[0] );
            }
            else if ( methodCall.Method.Name == "ToUpper" || methodCall.Method.Name == "ToLower"
                || methodCall.Method.Name == "Trim" || methodCall.Method.Name == "TrimEnd"
                || methodCall.Method.Name == "TrimStart"
                || methodCall.Method.Name == "GetStringLength" )
            {
                FunctionCall functionCall = ExpressionReader.GetFunction( methodCall.Method.Name );

                ExpressionCommand.Functions.Enqueue( new QueryFunction( functionCall , null ) );

                if ( methodCall.Object.IsNotNull( ) )
                {
                    Visit( methodCall.Object );
                }
                else
                {
                    Visit( methodCall.Arguments[0] );
                }
            }
            else if ( ( methodCall.Method.Name == "CompareTo" || methodCall.Method.Name == "Equals"
                || methodCall.Method.Name == "StartsWith" || methodCall.Method.Name == "Contains"
                || methodCall.Method.Name == "EndsWith" ) && methodCall.Object.Type == typeof( string ) )
            {
                Visit( methodCall.Object );

                Criteria criteria = CurrentCriteria;
                criteria.Condition = ExpressionReader.GetStringManipulationCondition( methodCall.Method.Name );
                criteria.Value = GetMethodAssignment( methodCall.Arguments[0] );
                criteria.AddFunction( ExpressionCommand.Functions );
                CriteriaStack.GetCurrent();
            }
            else if (
                !ExpressionReader.StripQuotes( methodCall.Arguments[0] ).Type.IsGroupingType( )
                && ExpressionReader.StripQuotes( methodCall.Arguments[0] ).Type.GetInterface( "IEnumerable" ) == typeof( IEnumerable ) )
            {
                Type genericArgument = ExpressionReader.StripQuotes( methodCall.Arguments[0] ).Type.GetGenericArguments( )[0];
                IsCorreleted = true;
                bool IsQueryExpression = methodCall.Method.ReturnType.Implements( typeof( IQueryable ) );
                Criteria leftJoin = null;
                Criteria rightJoin = null;
                PropertyMetadata property = null;

                QueryTranslator translator = new QueryTranslator(genericArgument);
                List<ExpressionCommand> commands = translator.GetTranslatedCommands(methodCall);
                ProjectionFunction functionType = ExpressionReader.GetFunctionType(methodCall.Method.Name);

                if ( !IsQueryExpression )
                {
                    MemberExpression root = ExpressionReader.GetImmediateMember( ExpressionReader.StripQuotes( methodCall.Arguments[0] ) );

                    property = EntityMetadata.GetMappingInfo( root.Expression.Type )
                                                .GetPropertyMappingIncludeBase( root.Member.Name );

                    foreach (JoinMetadata join in property.JoinDetails)
                    {
                        leftJoin = new Criteria();
                        leftJoin.Name = property.IsManyToMany ? join.LeftKey : join.JoinColumn;
                        leftJoin.DeclaringType = property.DeclaringType;
                        leftJoin.ReflectedType = root.Expression.Type;
                        leftJoin.QueryPart = QueryPart.JOIN_LEFT;

                        rightJoin = new Criteria();
                        rightJoin.Name = property.IsManyToMany ? join.OwnerColumn : join.RelationColumn;
                        rightJoin.DeclaringType = property.IsManyToMany ? property.JoinTableType : property.DeclaringType;
                        rightJoin.ReflectedType = property.IsManyToMany ? property.JoinTableType : root.Expression.Type;
                        rightJoin.QueryPart = QueryPart.JOIN_RIGHT;
                        ExpressionReader.JoinCriteria(leftJoin, rightJoin);


                        ExpressionCommand.AddCriteriaExpression(new ExpressionCriteria(root.Expression, leftJoin));

                        if (property.IsManyToMany)
                        {
                            Criteria leftJoin2 = new Criteria();
                            leftJoin2.Name = join.RightKey;
                            leftJoin2.DeclaringType = genericArgument;
                            leftJoin2.ReflectedType = genericArgument;
                            leftJoin2.QueryPart = QueryPart.WHERE;

                            Criteria rightJoin2 = new Criteria();
                            rightJoin2.Name = join.JoinColumn;
                            rightJoin2.DeclaringType = property.JoinTableType;
                            rightJoin2.ReflectedType = property.JoinTableType;
                            rightJoin2.QueryPart = QueryPart.WHERE;
                            ExpressionReader.JoinCriteria(leftJoin2, rightJoin2);

                            PathExpression pathExpression = new PathExpression(false, property.JoinTableType);
                            pathExpression.AddCriteria(rightJoin);
                            pathExpression.AddCriteria(rightJoin2);

                            translator.Factory.Main.AddCriteria(leftJoin2);
                            translator.Factory.Main.AddJoin(pathExpression);
                        }
                        else
                        {
                            translator.Factory.Main.AddCriteria(rightJoin);
                        }
                    }
                }

                ExpressionReader.CreateSelectParameterForParameterlessCorrelation( CurrentQueryPart ,
                                                                                   translator ,
                                                                                   functionType ,
                                                                                   property
                                                                                  );

                foreach ( ExpressionCommand command in commands )
                {
                    if ( command.Context.CurrentCall.Arguments.Count > 1 )
                    {
                        Expression expression = ExpressionReader.StripQuotes( command.Context.CurrentCall.Arguments[1] );

                        if ( expression is LambdaExpression )
                        {
                            LambdaExpression lambdaExpression = ( LambdaExpression ) expression;
                            ParameterExpression parameter = lambdaExpression.Parameters[0];

                            foreach ( ExpressionCriteria expressionCriteria in command.criteriaExpressions )
                            {
                                if ( !ExpressionReader.IsFromParameter( expressionCriteria.Expression , lambdaExpression.Parameters[0] ) )
                                {
                                    expressionCriteria.Criteria.IsProjected = true;
                                    expressionCriteria.Criteria.Value = string.Empty;
                                    expressionCriteria.Criteria.QueryPart = QueryPart.SELECT;
                                    translator.Factory.Main.RemoveFromTree( expressionCriteria.Criteria.Hash );
                                    translator.Factory.Main.RemovePathExpressionFromTree( expressionCriteria.Criteria.ContainerGuid );
                                    ExpressionCommand.AddCriteriaExpression( expressionCriteria );
                                }
                            }
                        }
                    }
                }

                ExpressionReader.CreateSelectCorrelation( factory , translator.Factory , CurrentQueryPart , leftJoin , ExpressionCommand.CriteriaOrdinal );

                if ( CurrentQueryPart != QueryPart.SELECT )
                {
                    Criteria mainCriteria = Criteria.CreateCriteria( QueryPart.WHERE , string.Empty , ProjectionFunction.NOTSET , null );
                    mainCriteria.CorrelatedPath = translator.Factory;
                    mainCriteria.Condition = CurrentCondition;
                    factory.Main.AddCriteria( mainCriteria );
                    CriteriaStack.AddCriteria( mainCriteria );
                }
            }
            else if ( methodCall.Method.Name == "Average" || methodCall.Method.Name == "Max" || methodCall.Method.Name == "Min" || methodCall.Method.Name == "Sum" || methodCall.Method.Name == "Count" )
            {
                if ( CurrentQueryPart != QueryPart.WHERE )
                {
                    FunctionCall functionCall = ExpressionReader.GetFunction( methodCall.Method.Name );

                    ExpressionCommand.Functions.Enqueue( new QueryFunction( functionCall , null ) );
                }

                Visit( ( Expression ) methodCall.Arguments.Last( ) );
            }
            else if ( methodCall.Method.ReturnType == typeof( String ) || methodCall.Method.Name == "ToString" )
            {
                return methodCall;
            }
            else if ( methodCall.Method.ReturnType == typeof( String ) || methodCall.Method.ReturnType.IsValueType )
            {
                throw new Exception( string.Format( "Rapid does not currently support this method: {0}" , methodCall.Method.Name ) );
            }
            else
            {
                methodCall = ( MethodCallExpression ) base.VisitMethodCall( methodCall );
            }

            return methodCall;
        }

        protected override Expression VisitUnary( UnaryExpression u )
        {
            switch ( u.NodeType )
            {
                case ExpressionType.Not:
                    this.Visit( u.Operand );
                    break;
                default:
                    this.Visit( u.Operand );
                    break;
            }
            return u;
        }

        internal Expression HandleBinaryExpression( BinaryExpression binaryExpression )
        {
            CurrentCondition = ExpressionReader.GetCondition( binaryExpression.NodeType );       

            this.Visit( binaryExpression.Left );

            Criteria leftCriteria = CriteriaStack.GetCurrent( );

            if ( leftCriteria.IsNull( ) && binaryExpression.NodeType.IsCombinigOperator( ) )
            {
                leftCriteria = ExpressionCommand.LastCriteriaExpression( );
            }

            this.Visit( binaryExpression.Right );

            Criteria rightCriteria = CriteriaStack.GetCurrent( );

            if ( IsCorreleted )
            {
                if ( leftCriteria.IsNotNull( ) )
                {
                    leftCriteria.Position = Operand.Left;
                }
                else if ( rightCriteria.IsNotNull( ) )
                {
                    rightCriteria.Position = Operand.Right;
                }
            }

            if ( leftCriteria.IsNotNull( ) && rightCriteria.IsNotNull( ) )
            {
                ExpressionReader.JoinCriteria( leftCriteria , rightCriteria );
                leftCriteria.AddFunction( ExpressionReader.GetFunction( binaryExpression.NodeType , null , rightCriteria ) );
            }
            else if ( leftCriteria.IsNotNull( ) && binaryExpression.Right.IsConstantOrUnary() )
            {
                leftCriteria.Condition = ExpressionReader.GetCondition( CurrentCondition , CurrentConstant );
                leftCriteria.Value = CurrentConstant;
                leftCriteria.AddFunction( ExpressionReader.GetFunction( binaryExpression.NodeType , CurrentConstant , rightCriteria ) );
            }
            else if ( rightCriteria.IsNotNull( ) && binaryExpression.Left.IsConstantOrUnary() )
            {
                rightCriteria.Condition = ExpressionReader.GetCondition( CurrentCondition , CurrentConstant );
                rightCriteria.Value = CurrentConstant;
                rightCriteria.AddFunction( ExpressionReader.GetFunction( binaryExpression.NodeType , CurrentConstant , rightCriteria ) );
            }

            IsCorreleted = false;
            Current = null;
            return binaryExpression;
        }

        protected override Expression VisitBinary( BinaryExpression b )
        {
            if ( b.NodeType.IsCompoundOperator( ) )
            {               
                QueryPart currentCopy = CurrentQueryPart;

                if ( b.IsLeftAndRightBinaryExpression() )
                {
                    if ( b.Left is BinaryExpression )
                    {
                        Visit( b.Left );
                        CurrentQueryPart = ExpressionReader.GetQueryPartByExpressionType( b.NodeType , currentCopy );                
                    }

                    if ( b.Right is BinaryExpression )
                    {
                        Visit( b.Right );
                        CurrentQueryPart = ExpressionReader.GetQueryPartByExpressionType( b.NodeType , currentCopy );
                    }
                }
                else
                {
                    if ( b.Left is BinaryExpression )
                    {
                        Visit( b.Left );
                        CurrentQueryPart = ExpressionReader.GetQueryPartByExpressionType( b.NodeType , currentCopy );
                        Visit( b.Right );
                    }
                    else
                    {
                        Visit( b.Left );
                        CurrentQueryPart = ExpressionReader.GetQueryPartByExpressionType(b.NodeType, currentCopy);
                        Visit( b.Right );
                    }                 
                }
                
                CurrentQueryPart = currentCopy;
                return b;
            }

            return HandleBinaryExpression( b );
        }

        protected override Expression VisitConstant( ConstantExpression constant )
        {
            IQueryable queryable = constant.Value as IQueryable;

            if ( !queryable.IsNull( ) )
            {

            }
            else
            {
                CurrentConstant = constant.Value;
            }

            return constant;
        }

        protected override IEnumerable<MemberBinding> VisitBindingList( ReadOnlyCollection<MemberBinding> memberBindings )
        {
            if ( CurrentQueryPart == QueryPart.SELECT )
            {
                return ( ( SelectCommand ) ExpressionCommand ).MakeProjection( base.VisitBindingList( memberBindings ) , false );
            }
            else
            {
                return base.VisitBindingList( memberBindings );
            }
        }

        internal override Expression VisitParameter( ParameterExpression p )
        {
            return ExpressionCommand.VisitParameter( p );
        }

        protected override Expression VisitMemberAccess( MemberExpression member )
        {
            Expression root = ExpressionReader.GetRootExpression( member );

            if ( root is ParameterExpression && root.Type.IsGroupingType( ) )
            {
                foreach ( NameResolver resolver in factory.GroupBys )
                {
                    if ( member.Member.Name == resolver.CanonicalName )
                    {
                        Visit( resolver.GetExpression( ) );
                    }
                    else if ( member.Member.Name == "Key" )
                    {
                        Visit( resolver.GetExpression( ) );
                    }
                }
                OriginalCheck = false;
                return member;
            }

            Expression expression = OriginalCheck ? member : ExpressionCommand.GetOriginalMemberExpression( member );

            if ( expression is MemberExpression )
            {
                if ( member.Expression.IsNull( ) )
                    return member;

                if ( member.Expression is MemberExpression )
                {
                    if ( ( ( MemberExpression ) member.Expression ).Expression.NodeType == ExpressionType.Constant )
                        return member;
                }

                if ( ExpressionReader.GetRootExpression( expression ).Type.IsGroupingType( ) )
                {
                    Visit( expression );
                }

                MemberExpression member2 = expression as MemberExpression;

                if ( member2.Member is PropertyInfo && ( MetaDataManager.IsPersistentable( member2.Expression.Type )
                    || MetaDataManager.HasPersistentProperty( member2.Expression.Type ) ) )
                {
                    OriginalCheck = false;

                    CurrentCriteria = new Criteria( );
                    CurrentCriteria.Name = ExpressionReader.GetMemberName( ( PropertyInfo ) member2.Member );
                    CurrentCriteria.QueryPart = CurrentQueryPart;
                    CurrentCriteria.DeclaringType = ExpressionReader.GetDeclaringType( member2.Member.Name , member2.Expression.Type );
                    CurrentCriteria.ReflectedType = ExpressionReader.GetReflectedType( member2.Member.Name , member2.Expression.Type );
                    CurrentCriteria.Condition = CurrentCondition;
                    CurrentCriteria.AddFunction( ExpressionCommand.Functions );

                    if ( member2.Expression is MemberExpression )
                    {
                        CurrentCriteria.OwnerPropertyName = ( ( MemberExpression ) member2.Expression ).Member.Name;
                    }

                    ExpressionCommand.AddCriteriaExpression( new ExpressionCriteria( member2.Expression , CurrentCriteria ) );

                    AddCriteria( CurrentCriteria , member2.Member.Name );

                    return member2;
                }
            }
            else
            {
                OriginalCheck = true;
                Visit( expression );
            }

            return member;
        }

        private void AddCriteria( Criteria criteria , string memberName )
        {
            if ( criteria.QueryPart == QueryPart.SELECT )
            {
                criteria.Ordinal = ExpressionCommand.CriteriaOrdinal;
                factory.AddSelectArgument( criteria );
            }
            else if ( criteria.QueryPart == QueryPart.GroupBy )
            {
                factory.AddGroupBy( memberName , criteria );
            }

            CriteriaStack.AddCriteria( criteria );
        }

        protected PathExpressionFactory Evaluate( PathExpressionFactory query , QueryPart queryPart , ExpressionCommand command )
        {
            CurrentQueryPart = queryPart;
            ExpressionCommand = command;
            factory = query;

            Visit( NormalizedLambda );

            command.BuildQueryPaths( );

            return query;
        }

        protected QueryHandler EvaluateCriteria( PathExpressionFactory query , QueryPart queryPart , ExpressionCommand command )
        {
            CurrentQueryPart = queryPart;
            ExpressionCommand = command;
            factory = query;

            Visit( NormalizedLambda );

            command.BuildQueryPaths( );

            return this;
        }
    }
}
