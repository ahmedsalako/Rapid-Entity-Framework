using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Processors
{
    internal class CompositeKeyProcessor : PathExpressionProcessor
    {
        internal override void Process( PathExpression pathExpression )
        {
            if ( pathExpression.MetaData.IsNotNull( ) )
            {
                List<Criteria> removables = new List<Criteria>( );
                List<Criteria> addition = new List<Criteria>( );

                foreach ( Criteria criteria in pathExpression.Criterias.Where( c => c.DeclaringType == pathExpression.Type ) )
                {
                    PropertyMetadata property = pathExpression.MetaData.PropertyMapping( criteria.Name );

                    if ( property.IsNull( ) )
                        continue;

                    if ( property.IsPlaceHolding )
                    {
                        addition.AddRange( EnsureCompositeReference( pathExpression , property , criteria ).ToList( ) );
                        removables.Add( criteria );
                    }
                    else if ( criteria.Value.IsNotNull( ) && criteria.Value.GetType( ).IsCompilerGenerated( ) )
                    {
                        criteria.Value = MetaDataManager.GetPropertyValue( property.ClassDefinationName , criteria.Value );
                    }
                    else if ( property.IsCompositional )
                    {
                        if ( criteria.Value.IsNotNull( ) && criteria.Value.GetType( ).IsClassOrInterface( ) )
                        {
                            criteria.Value = MetaDataManager.GetPropertyValue( property.ClassDefinationName , criteria.Value );
                        }
                    }
                }

                foreach ( Criteria criteria in removables ) pathExpression.Criterias.Remove( criteria );

                pathExpression.Criterias.AddRange( addition );
            }            
        }

        private IEnumerable<Criteria> EnsureCompositeReference( PathExpression path , PropertyMetadata property , Criteria criteria )
        {
            Guid compositionGroupId = Guid.NewGuid( );

            foreach ( PropertyMetadata key in EntityMetadata.GetMappingInfo( property.DeclaringType )
                                                                                .Keys.Where( k =>
                                                                                k.CompositeId == property.CompositeId
                                                                            ) )
            {
                Criteria keyCriteria = criteria.Clone( ) as Criteria;
                keyCriteria.PropertyName = property.ClassDefinationName;
                keyCriteria.Name = key.ClassDefinationName;
                keyCriteria.GroupId = compositionGroupId;
                keyCriteria.ApplyAlias( path );

                if ( criteria.QueryPart != QueryPart.SELECT && criteria.Value.IsNotNull( ) )
                {
                    keyCriteria.Value = MetaDataManager.GetPropertyValue( key.ClassDefinationName , keyCriteria.Value );
                }

                yield return keyCriteria;
            }
        }
    }
}
