using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Ghosting;
using PersistentManager.Metadata;

namespace PersistentManager.Query.Keywords
{
    public class IEnumerableConstraints<TEntity, T, P> : Constraints<TEntity, T, P> where T : Keyword
    {
        internal IEnumerableConstraints( object property , QueryPart QueryPart , T Current )
            : base(  property , QueryPart , Current )
        {
            this.Current = Current;
            this.QueryPart = QueryPart;
        }

        public T Count(Action<Constraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.Count );
        }

        public T Min(Action<Constraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.Min );
        }

        public T Max(Action<Constraints<TEntity, T, P>> expression)
        {
            return Invoke( expression , FunctionCall.Max );
        }

        internal PathExpressionFactory GetManyToManyQuery( PropertyMetadata property , PathExpression path , FunctionCall call )
        {
            AS keyword = new From( property.RelationType ).As( "a" );

            keyword = keyword.Join( property.JoinTableType , "b" )
                            .On( "b".Dot( property.JoinColumns ) )
                            .EqualsTo( "a".Dot( property.RightKeys ) );

            foreach ( JoinMetadata join in property.JoinDetails )
            {
                Criteria joinCriteria = Criteria.CreateCriteria( QueryPart.WHERE , join.LeftKey , path.Type );
                joinCriteria.Value = "b." + join.OwnerColumn;

                keyword.JoinWhere( "b".Dot( join.OwnerColumn ) , joinCriteria );

                path.AddCriteria( joinCriteria );

                keyword.AddProjectionExpression( QueryPart.SELECT , call , "a".Dot( join.RightKey ) );
            }

            return keyword.Path;
        }

        internal PathExpressionFactory GetOneToMany( PropertyMetadata property , PathExpression path , FunctionCall call )
        {
            AS keyword = new From( property.RelationType ).As( "t01" );

            foreach ( JoinMetadata join in property.JoinDetails )
            {
                Criteria joinCriteria = Criteria.CreateCriteria( QueryPart.WHERE , join.JoinColumn , path.Type );
                //joinCriteria.Value = "t01".Dot( join.JoinColumn );

                path.AddCriteria( joinCriteria );

                keyword.JoinWhere( "t01".Dot( join.RelationColumn ) , joinCriteria );
                keyword.AddProjectionExpression( QueryPart.SELECT , call , "t01".Dot( join.JoinColumn ) );

            }

            return keyword.Path;
        }

        internal T Invoke(Action<Constraints<TEntity, T, P>> expression, FunctionCall call)
        {
            expression.Invoke( this );
            
            PropertyMetadata property = EntityMetadata.GetProperty( CurrentCriteria );
            PathExpression path = Keyword.Path.Main.FindPath( CurrentCriteria.ContainerGuid ) ?? Keyword.Path.Main;

            CurrentCriteria.CorrelatedPath = property.IsOneToMany ? GetOneToMany( property , path , call ) : GetManyToManyQuery( property , path , call );


           
            return Current;
        }
    }
}
