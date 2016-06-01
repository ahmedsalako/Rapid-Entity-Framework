using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using PersistentManager.Descriptors;
using PersistentManager.Query;
using PersistentManager;
using PersistentManager.Metadata;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class ExpressionReader : ExpressionVisitor
    {
        internal static bool IsFromParameter( Expression expression , ParameterExpression parameter )
        {
            if ( expression is ParameterExpression )
            {
                ParameterExpression parameterExpression = ( ParameterExpression ) expression;
                if ( parameterExpression.Name == parameter.Name && parameterExpression.Type == parameter.Type )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return IsFromParameter( ( ( MemberExpression ) expression ).Expression , parameter );
        }

        internal static List<Type> GetMemberTypeChain( Expression expression , bool ignoreCompilerGeneratedTypes )
        {
            if ( expression == null ) return null;

            if ( expression.NodeType == ExpressionType.MemberAccess )
            {
                MemberExpression memberExpression = ( MemberExpression ) expression;
                var result = GetMemberTypeChain( memberExpression.Expression , ignoreCompilerGeneratedTypes );

                Type type = memberExpression.Type;

                if ( !( ignoreCompilerGeneratedTypes && IsCompilerGeneratedType( type ) ) )
                    result.Add( type );

                return result;
            }
            else if ( expression.NodeType == ExpressionType.Constant || expression.NodeType == ExpressionType.Parameter )
            {
                var typeList = new List<Type>( );

                Type type = expression.Type;

                if ( !( ignoreCompilerGeneratedTypes && IsCompilerGeneratedType( type ) ) )
                    typeList.Add( type );

                return typeList;
            }
            else
            {
                throw new NotSupportedException( "Expression type not supported: " + expression.GetType( ).FullName );
            }
        }

        internal static Expression GetRootExpression( Expression expression )
        {
            if ( expression == null ) return null;

            {
                if ( expression.NodeType == ExpressionType.MemberAccess )
                {
                    return GetRootExpression( ( ( MemberExpression ) expression ).Expression );
                }
            }

            return expression;
        }

        internal static MemberInfo GetRootMember( MemberExpression memberExpression )
        {
            if ( memberExpression.Expression is ParameterExpression )
            {
                return memberExpression.Member;
            }

            return GetRootMember( memberExpression.Expression as MemberExpression );
        }

        internal static Queue<MemberInfo> GetMemberInfoChain( Queue<MemberInfo> expressions , Expression current )
        {            
            if ( current is ParameterExpression )
            {                
                return new Queue<MemberInfo>( expressions.Reverse() );
            }
            else if ( ( ( MemberExpression ) current ).Expression is ParameterExpression )
            {
                return new Queue<MemberInfo>( expressions.Reverse( ) );
            }

           expressions.Enqueue( ((MemberExpression) current).Member );
           return GetMemberInfoChain( expressions , ( ( MemberExpression ) current ).Expression );
        }

        internal static Expression GetRootExpressionExcludeParameter( MemberExpression memberExpression )
        {
            if ( memberExpression.Expression is ParameterExpression )
            {
                return memberExpression;
            }

            return GetRootExpressionExcludeParameter( memberExpression.Expression as MemberExpression );
        }

        internal static bool RootMemberInfoIsClassOrInterface( MemberExpression memberExpression )
        {
            return ( ( ( PropertyInfo ) GetRootMember( memberExpression ) ).PropertyType.IsClassOrInterface( ) );
        }

        internal static MemberExpression MakeMemberAccess( ParameterExpression parameter , MemberInfo memberInfo )
        {
            return Expression.MakeMemberAccess( parameter , memberInfo );
        }

        internal static MemberExpression SwapParameter( Expression ownerExpression , MemberExpression expression )
        {
            Queue<MemberInfo> expressionChains = GetMemberInfoChain( new Queue<MemberInfo>( ) , expression );

            MemberExpression newExpression = Expression.MakeMemberAccess( ownerExpression , expressionChains.Dequeue( ) );

            foreach ( int index in expressionChains.GetIndices( ) )
            {
                newExpression = (MemberExpression) Expression.MakeMemberAccess( newExpression , expressionChains.Dequeue( ) );
            }

            return newExpression;
        }

        internal static MemberExpression MakeMemberAccess( MemberExpression parameterSource , MemberExpression memberInfoSource )
        {
            return SwapParameter( parameterSource , memberInfoSource );
        }

        internal static Expression StripQuotes( Expression expression )
        {
            while ( expression.NodeType == ExpressionType.Quote )
            {
                expression = ( ( UnaryExpression ) expression ).Operand;
            }
            return expression;
        }

        internal static void MakeMemberAccess( Type parameter , string property )
        {
            var t = Expression.Parameter( parameter , "tt" );
            var prop = Expression.Property( t , property );


            LambdaExpression expression = Expression.Lambda( prop , t );
        }

        internal static IEnumerable<BinaryExpression> GetBinaries( Expression expression )
        {
            if ( expression is LambdaExpression )
            {
                LambdaExpression lambdaExpression = ( LambdaExpression ) expression;

                foreach ( var expr in GetBinaries( lambdaExpression.Body ) )
                    yield return expr;
            }
            else if ( expression is BinaryExpression && !( ( BinaryExpression ) expression ).NodeType.IsCompoundOperator( ) )
            {
                yield return ( BinaryExpression ) expression;
            }
            else if ( expression is BinaryExpression )
            {
                foreach ( var expr in GetBinaries( ( ( BinaryExpression ) expression ).Left ) )
                    yield return expr;

                foreach ( var expr in GetBinaries( ( ( BinaryExpression ) expression ).Right ) )
                    yield return expr;
            }
        }

        internal static IEnumerable<MemberExpression> GetMemberExpressions( Expression expression )
        {
            if ( expression is LambdaExpression )
            {
                LambdaExpression lambdaExpression = ( LambdaExpression ) expression;

                foreach ( var expr in GetMemberExpressions( lambdaExpression.Body ) )
                    yield return expr;
            }
            else if ( expression is BinaryExpression )
            {
                foreach ( var expr in GetMemberExpressions( ( ( BinaryExpression ) expression ).Left ) )
                    yield return expr;

                foreach ( var expr in GetMemberExpressions( ( ( BinaryExpression ) expression ).Right ) )
                    yield return expr;
            }
            else if ( expression is UnaryExpression )
            {
                foreach ( var expr in GetMemberExpressions( ( ( UnaryExpression ) expression ).Operand ) )
                    yield return expr;
            }
            else if ( expression is MemberExpression )
            {
                yield return ( MemberExpression ) expression;
            }
        }

        internal static MemberExpression GetRootExpression( MethodCallExpression MethodCall )
        {
            if ( MethodCall.Arguments[0] is MemberExpression )
                return MethodCall.Arguments[0] as MemberExpression;

            return GetRootExpression( MethodCall.Arguments[0] as MethodCallExpression );
        }

        internal static Type GetReflectedType( string name , Type alternative )
        {
            return alternative;
        }

        internal static Type GetDeclaringType( string name , Type alternative )
        {
            if ( MetaDataManager.IsPersistentable( alternative ) )
            {
                PropertyMetadata propertyMeta = EntityMetadata.GetMappingInfo( alternative )
                                                    .GetPropertyMappingIncludeBase( name );

                if ( propertyMeta.IsNotNull( ) )
                    return alternative;
            }

            return alternative;
        }


        internal static QueryFunction GetFunction( ExpressionType expressionType , object value , Criteria criteria )
        {         
            switch ( expressionType )
            {
                case ExpressionType.Coalesce:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Add:
                    {
                        if ( criteria.IsNotNull( ) )
                        {
                            criteria.ContainedInFunction = true;
                            return new QueryFunction( expressionType.GetCombiningFunction( ) , value , criteria.Hash );
                        }
                        return new QueryFunction( expressionType.GetCombiningFunction() , value , Guid.Empty );
                    }
                default:
                    return null;
            }
        }

        internal static string GetMemberName( PropertyInfo member )
        {
            return member.Name;
        }

        internal static bool EnsureIsEntityMember( Expression expression )
        {
            if ( expression is ConstantExpression ) return false;
            MemberExpression memberExpression = GetImmediateMember( expression );

            if ( memberExpression.IsNotNull( ) )
            {
                if ( memberExpression.Member is PropertyInfo && ( MetaDataManager.IsPersistentable( memberExpression.Expression.Type )
                    || MetaDataManager.HasPersistentProperty( memberExpression.Expression.Type ) ) )
                {
                    return true;
                }
            }

            return false;
        }

        internal static MemberExpression GetImmediateMember( Expression expression )
        {
            if ( expression is MemberExpression )
            {
                return expression as MemberExpression;
            }
            else if ( expression is MethodCallExpression )
            {
                return GetImmediateMember( ( MethodCallExpression ) expression );
            }
            else if ( expression.IsNull( ) )
            {
                return null;
            }

            return ( MemberExpression ) expression;
        }

        internal static MemberExpression GetImmediateMember( MemberExpression expression )
        {
            if ( expression.Expression is MemberExpression && EnsureIsEntityMember( expression ) )
            {
                return expression;
            }
            else if ( expression.Expression is MemberExpression && !EnsureIsEntityMember( expression ) )
            {
                return GetImmediateMember( ( MemberExpression ) expression.Expression );
            }

            return expression;
        }

        internal static MemberExpression GetImmediateMember( MethodCallExpression expression )
        {
            var call = expression;

            while ( call is MethodCallExpression )
            {
                if ( call.Object is MethodCallExpression )
                {
                    call = ( MethodCallExpression ) call.Object;
                }
                else if ( call.Object.IsNull( ) )
                {
                    return ( ( MemberExpression ) StripQuotes( call.Arguments[0] ) );
                }
                else
                {
                    return ( MemberExpression )call.Object;
                }
            }

            return ( MemberExpression ) call.Object;
        }

        internal static QueryPart GetQueryPartByExpressionType( ExpressionType expressionType , QueryPart part )
        {
            switch ( expressionType )
            {
                case ExpressionType.And:
                    return QueryPart.AND;
                case ExpressionType.Or:
                    return QueryPart.OR;
                case ExpressionType.AndAlso:
                    return QueryPart.AND;
                case ExpressionType.OrElse:
                    return QueryPart.OR;
                default:
                    return part;
            }
        }

        internal static Condition GetCondition( Condition condition , object value )
        {
            switch ( condition )
            {
                case Condition.Equals:
                    if ( value.IsNull( ) ) return Condition.IsNull;
                    break;
                case Condition.NotEquals:
                    if ( value.IsNull( ) ) return Condition.IsNotNull;
                    break;
                default:
                    return condition;
            }

            return condition;
        }

        internal static Condition GetCondition( ExpressionType expressionType )
        {
            switch ( expressionType )
            {
                case ExpressionType.Equal:
                    return Condition.Equals;
                case ExpressionType.NotEqual:
                    return Condition.NotEquals;
                case ExpressionType.LessThan:
                    return Condition.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Condition.LessThanEqualsTo;
                case ExpressionType.GreaterThan:
                    return Condition.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Condition.GreaterThanEqualsTo;              
                default:
                    return Condition.Default;
            }
        }

        internal static void CreateSelectParameterForParameterlessCorrelation( QueryPart queryPart , QueryTranslator translator , ProjectionFunction function , PropertyMetadata property )
        {
            if ( property.IsNotNull( ) )
            {
                if ( function != ProjectionFunction.COUNT && queryPart == QueryPart.SELECT )
                {
                    ExpressionCommand command = translator.GetCommand<SelectCommand>( );
                    command = command.IsNull( ) ? translator.GetCommand<AggregateFunctionCommand>( ) : command;

                    if ( ( command.IsNotNull( ) && !command.HasExpression ) )
                    {
                        foreach ( JoinMetadata join in property.JoinDetails )
                        {
                            Criteria criteria = Criteria.CreateCriteria( QueryPart.SELECT , string.Empty );
                            criteria.AddFunction( GetFunctionType( function ) );
                            criteria.DeclaringType = translator.Factory.Main.Type;
                            criteria.ReflectedType = translator.Factory.Main.Type;
                            criteria.Name = join.RelationColumn;
                            translator.Factory.Main.AddCriteria( criteria );
                        }
                    }
                }
            }
        }

        internal static void CreateSelectCorrelation( PathExpressionFactory main , PathExpressionFactory correlation , QueryPart queryPart , Criteria leftJoin , int ordinal )
        {
            if ( queryPart == QueryPart.SELECT )
            {
                Criteria criteria = Criteria.CreateCriteria( QueryPart.SELECT , string.Empty );
                criteria.DeclaringType = leftJoin.IsNotNull() ? leftJoin.DeclaringType : correlation.Main.Type ;
                criteria.ReflectedType = leftJoin.IsNotNull( ) ? leftJoin.ReflectedType : correlation.Main.Type;
                criteria.CorrelatedPath = correlation;
                criteria.Name = leftJoin.IsNotNull() ? leftJoin.Name : string.Empty;
                criteria.Hash = leftJoin.IsNotNull() ? leftJoin.Hash : Guid.NewGuid();
                criteria.Ordinal = ordinal;

                main.Main.AddCriteria( criteria );
                main.AddSelectArgument( criteria );
            }
        }

        internal static Criteria GetCriteria( MemberExpression member , ExpressionType expressionType , QueryPart queryPart )
        {
            if ( ( MetaDataManager.IsPersistentable( member.Expression.Type ) || MetaDataManager.HasPersistentProperty( member.Expression.Type ) ) )
            {
                Criteria CurrentCriteria = new Criteria( );
                CurrentCriteria.Name = GetMemberName( ( PropertyInfo ) member.Member );
                CurrentCriteria.QueryPart = queryPart;
                CurrentCriteria.DeclaringType = GetDeclaringType( member.Member.Name , member.Expression.Type );
                CurrentCriteria.ReflectedType = GetReflectedType( member.Member.Name , member.Expression.Type );
                CurrentCriteria.Condition = GetCondition( expressionType );

                if ( member.Expression is MemberExpression )
                {
                    CurrentCriteria.OwnerPropertyName = ( ( MemberExpression ) member.Expression ).Member.Name;
                }

                return CurrentCriteria;
            }

            return null;
        }

        internal static void JoinCriteria( object value1 , object value2 )
        {
            if ( value1 is Criteria && value2 is Criteria )
            {
                ( ( Criteria ) value1 ).JoinWith = ( ( Criteria ) value2 ).Hash;
                ( ( Criteria ) value1 ).JoinType = JoinType.LeftJoin;

                ( ( Criteria ) value2 ).JoinWith = ( ( Criteria ) value1 ).Hash;
                ( ( Criteria ) value2 ).JoinType = JoinType.RightJoin;
            }
            else if ( value1 is Criteria )
            {
                ( ( Criteria ) value1 ).Value = value2;
                ( ( Criteria ) value1 ).Condition = Condition.Equals;
            }
            else if ( value2 is Criteria )
            {
                ( ( Criteria ) value2 ).Value = value1;
                ( ( Criteria ) value2 ).Condition = Condition.Equals;
            }
            else
            {
                return;
            }
        }

        internal static Queue<string> MakeAnonymousParameters( Type anonymous )
        {
            if ( ( anonymous.IsClass && anonymous != typeof( string ) ) )
            {
                var values = new Queue<string>( );

                foreach ( var arg in anonymous.GetProperties( ) )
                {
                    values.Enqueue( arg.Name );
                }

                return values;
            }

            return null;
        }

        internal static FunctionCall GetFunction( string name )
        {
            switch ( name )
            {
                case "Day":
                    return FunctionCall.Day;
                case "Month":
                    return FunctionCall.Month;
                case "Year":
                    return FunctionCall.Year;
                case "Hour":
                    return FunctionCall.Hour;
                case "Second":
                    return FunctionCall.Second;
                case "ToUpper":
                    return FunctionCall.UpperCase;
                case "ToLower":
                    return FunctionCall.LowerCase;
                case "Trim":
                    return FunctionCall.Trim;
                case "TrimEnd":
                    return FunctionCall.TrimRight;
                case "TrimStart":
                    return FunctionCall.TrimLeft;
                case "Concat":
                    return FunctionCall.Concat;
                case "Average":
                    return FunctionCall.Avg;
                case "Max":
                    return FunctionCall.Max;
                case "Min":
                    return FunctionCall.Min;
                case "Sum":
                    return FunctionCall.Sum;
                case "Count":
                    return FunctionCall.Count;
                case "GetStringLength":
                    return FunctionCall.StringLength;
                default:
                    return FunctionCall.None;
            }
        }

        internal static FunctionCall GetFunctionType(ProjectionFunction function)
        {
            switch (function)
            {
                case ProjectionFunction.SUM :
                    return FunctionCall.Sum;
                case ProjectionFunction.MIN:
                    return FunctionCall.Min;
                case ProjectionFunction.MAX:
                    return FunctionCall.Max;
                case ProjectionFunction.COUNT:
                    return FunctionCall.Count;
                case ProjectionFunction.AVG:
                    return FunctionCall.Avg;
                default:
                    return FunctionCall.None;
            }
        }

        internal static QueryPart GetQueryPart( ExpressionType expressionType )
        {
            switch ( expressionType )
            {
                case ExpressionType.And:
                    return QueryPart.AND;
                case ExpressionType.Or:
                    return QueryPart.OR;
                case ExpressionType.AndAlso:
                    return QueryPart.AND;
                default:
                    return QueryPart.AND;
            }
        }

        internal static ProjectionFunction GetFunctionType( string name )
        {
            switch ( name )
            {
                case "First":
                case "FirstOrDefault":
                    return ProjectionFunction.TOP;
                case "Last":
                case "LastOrDefault":
                    return ProjectionFunction.Last;
                case "Max":
                    return ProjectionFunction.MAX;
                case "Min":
                    return ProjectionFunction.MIN;
                case "Sum":
                    return ProjectionFunction.SUM;
                case "Count":
                    return ProjectionFunction.COUNT;
                case "Average":
                    return ProjectionFunction.AVG;
                case "Reverse":
                    return ProjectionFunction.Reverse;
                default:
                    return ProjectionFunction.NOTSET;
            }
        }

        internal static Condition GetStringManipulationCondition( string name )
        {
            switch ( name )
            {
                case "StartsWith":
                    return Condition.StartsWith;
                case "Contains":
                    return Condition.Contains;
                case "EndsWith":
                    return Condition.EndsWith;
                case "Equals":
                    return Condition.Equals;
                case "CompareTo":
                    return Condition.Equals;
                default:
                    return Condition.Default;
            }
        }
    }
}
