using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;
using PersistentManager.Ghosting;

namespace PersistentManager.Query.Keywords
{
    internal class ForEach : Keyword
    {
        Criteria Projected { get; set; }

        public ForEach( IEnumerable enumerable )
        {
            BaseScope = This.GetCurrentScopeObject( ).Keyword;
            string parameter = ReadLastParameter( enumerable , BaseScope.Path.ScopeId );
            Projected = BaseScope.AddProjectionExpression( parameter , QueryPart.SELECT );
            Projected .IsProjected = true;
        }

        internal AS As( object alias )
        {
            PropertyMetadata property = EntityMetadata.GetMappingInfo( Projected.DeclaringType )
                                                        .PropertyMapping( Projected.Name );

            Path = new PathExpressionFactory( property.RelationType , this );
            Path.Main.CanonicalAlias = GetIdentifier( alias );
            Path.Keyword.Identifier = new AS( Path );

            if( property.IsManyToMany )
            {
                object joinalias = CallStack.CreateAlias( property.JoinTableType , string.Empty );
                string joinalias1 = CallStack.GetIdentifier( joinalias );

                Path.Keyword.Identifier.Join( property.JoinTableType , joinalias )
                            .On( Path.Main.CanonicalAlias.Dot( property.RightKeys ) )
                            .EqualsTo( joinalias1.Dot( property.JoinColumns ) )
                            .JoinWhere( joinalias1.Dot( property.OwningColumns ) , Projected );

                //Path.Keyword.AddProjectionExpression( Path.Main.CanonicalAlias.Dot( property.RightKeys ) , QueryPart.SELECT );
            }
            else
            {
                foreach( JoinMetadata joinMetadata in property.JoinDetails )
                {
                    Criteria join = Criteria.CreateCriteria( QueryPart.WHERE , joinMetadata.RelationColumn );
                    join.Condition = Condition.Equals;
                    join.QueryPart = QueryPart.WHERE;
                    join.JoinWith = Projected.Hash;
                    join.IsProjected = true;
                    Projected.JoinWith = join.Hash;

                    Path.Main.AddCriteria( join );
                }
            }

            return Path.Keyword.Identifier;
        }
    }
}
