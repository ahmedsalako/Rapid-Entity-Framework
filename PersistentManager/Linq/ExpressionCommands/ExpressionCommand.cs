using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Util;
using System.Linq.Expressions;
using System.Reflection;
using PersistentManager.Descriptors;
using System.Collections.ObjectModel;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal delegate void NotifyCommandListeners( );

    abstract class ExpressionCommand : ExpressionVisitor
    {
        internal List<ExpressionCriteria> criteriaExpressions = new List<ExpressionCriteria>( );
        internal List<NameResolver> callerProjections = new List<NameResolver>();
        internal Queue<QueryFunction> Functions = new Queue<QueryFunction>();        
        protected event NotifyCommandListeners NotificationEvent;        
        internal static ExpressionCommand EmptyCommand = null;        
        private int criteriaOrdinal = 0;

        internal bool HasExpression
        {
            get
            {
                return ( criteriaExpressions.IsNotNull( ) && criteriaExpressions.Count > 0 );
            }
        }

        internal ExpressionCommand( CommandContext context , params NotifyCommandListeners[] listeners )
        {
            this.Context = context;

            if ( context.CurrentCall.IsNotNull( ) )
            {
                callerProjections = GetCallerProjection( Context.CurrentCall.Arguments[0] ).ToList( );
            }

            foreach ( var listerner in listeners )
            {
                NotificationEvent += listerner;
            }
        }

        internal virtual event NotifyCommandListeners Listeners
        {
            add
            {
                if ( !NotificationEvent.IsNull( ) )
                {
                    NotificationEvent += value;
                }
                else
                {
                    NotificationEvent = new NotifyCommandListeners( value );
                }
            }
            remove
            {
                if ( !NotificationEvent.IsNull( ) )
                {
                    NotificationEvent -= value;
                }
            }
        }

        protected void NotifyListeners( )
        {
            if ( NotificationEvent.IsNotNull( ) )
            {
                NotificationEvent.Invoke( );
            }
        }

        internal CommandContext Context { get; set; }
        internal abstract int Priority { get; }
        internal int CriteriaOrdinal
        {
            get
            {
                return criteriaOrdinal++;
            }
        }

        internal abstract void Execute( );
        internal abstract void ValidateExpression( );

        internal void SetCriteria( Expression expression , Criteria criteria )
        {
            if ( !MetaDataManager.IsPersistentable( GetSourceType( expression ) )
                && IsCompilerGeneratedType( GetSourceType( expression ) ) )
            {
                SetCriteria( ( MemberExpression )expression , criteria , true );
            }
            else if ( expression is ParameterExpression )
            {
                SetParameterCriteria( ( ParameterExpression )expression , criteria );
            }
            else if ( expression is MemberExpression )
            {
                SetCriteria( ( MemberExpression )expression , criteria , false );
            }
            else if ( expression is MethodCallExpression )
            {
                criteria.Path = ArrayUtil.ToString( GetPath( ( MethodCallExpression )expression ).ToArray( ) );
                GetPaths( expression , null , criteria , false );
            }
            else
            {
                Context.Factory.Main.AddCriteria( criteria );
            }
        }

        private void SetCriteria( MemberExpression memberExpression , Criteria criteria , bool isCompilerGenerated )
        {
            if ( isCompilerGenerated )
            {
                criteria.Path = StringUtil.RemoveElements( memberExpression.ToString( ) , "." , 2 );
            }
            else
            {
                criteria.Path = StringUtil.RemoveFirstElement( memberExpression.ToString( ) , "." );
            }

            if ( ((PropertyInfo)memberExpression.Member).PropertyType.IsClassOrInterface() )
            {
                GetPaths( memberExpression , null , criteria , isCompilerGenerated );
            }
            else
            {
                GetPaths( memberExpression.Expression , null , criteria , isCompilerGenerated );
            }
        }

        private IEnumerable<string> GetPath( Expression expression )
        {
            if ( expression is MethodCallExpression )
            {
                if ( ( ( MethodCallExpression )expression ).Arguments[0] is MethodCallExpression )
                {
                    foreach ( var path in GetPath( ( ( MethodCallExpression )expression ).Arguments[0] ) )
                    {
                        yield return path;
                    }
                }

                if ( ( ( MethodCallExpression )expression ).Arguments[0] is MemberExpression )
                {
                    MemberExpression memberExpr = ( MemberExpression )( ( MethodCallExpression )expression ).Arguments[0];
                    yield return memberExpr.Member.Name;

                    foreach ( var path in GetPath( memberExpr.Expression ) )
                    {
                        yield return path;
                    }
                }
            }

            if ( expression is MemberExpression )
            {
                yield return ( ( MemberExpression )expression ).Member.Name;

                foreach ( var path in GetPath( ( ( MemberExpression )expression ).Expression ) )
                {
                    yield return path;
                }
            }
        }

        private void SetParameterCriteria( ParameterExpression expression , Criteria criteria )
        {
            PropertyMetadata property = Context.Factory.Main.GetPropertyMetadata( criteria.Name );

            if ( Context.Factory.Main.HasReferenceWithCanonicalAlias( expression.Name ) )
            {
                PathExpression path = Context.Factory.Main.FindReferenceByCanonicalAlias( expression.Name );
                path.AddCriteria( criteria );
            }
            else if ( Context.Factory.Main.FindPathExpressionByType( expression.Type ).IsNotNull( ) )
            {
                PathExpression path = Context.Factory.Main.FindPathExpressionByType( expression.Type );
                path.AddCriteria( criteria );
            }
            else if ( Context.Factory.Main.Type == criteria.DeclaringType )
            {
                Context.Factory.Main.AddCriteria( criteria );                
            }
            else
            {
                Context.Factory.Main.AddJoin( Context.Factory.Create( criteria.OwnerPropertyName , criteria.DeclaringType , criteria ) );
            }
        }

        internal PathExpression CreateQueryPath( Expression expression , PathExpression queryPath )
        {
            if ( expression is MemberExpression )
            {
                MemberInfo member = ( ( MemberExpression )expression ).Member;
                PathExpression path = null;
                Type type = null;

                if ( member is PropertyInfo )
                {
                    PropertyInfo property = ( PropertyInfo )member;

                    if ( property.PropertyType.IsGenericType )
                    {
                        type = property.PropertyType.GetGenericArguments( )[0];
                    }
                    else
                    {

                        type = property.PropertyType;
                    }

                    path = Context.Factory.Create( property.Name , type ); //Names in a declaring type are unique
                }

                if ( queryPath.IsNotNull( ) )
                {
                    path.References.Add( queryPath.Property , queryPath );
                }

                return CreateQueryPath( ( ( MemberExpression )expression ).Expression , path );
            }
            else if ( expression is MethodCallExpression )
            {
                return CreateQueryPath( ( ( MethodCallExpression )expression ).Arguments[0] , null );
            }

            return queryPath;
        }

        private void GetPaths( Expression expression , PathExpression mainPath , Criteria criteria , bool isCompilerGenerated )
        {
            if ( expression is MemberExpression && ( isCompilerGenerated && ( ( ( MemberExpression ) expression ).Expression is ParameterExpression ) ) )
            {
                PropertyInfo property = ( PropertyInfo ) ( ( MemberExpression ) expression ).Member;

                if ( criteria.IsNotNull( ) )
                {
                    if ( Context.Factory.Main.HasReferenceWithCanonicalAlias( property.Name ) )
                    {
                        string alias = property.Name;
                        PathExpression path = Context.Factory.Main.FindReferenceByCanonicalAlias( alias );
                        path.AddCriteria( criteria );
                    }
                    else if ( criteria.DeclaringType == Context.Factory.Main.Type )
                    {
                        Context.Factory.Main.AddCriteria( criteria );
                    }
                    else
                    {
                        Context.Factory.Main.AddToJoin( criteria );
                    }
                }
                else if ( IsCompilerGeneratedType( property.PropertyType ) )
                {
                    if ( Context.Factory.Main.HasReferenceWithCanonicalAlias( property.Name ) )
                    {
                        string alias = property.Name;
                        PathExpression path = Context.Factory.Main.FindReferenceByCanonicalAlias( alias );
                        path.CopyEssentials( mainPath );
                    }
                    else if ( mainPath.Type == Context.Factory.Main.Type )
                    {
                        Context.Factory.Main.CopyEssentials( mainPath );
                    }
                    else
                    {
                        Context.Factory.Main.AddJoin( mainPath );
                    }
                }
                else if ( mainPath.IsNotNull( ) )
                {
                    if ( Context.Factory.Main.HasReferenceWithCanonicalAlias( property.Name ) )
                    {
                        string alias = property.Name;
                        PathExpression path = Context.Factory.Main.FindReferenceByCanonicalAlias( alias );
                        path.AddReferences( mainPath );
                    }
                    else if ( expression.Type == Context.Factory.Main.Type )
                    {
                        Context.Factory.Main.AddReferences( mainPath );
                    }
                    else
                    {
                        Context.Factory.Main.AddJoin( mainPath , expression.Type , ( ( MemberExpression ) expression ).Member.Name );
                    }
                }
            }
            else if ( expression is ParameterExpression )
            {
                if ( Context.Factory.Main.HasReferenceWithCanonicalAlias( ( ( ParameterExpression ) expression ).Name ) )
                {
                    string alias = ( ( ParameterExpression ) expression ).Name;
                    PathExpression path = Context.Factory.Main.FindReferenceByCanonicalAlias( alias );
                    path.CopyEssentials( mainPath );
                }
                else if ( Context.Factory.Main.Type == expression.Type )
                {
                    if ( mainPath.IsNotNull( ) )
                    {
                        Context.Factory.Main.AddReferences( mainPath );
                    }
                    else if ( criteria.IsNotNull( ) )
                    {
                        Context.Factory.Main.AddCriteria( criteria );
                    }
                }
                else
                {
                    Context.Factory.Main.AddJoin( mainPath , expression.Type , ( ( ParameterExpression ) expression ).Name );
                }

                return;
            }
            else if ( expression is MethodCallExpression )
            {
                GetPaths( ( ( MethodCallExpression ) expression ).Arguments[0] , mainPath , criteria , isCompilerGenerated );
            }
            else if ( expression is MemberExpression )
            {
                MemberInfo member = ( ( MemberExpression ) expression ).Member;
                PathExpression path = null;
                Type type = null;

                if ( member is PropertyInfo )
                {
                    PropertyInfo property = ( PropertyInfo ) member;

                    if ( property.PropertyType.IsGenericType )
                    {
                        type = property.PropertyType.GetGenericArguments( )[0];
                    }
                    else
                    {
                        type = property.PropertyType;
                    }

                    path = Context.Factory.Create( property.Name , type ); //Names in a declaring type are unique
                }

                if ( mainPath.IsNotNull( ) )
                {
                    path.References.Add( mainPath.Property , mainPath );
                }

                if ( criteria.IsNotNull( ) && ( type.IsNotNull( ) && criteria.ReflectedType == type ) )
                {
                    path.AddCriteria( criteria );
                }

                GetPaths( ( ( MemberExpression ) expression ).Expression , path , null , isCompilerGenerated );
            }
        }

        private Type GetSourceType( Expression expression )
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.MemberAccess:
                    return GetSourceType(
                        ( expression as MemberExpression ).Expression );
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    return GetSourceType( ( expression as UnaryExpression ).Operand );
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    return expression.Type;
                case ExpressionType.Call:
                    var method = expression as MethodCallExpression;

                    return GetSourceType( method.Arguments[0] );
                default:
                    throw new ArgumentException( );
            }
        }

        internal Criteria LastCriteriaExpression( )
        {
            if ( criteriaExpressions.IsNull( ) || criteriaExpressions.Count <= 0 ) return null;            
            return criteriaExpressions.Last( ).Criteria;
        }

        internal void AddCriteriaExpression( ExpressionCriteria expressionCritera )
        {
            criteriaExpressions.Add( expressionCritera );
        }

        internal void BuildQueryPaths( )
        {
            foreach ( ExpressionCriteria expressionCriteria in criteriaExpressions )
            {
                SetCriteria( expressionCriteria.Expression , expressionCriteria.Criteria );
            }
        }

        internal IEnumerable<NameResolver> GetCallerProjection( Expression CallerExpression )
        {
            if ( CallerExpression is MethodCallExpression )
            {
                MethodCallExpression projection = CallerExpression as MethodCallExpression;

                if ( projection.Arguments.Count > 1 )
                {
                    if ( !( projection.Arguments[1] is ConstantExpression ) )
                    {
                        LambdaExpression expression = QueryHandler.GetLambdaExpression( projection.Arguments[1] );
                        Type type = expression.Parameters[0].Type;

                        if ( expression.Body is NewExpression )
                        {
                            NewExpression newExpression = ( NewExpression )expression.Body;
                            ParameterInfo[] parameters = newExpression.Constructor.GetParameters( );
                            Expression[] arguments = ( ( ReadOnlyCollection<Expression> )newExpression.Arguments ).ToArray( );

                            for ( int i = 0 ; i < parameters.Length ; i++ )
                            {
                                if (arguments[i] is ParameterExpression)
                                {
                                    yield return new NameResolver(parameters[i].Name, ((ParameterExpression)arguments[i]).Name , (ParameterExpression) arguments[i] );
                                }
                                else if (arguments[i] is MethodCallExpression)
                                {
                                    yield return new NameResolver(parameters[i].Name, null , (MethodCallExpression)arguments[i]);
                                }
                                else if ( arguments[i] is BinaryExpression )
                                {
                                    yield return new NameResolver( parameters[i].Name , null , ( BinaryExpression ) arguments[i] );
                                }
                                else
                                {
                                    yield return new NameResolver( parameters[i].Name , ExpressionReader.GetImmediateMember( ( MemberExpression ) arguments[i] ).Member.Name , ( MemberExpression ) arguments[i] );
                                }
                            }
                        }
                        else if ( expression.Body is MemberInitExpression )
                        {
                            MemberInitExpression memberInitExpression = ( MemberInitExpression ) expression.Body;

                            foreach ( MemberBinding binding in memberInitExpression.Bindings )
                            {
                                if ( ((MemberAssignment )binding).Expression is ParameterExpression )
                                {
                                    yield return new NameResolver( binding.Member.Name , null , ( ( MemberAssignment ) binding ).Expression );
                                }
                                else if( ((MemberAssignment )binding).Expression is MethodCallExpression )
                                {
                                    yield return new NameResolver( binding.Member.Name , null , ( ( MemberAssignment ) binding ).Expression );
                                }
                                else if ( ( ( MemberAssignment )binding ).Expression is BinaryExpression )
                                {
                                    yield return new NameResolver( binding.Member.Name , null , ( ( MemberAssignment )binding ).Expression );
                                }
                                else if ( ( ( MemberAssignment ) binding ).Expression is MemberExpression )
                                {
                                    MemberExpression member = ExpressionReader.GetImmediateMember( ( ( MemberAssignment ) binding ).Expression );
                                    yield return new NameResolver( binding.Member.Name , member.Member.Name , member );
                                }
                            }
                        }
                        else if ( !MetaDataManager.IsPersistentable( type ) )
                        {
                            foreach ( var resolved in GetCallerProjection( projection.Arguments[0] ) )
                                yield return resolved;
                        }
                        else if ( expression.Body is MemberExpression )
                        {
                            MemberExpression memberExpression = expression.Body as MemberExpression;
                            yield return new NameResolver( "Key" ,
                                    memberExpression.Member.Name ,
                                    memberExpression );
                        }
                    }
                    else if ( projection.Arguments[0] is MethodCallExpression )
                    {
                        foreach ( var resolved in GetCallerProjection( projection.Arguments[0] ) )
                            yield return resolved;
                    }
                }
                else if ( projection.Arguments[0] is MethodCallExpression )
                {
                    foreach ( var resolved in GetCallerProjection( projection.Arguments[0] ) )
                        yield return resolved;
                }
            }
        }

        internal Expression GetOriginalMemberExpression( MemberExpression memberExpression )
        {
            if ( callerProjections.Count > 0 )
            {
                NameResolver original = callerProjections.Where( n => n.CanonicalName == memberExpression.Member.Name ).FirstOrDefault();

                return original.IsNull() ? memberExpression : original.GetExpression( );
            }

            return memberExpression;
        }
    }
}
