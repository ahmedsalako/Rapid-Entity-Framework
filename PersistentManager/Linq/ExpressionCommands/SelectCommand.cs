using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;
using System.Linq.Expressions;
using PersistentManager.Descriptors;
using System.Collections.ObjectModel;
using PersistentManager.Query.Projections;
using PersistentManager.Metadata;
using System.Reflection;

namespace PersistentManager.Linq.ExpressionCommands
{
    internal class SelectCommand : ExpressionCommand
    {
        internal SelectCommand( CommandContext context , params NotifyCommandListeners[] listerners )
            : base( context , listerners )
        {

        }

        internal override void Execute( )
        {
            LambdaExpression selectArgument = ExpressionReader.StripQuotes
                                              ( 
                                                Context.CurrentCall.IsNotNull()? Context.CurrentCall.Arguments[1] 
                                                    : Context.Expression
                                              ) as LambdaExpression;

            if ( Context.CurrentCall.IsNotNull( ) )
            {
                Context.Factory = ExpressionHandler.EvaluateMethodCall( Context.CurrentCall , Context.Factory , QueryPart.SELECT , this );
            }
            else
            {
                Context.Factory = ExpressionHandler.EvaluateExpression( Context.Expression , Context.Factory , QueryPart.SELECT , this );
            }

            NotifyListeners( );
        }

        internal override void ValidateExpression( )
        {
            throw new NotImplementedException( );
        }

        internal override Expression VisitParameter( ParameterExpression parameterExpression )
        {
            if ( parameterExpression.Type.IsGroupingType( ) )
            {
                foreach ( NameResolver resolver in Context.Factory.GroupBys )
                {
                    if ( resolver.Criteria.IsNotNull( ) )
                    {
                        Criteria criteria = ( Criteria ) resolver.Criteria.Clone( );
                        criteria.QueryPart = QueryPart.SELECT;
                        this.AddCriteriaExpression( new ExpressionCriteria( resolver.Projection , criteria ) );
                        Context.Factory.AddSelectArgument( criteria );
                    }
                }
            }
            else if ( MetaDataManager.IsPersistentable( parameterExpression.Type ) )
            {
                CompositeCriteria compositeCriteria = Criteria.CreateCompositeCriteria( QueryPart.SELECT , "" , parameterExpression.Type , CompositeType.EntityClass , CriteriaOrdinal );

                AddCriteriaExpression( new ExpressionCriteria( parameterExpression , compositeCriteria ) );
            }

            return parameterExpression;
        }

        protected override Expression VisitMemberAccess( MemberExpression memberExpression )
        {
            return memberExpression;
        }

        internal static ProjectionBinder VisitProjectionBinder( PathExpressionFactory Path , ReadOnlyCollection<MemberBinding> memberBindigs )
        {
            SelectCommand command = new SelectCommand( new CommandContext { Factory = Path } );

            command.VisitBindingList( memberBindigs );

            return command.Context.Factory.ProjectionBinder;
        }

        internal IEnumerable<MemberBinding> MakeProjection( IEnumerable<MemberBinding> bindings , bool evaluateRightSide )
        {
            ProjectionBinder binder = new ProjectionBinder( );
            foreach ( MemberBinding binding in bindings )
            {
                object value = Null.NOTHING;
                MemberExpression right = ExpressionReader.GetImmediateMember( ( ( MemberAssignment ) binding ).Expression );

                // Both Linq and RQL makes use of this call . 
                // RQL Requires evalutation of the right side in order to get the called property from the call stack
                if ( evaluateRightSide )
                {
                    value = ExpressionReader.GetMethodAssignment( ( ( MemberAssignment ) binding ).Expression );
                }

                binder.AddProperty( binding.Member.Name , ((MemberAssignment) binding).Expression.Type , right.IsNotNull( ) ? right.Member.Name : string.Empty , value );
            }

            Context.Factory.ProjectionBinder = binder;

            return bindings;
        }

        protected override IEnumerable<MemberBinding> VisitBindingList( ReadOnlyCollection<MemberBinding> memberBindings )
        {
            return MakeProjection( base.VisitBindingList( memberBindings ) , true );
        }

        internal override int Priority
        {
            get { return 6; }
        }
    }
}