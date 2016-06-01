using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal class CriteriaScalarProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            if ( pathExpression.IsEmbedded )
            {
                foreach ( Criteria criteria in pathExpression.Criterias )
                {
                    if ( !pathExpression.Parent.Criterias.Contains( criteria ) )
                    {
                        pathExpression.Parent.Criterias.Add( criteria );
                    }
                }
            }
            else if ( pathExpression.IsBase )
            {
                foreach ( Criteria criteria in pathExpression.Criterias )
                {
                    if ( !pathExpression.Parent.Criterias.Contains( criteria ) )
                    {
                        pathExpression.Parent.Criterias.Add( criteria );
                    }
                }
            }
            else if( !pathExpression.IsMain )
            {
                ScaleUp( pathExpression.Parent , pathExpression );
            }
        }

        internal void ScaleUp( PathExpression Owner , PathExpression Referenced )
        {
            PropertyMetadata reference = EntityMetadata.GetMappingInfo( Owner.Type ).PropertyMapping( Referenced.Property );

            if ( reference.IsNotNull( ) && reference.IsPlaceHolding )
            {
                foreach ( Criteria criteria in Referenced.Criterias )
                {
                    criteria.ContainerGuid = Owner.UniqueId;
                    criteria.PropertyName = criteria.Name;
                    Owner.Criterias.Add( criteria );
                }
            }
            else
            {
                EntityMetadata metadata = EntityMetadata.GetMappingInfo( Referenced.Type );
                List<Criteria> removables = new List<Criteria>( );

                foreach ( Criteria criteria in Referenced.GetScalableCriterias( ) )
                {
                    PropertyMetadata property = metadata.GetPropertyMappingIncludeBase( criteria.Name );
                    Criteria scaled = criteria.CloneScalable( );

                    scaled.ScaleCriteria( Referenced );
                    scaled.ContainerGuid = Owner.UniqueId;
                    scaled.PropertyName = criteria.PropertyName ?? criteria.Name; //Hack

                    scaled.Name = property.IsNotNull( ) &&
                            !property.IsRelationshipMapping ?
                            property.MappingName : criteria.Name;

                    Owner.ScaledUpCriterias.Add( scaled );
                }

                foreach ( Criteria criteria in Referenced.ScaledUpCriterias )
                {
                    Criteria scaled = criteria.CloneScalable( );
                    scaled.ScaleCriteria( Referenced );
                    scaled.ContainerGuid = Owner.UniqueId;
                    scaled.Name = criteria.Name;

                    Owner.ScaledUpCriterias.Add( scaled );
                }
            }
        }
    }
}
