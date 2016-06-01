using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;

namespace PersistentManager.Query.Processors
{
    internal class RelationshipReferenceProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            List<Criteria> deleted = new List<Criteria>( );
            List<Criteria> inserted = new List<Criteria>( );

            foreach ( Criteria criteria in pathExpression.Criterias.Where( c => c.DeclaringType == pathExpression.Type ) )
            {
                PropertyMetadata property = pathExpression.MetaData.GetPropertyMappingIncludeBase( criteria.Name );

                if ( property.IsNotNull( ) && property.IsRelationshipMapping && !property.IsEntitySplitJoin )
                {
                    inserted.AddRange( EnsureRelationshipReference( pathExpression , property , criteria ) );
                    deleted.Add( criteria );
                }
            }

            foreach ( Criteria current in deleted ) pathExpression.Criterias.Remove( current );
            pathExpression.Criterias.AddRange( inserted );
        }

        private IEnumerable<Criteria> EnsureRelationshipReference( PathExpression path , PropertyMetadata property , Criteria criteria )
        {
            Guid GroupId = Guid.NewGuid( );

            if ( property.IsManyToMany )
            {
                foreach ( PropertyMetadata key in EntityMetadata.GetMappingInfo( property.DeclaringType ).Keys )
                {
                    Criteria newCriteria = criteria.Clone( ) as Criteria;
                    newCriteria.PropertyName = criteria.Name;
                    newCriteria.GroupId = GroupId;
                    newCriteria.Name = key.ClassDefinationName;
                    newCriteria.ApplyAlias( path );

                    yield return newCriteria;
                }
            }
            else if ( property.IsRelationshipMapping )
            {
                foreach ( JoinMetadata join in property.JoinDetails )
                {
                    Criteria newCriteria = criteria.Clone( ) as Criteria;
                    newCriteria.PropertyName = criteria.Name;
                    newCriteria.GroupId = GroupId;
                    newCriteria.ApplyAlias( path );

                    if ( property.IsOneSided )
                    {
                        newCriteria.Name = join.RelationColumn;
                    }
                    else if ( property.IsOneToMany )
                    {
                        newCriteria.Name = join.JoinColumn;
                    }

                    yield return newCriteria;
                }
            }
        }
    }
}
