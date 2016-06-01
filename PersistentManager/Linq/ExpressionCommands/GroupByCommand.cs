using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;
using PersistentManager.Descriptors;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class GroupByCommand : ExpressionCommand
    {
        internal GroupByCommand( CommandContext context , params NotifyCommandListeners[] listerners )
            : base(context, listerners)
        {
            //
        }

        internal override void Execute()
        {
            LambdaExpression expression = QueryHandler.GetLambdaExpression( Context.CurrentCall.Arguments[1] );

            if ( expression.Body is NewExpression )
            {
                NewExpression newExpression = ( NewExpression ) expression.Body;
                ParameterInfo[] parameters = newExpression.Constructor.GetParameters( );
                Expression[] arguments = ((ReadOnlyCollection<Expression>)newExpression.Arguments).ToArray();

                for ( int i = 0 ; i < parameters.Length ; i++ )
                {
                    NameResolver nameResolver = new NameResolver( parameters[i].Name , null , arguments[i] );

                    Context.Factory.GroupBys.Add( nameResolver );
                }
            }
            else if ( expression.Body is MemberExpression )
            {
                NameResolver nameResolver = new NameResolver("Key", null, expression.Body );

                Context.Factory.GroupBys.Add( nameResolver );
            }
            else if ( expression.Body is ParameterExpression )
            {
                ParameterExpression parameter = expression.Body as ParameterExpression;
                List<Expression> members = callerProjections.Select( p => p.Projection ).ToList(); //GetOriginalMemberExpression( parameter ).ToList( );

                PropertyInfo[] properties = parameter.Type.GetProperties( );

                for ( int i = 0 ; i < properties.Length ; i++ )
                {
                    NameResolver nameResolver = new NameResolver( properties[i].Name , null
                        , members[i] );

                    Context.Factory.GroupBys.Add( nameResolver );
                }
            }

            Context.Factory = ExpressionHandler.EvaluateMethodCall( Context.CurrentCall , Context.Factory , QueryPart.GroupBy , this );

            NotifyListeners();
        }

        internal override Expression VisitParameter( ParameterExpression parameterExpression )
        {
            foreach ( var group in Context.Factory.GroupBys )
            {
                MemberExpression member = (MemberExpression) group.Projection;
                Criteria criteria = new Criteria();
                criteria.Name = ExpressionReader.GetMemberName((PropertyInfo)member.Member);
                criteria.QueryPart = QueryPart.GroupBy;
                criteria.DeclaringType = ExpressionReader.GetDeclaringType(member.Member.Name, member.Expression.Type);
                criteria.ReflectedType = ExpressionReader.GetReflectedType(member.Member.Name, member.Expression.Type);

                SetCriteria( group.Projection , criteria );
            }

            return parameterExpression;
        }

        internal override int Priority
        {
            get { return 4; }
        }

        internal override void ValidateExpression()
        {
            throw new NotImplementedException();
        }
    }
}
