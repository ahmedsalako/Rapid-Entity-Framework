using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PersistentManager.Descriptors;
using PersistentManager.Query;
using PersistentManager.Metadata;
using PersistentManager.Mapping;

namespace PersistentManager
{
    internal class CompositeCriteria : Criteria , ICompositeCriterion , ICriteriaCloneable
    {
        public Guid CompositionId { get; set; }
        public IList<Criteria> Criterions { get; set; }
        public PropertyMetadata Metadata { get; set; }
        public CompositeType CompositeType { get; set; }

        public bool IsPlaceHolding { get; set; }        

        internal CompositeCriteria( List<Criteria> criterias , CompositeType compositeType ): this( )
        {
            DeclaringType = criterias.First( ).DeclaringType;
            CompositionId = criterias.First( ).GroupId;
            IsProjected = criterias.First( ).IsProjected;
            this.CompositeType = compositeType;

            PropertyMetadata first = DeclaringType.GetMetataData( ).PropertyMapping
                                                                    (
                                                                        criterias.First( ).PropertyName
                                                                    );

            if ( first.IsNotNull( ) )
            {
                if ( first.IsPlaceHolding )
                {
                    Metadata = DeclaringType.GetMetataData( ).Placeholders.FirstOrDefault( p => p.IsPlaceHolding && p.CompositeId == first.CompositeId );
                    IsPlaceHolding = true;
                }
                else
                {
                    Metadata = first;
                }

                Name = Metadata.ClassDefinationName;
            }

            foreach ( var criteria in criterias )
            {
                Criterions.Add( criteria as Criteria );
            }
        }

        internal CompositeCriteria( )
            : base( )
        {
            Criterions = new List<Criteria>( );
            CompositionId = Guid.Empty;
        }

        internal IEnumerable<object> GetValues( IDataReader dataReader )
        {
            foreach ( string alias in Criterions.Select( c => c.Alias ) )
            {
                yield return dataReader[alias];
            }
        }

        internal override Guid ContainerGuid
        {
            get
            {
                return base.ContainerGuid;
            }
            set
            {
                base.ContainerGuid = value;
                
                foreach ( Criteria criteria in Criterions )
                {
                    criteria.ContainerGuid = value;
                }                
            }
        }

        internal IDictionary<string , object> GetNameValue( IDataReader dataReader )
        {
            IDictionary<string , object> row = new Dictionary<string , object>( );

            foreach ( Criteria criteria in Criterions )
            {
                if ( !criteria.Alias.IsNullOrEmpty( ) && !row.ContainsKey( criteria.Name ) )
                {
                    row.Add( criteria.Name , dataReader[criteria.Alias] );
                }
            }

            return row;
        }

        internal override void ScaleCriteria( PathExpression pathExpression )
        {
            foreach ( Criteria criteria in Criterions )
            {
                criteria.ScaleCriteria( pathExpression );
            }
        }

        internal override Criteria ApplyAlias( PathExpression pathExpression )
        {
            foreach ( Criteria criteria in Criterions ) 
                criteria.ApplyAlias( pathExpression );

            return this;
        }

        internal override Criteria UpdateContainerGuid( Guid oldGuid , Guid newGuid )
        {
            foreach ( Criteria criteria in Criterions )
            {
                criteria.UpdateContainerGuid( oldGuid , newGuid );
            }

            return this;
        }

        public override string ToString( )
        {
            return string.Format( "Composite Criteria : {0} " , Name );
        }

        internal static CompositeCriteria CreateSelectAllCriteria( Type type , CompositeCriteria compositeCriteria , Guid containerGuid )
        {                        
            EntityMetadata entityMetadata = EntityMetadata.GetMappingInfo( type );

            foreach ( PropertyMetadata property in entityMetadata.ColumnInfoBag )
            {
                Criteria criteria = null;

                if ( property.IsRelationshipMapping )
                {
                    if ( property.IsOneSided )
                    {
                        foreach ( JoinMetadata join in property.JoinDetails )
                        {
                            criteria = Criteria.CreateCriteria( QueryPart.SELECT , join.RelationColumn );
                            criteria.PropertyName = criteria.Name;
                            criteria.Name = join.RelationColumn;
                            criteria.Unassigned = true;
                            criteria.Ordinal = compositeCriteria.Ordinal;
                            criteria.ContainerGuid = containerGuid;
                            compositeCriteria.Criterions.Add( criteria );
                        }
                    }
                }
                else if ( property.IsInheritance )
                {
                    foreach (JoinMetadata join in entityMetadata.InheritanceRelation.JoinDetails)
                    {
                        criteria = Criteria.CreateCriteria(QueryPart.SELECT, join.RelationColumn);
                        criteria.PropertyName = criteria.Name;
                        criteria.Name = join.RelationColumn;
                        criteria.Unassigned = true;
                        criteria.Ordinal = compositeCriteria.Ordinal;
                        criteria.ContainerGuid = containerGuid;
                        compositeCriteria.Criterions.Add(criteria);
                    }
                }
                else
                {
                    criteria = Criteria.CreateCriteria( QueryPart.SELECT ,
                                                            property.MappingName ,
                                                            type
                                                       );

                    criteria.Ordinal = compositeCriteria.Ordinal;
                    criteria.ContainerGuid = containerGuid;
                    compositeCriteria.Criterions.Add( criteria );
                }
            }

            return compositeCriteria;
        }

        #region ICloneable Members

        public override Criteria CloneScalable( )
        {
            CompositeCriteria clone = Clone( ) as CompositeCriteria;

            if ( this.CorrelatedPath.IsNotNull( ) )
            {
                this.CorrelatedPath = null;
                this.QueryPart = QueryPart.SELECT; //Hack
            }

            return clone;
        }

        public override object Clone( )
        {
            CompositeCriteria criteria = ( CompositeCriteria ) this.MemberwiseClone( );
            criteria.Functions = new Queue<QueryFunction>( );
            criteria.Criterions = new List<Criteria>();
            MoveAggregates( criteria );

            foreach ( Criteria cloneable in this.Criterions )
            {
                criteria.Criterions.Add( cloneable.Clone( ) as Criteria );
            }

            return criteria;
        }

        #endregion
    }
}
