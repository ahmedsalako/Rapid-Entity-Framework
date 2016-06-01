using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Collections;
using PersistentManager;
using PersistentManager.Metadata;
using PersistentManager.Mapping;
using PersistentManager.Query.Processors;

namespace PersistentManager.Query
{
    internal class PathExpression : AbstractExpression
    {
        internal PathExpression( string property , Type type )
        {
            this.Type = type;
            this.Property = property;
            this.UniqueId = Guid.NewGuid( );
            this.joins = new Dictionary<Type , PathExpression>( );
            this.references = new Dictionary<string , PathExpression>( );
            this.embeddeds = new Dictionary<Type , PathExpression>( );
        }

        internal PathExpression( string property , Type type , Criteria criteria )
            : this( property , type )
        {
            AddCriteria( criteria );
        }

        internal PathExpression( string property , Type type , PathExpression path )
            : this( property , type )
        {
            AddReferences( path );
        }

        internal PathExpression( bool isMain , Type type )
            : this( type.Name , type )
        {
            this.IsMain = isMain;
        }

        internal PathExpression( bool isMain , Type type , bool isBase )
            : this( type.Name , type )
        {
            this.IsMain = isMain;
            this.IsBase = isBase;
        }

        internal ProjectionFunction FunctionType { get; set; }
        internal Type Type { get; set; }
        internal bool IsMain { get; set; }             

        internal string ALIAS { get; set; }

        internal bool IsEmbedded { get; private set; }

        internal string CanonicalAlias { get; set; }

        internal string OuterALIAS { get; set; }

        internal PathExpression Base { get; set; }

        internal PathExpression Parent { get; set; }        

        internal bool HasBase { get { return null != Base; } }

        internal bool IsBase { get; set; }        

        internal string Property { get; set; }

        public EntityMetadata MetaData
        {
            get { return EntityMetadata.GetMappingInfo( Type ); }
        }

        public void AddCriteria( Criteria criteria )
        {
            PropertyMetadata property = GetPropertyMetadata( criteria.Name );

            if ( criteria.IsSelectCriteria( ) )
            {
                if ( property.IsNotNull( ) && property.IsOneSided && property.DeclaringType == Type )
                {
                    PathExpression reference = new PathExpression( property.ClassDefinationName , property.PropertyType );
                    reference.CreateCompositeSelectAll( property.ClassDefinationName , Type , CompositeType.OneSideRelationship , criteria.Ordinal );
                    AddReferences( reference );
                    return;
                }
                else if ( criteria is CompositeCriteria && criteria.QueryPart == QueryPart.SELECT && ( criteria as CompositeCriteria ).IsEmptyCriteria( ) )
                {
                    CreateCompositeSelectAll( criteria.DeclaringType , criteria as CompositeCriteria , UniqueId );
                    return;
                }
            }

            if ( property.IsNull( ) || property.DeclaringType == Type )
            {
                if ( property.IsNotNull( ) && !property.IsRelationshipMapping )
                {
                    criteria.Name = property.MappingName;
                }
                
                AddPathCriteria( criteria );
                return;
            }

            if ( MetaData.HasBaseEntity )
            {
                if ( null == Base )
                {
                    Base = CreateBaseJoin( MetaData.BaseEntity );                    
                }

                Base.AddCriteria( criteria );
            }
            
            if ( MetaData.HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity embedded in MetaData.EmbeddedTypes )
                {
                    if ( property.DeclaringType == embedded.Type )
                    {
                        if ( Embeddeds.ContainsKey( embedded.Type ) )
                        {
                            Embeddeds[embedded.Type].AddCriteria( criteria );
                        }
                        else
                        {
                            Embeddeds.Add( embedded.Type , CreateEmbeddedJoin( embedded.Type ) );
                            Embeddeds[embedded.Type].AddCriteria( criteria );                          
                        }
                    }
                }
            }
        }

        private void AddPathCriteria( Criteria criteria )
        {
            if ( criteria.IsNotNull( ) )
            {
                criteria.ContainerGuid = UniqueId;
                criterias.Add( criteria );
            }
        }

        private void AddPathCriteria( List<Criteria> criterias )
        {
            foreach ( Criteria criteria in criterias )
                AddPathCriteria( criteria );
        }

        private CompositeCriteria CreateEmbeddedSelectAll( EmbeddedEntity embeddedEntity , CompositeCriteria composite )
        {
            PathExpression embeddedPath = PathExpression.Empty;

            if ( Embeddeds.ContainsKey( embeddedEntity.Type ) )
            {
                embeddedPath = Embeddeds[embeddedEntity.Type];
            }
            else
            {
                embeddedPath = CreateEmbeddedJoin( embeddedEntity.Type );
                Embeddeds.Add( embeddedEntity.Type , embeddedPath );
            }

            foreach( PropertyMetadata property in EntityMetadata.GetMappingInfo( embeddedEntity.Type ) )
            {
                if ( property.MappingName != embeddedEntity.JoinColumn ) //Embedded is Left Outer Join, no point adding the relation column in Select All
                {
                    Criteria criteria = Criteria.CreateCriteria( QueryPart.SELECT ,
                                                            property.MappingName ,
                                                            embeddedEntity.Type
                                                       );

                    criteria.Ordinal = composite.Ordinal;
                    criteria.ContainerGuid = embeddedPath.UniqueId;
                    composite.Criterions.Add( criteria );
                }
            }            

            embeddedPath.Criterias.Add( composite ); //Need to think of another way. Because update to ContainerGuid for Composites updates child criteria 
            //which is not healthy in an embedded and inheritance situation

            return composite;
        }

        internal void CreateCompositeSelectAll( string property , Type declaringType , CompositeType compositeType , int ordinal )
        {
            CompositeCriteria composite = CompositeCriteria.CreateCompositeCriteria
            (
                    QueryPart.SELECT ,
                    property ,
                    declaringType ,
                    compositeType ,
                    ordinal
            );

            composite.ContainerGuid = UniqueId;

            CreateCompositeSelectAll( declaringType , composite , UniqueId );
        }

        internal void CreateCompositeSelectAll( Type declaringType , CompositeCriteria composite , Guid containerGuid )
        {
            if ( MetaData.HasBaseEntity )
            {
                if ( null == Base ) Base = CreateBaseJoin( MetaData.BaseEntity );
                Base.CreateCompositeSelectAll( MetaData.BaseEntity , composite , Base.UniqueId  );
            }

            if ( MetaData.HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity embeddedEntity in MetaData.EmbeddedTypes )
                {
                    composite = CreateEmbeddedSelectAll( embeddedEntity , composite );
                }
            }

            Criterias.Add( CompositeCriteria.CreateSelectAllCriteria( Type , composite , UniqueId ) );
        }      

        internal void AddCriterias( ICollection<Criteria> criterias )
        {
            foreach ( var criteria in criterias )
            {
                AddPathCriteria ( criteria );
            }
        }

        internal void AddToJoin( Criteria criteria )
        {
            AddJoin( new PathExpression( criteria.OwnerPropertyName , criteria.ReflectedType , criteria ) );
        }

        internal void AddJoin( PathExpression path , Type type , string identifier )
        {
            if ( Joins.ContainsKey( type ) )
            {
                Joins[type].AddReferences( path );
            }
            else
            {
                AddJoin( new PathExpression( identifier , type , path ) );
            }
        }

        internal void AddJoin( PathExpression path )
        {
            if ( Joins.ContainsKey( path.Type ) )
            {
                Joins[path.Type].CopyEssentials( path );
            }
            else
            {
                path.Parent = this;
                joins.Add( path.Type , path );
            }
        }

        internal IEnumerable<Criteria> GetScalableCriterias( )
        {
            if ( IsBase )
            {
                foreach ( Criteria criteria in Criterias )
                    yield return criteria;
            }
            else
            {
                foreach ( var criteria in Criterias )
                {
                    if ( criteria.JoinType != JoinType.NOTSET && criteria.JoinType != JoinType.LeftJoin )
                        yield return criteria;

                    else if ( criteria.QueryPart == QueryPart.GroupBy )
                        yield return criteria;

                    else if ( criteria.QueryPart == QueryPart.ORDERBY )
                        yield return criteria;

                    else if ( criteria.QueryPart == QueryPart.SELECT )
                        yield return criteria;
                    else if ( criteria.CorrelatedPath.IsNotNull( ) )
                        yield return criteria;
                }
            }
        }

        internal PathExpression AddReferences( string property )
        {
            PropertyMetadata metadata = GetPropertyMetadata( property );
            return AddReferences( new PathExpression( property , metadata.PropertyType ) );
        }

        internal PathExpression AddReferences( PathExpression path )
        {
            if ( References.ContainsKey( path.Property ) )
            {
                References[path.Property].CopyEssentials( path );
                return References[path.Property];
            }
            else
            {
                string property = path.Property;
                path = CreateReferenceJoin( path );
                path.Parent = path.Parent ?? this;
                References.Add( property , path );                
            }

            return path;
        }

        internal PathExpression CreateEmbeddedJoin( Type type )
        {
            PathExpression embedded = new PathExpression( false , type );
            embedded.Parent = this;
            embedded.IsEmbedded = true;

            return embedded;
        }

        internal PathExpression CreateBaseJoin( Type type )
        {
            if ( Base.IsNotNull( ) && Base.Type != type )
            {
                Base.CreateBaseJoin( type );
            }
            else
            {
                Base = new PathExpression( false , type , true );
                Base.Parent = this;
            }

            PropertyMetadata property = MetaData.InheritanceRelation;

            foreach ( JoinMetadata join in property.JoinDetails )
            {
                Criteria criteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                         join.RelationColumn ,
                                         Condition.Equals , null ,
                                         ProjectionFunction.NOTSET ,
                                         Type
                                        );

                criteria.Unassigned = true;

                Criteria joinCriteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                         join.JoinColumn ,
                                         Condition.Equals , null ,
                                         ProjectionFunction.NOTSET ,
                                         Base.Type
                                        );

                joinCriteria.Unassigned = true;

                criteria.JoinCriteria( joinCriteria );

                AddPathCriteria( criteria );
                Base.AddPathCriteria( joinCriteria );
            }

            return Base;
        }

        internal PathExpression CreateReferenceJoin( PathExpression pathExpression )
        {
            PropertyMetadata property = GetPropertyMetadata( pathExpression.Property );

            if ( Type != property.DeclaringType )
            {
                return CreateBaseJoin( property.DeclaringType )
                            .CreateReferenceJoin( pathExpression );
            }

            if ( property.IsOneSided )
            {                
                return CreateOneSidedReference( property , pathExpression );
            }

            return CreateManySideReference( property , pathExpression );
        }

        internal PathExpression CreateOneSidedReference( PropertyMetadata property , PathExpression pathExpression )
        {           
            if ( property.IsOneSided && property.IsEntitySplitJoin )
            {
                //this.AddPathCriteria( pathExpression.Criterias );
                return pathExpression;
            }
            else if ( property.IsOneSided )
            {
                foreach ( JoinMetadata join in property.JoinDetails )
                {
                    Criteria criteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                             property.IsImported ? join.JoinColumn : join.RelationColumn ,
                                             Condition.Equals , null ,
                                             ProjectionFunction.NOTSET ,
                                             Type
                                            );

                    criteria.Unassigned = true;

                    Criteria joinCriteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                             property.IsImported ? join.RelationColumn : join.JoinColumn ,
                                             Condition.Equals , null ,
                                             ProjectionFunction.NOTSET ,
                                             pathExpression.Type
                                            );

                    joinCriteria.Unassigned = true;

                    criteria.JoinCriteria( joinCriteria );

                    AddPathCriteria( criteria );
                    pathExpression.AddPathCriteria( joinCriteria );
                }
            }

            return pathExpression;
        }

        internal PathExpression CreateManySideReference( PropertyMetadata property , PathExpression pathExpression )
        {
            if ( property.IsOneToMany )
            {
                foreach ( JoinMetadata join in property.JoinDetails )
                {
                    Criteria criteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                             join.JoinColumn ,
                                             Condition.Equals , null ,
                                             ProjectionFunction.NOTSET ,
                                             Type
                                            );

                    criteria.Unassigned = true;

                    Criteria joinCriteria = Criteria.CreateCriteria( QueryPart.WHERE ,
                                             join.RelationColumn ,
                                             Condition.Equals , null ,
                                             ProjectionFunction.NOTSET ,
                                             pathExpression.Type
                                            );

                    joinCriteria.Unassigned = true;

                    criteria.JoinCriteria( joinCriteria );

                    AddPathCriteria( criteria );
                    pathExpression.AddPathCriteria( joinCriteria );
                }
            }
            else if ( property.IsManyToMany )
            {
                PathExpression joinTable = new PathExpression( false , property.JoinTableType );
                pathExpression.AddJoin( joinTable );

                foreach ( JoinMetadata join in property.JoinDetails )
                {
                    Criteria ownerColumn = Criteria.CreateCriteria( QueryPart.WHERE , join.OwnerColumn );
                    ownerColumn.Unassigned = true;

                    Criteria joinColumn = Criteria.CreateCriteria( QueryPart.WHERE , join.JoinColumn );
                    joinColumn.Unassigned = true;

                    joinTable.AddCriteria( ownerColumn );
                    joinTable.AddCriteria( joinColumn );

                    #region Right Key Join

                    Criteria rightKey = Criteria.CreateCriteria( QueryPart.WHERE , join.RightKey );
                    rightKey.DeclaringType = property.JoinTableType;
                    rightKey.Condition = Condition.Equals;
                    rightKey.Unassigned = true;

                    rightKey.JoinCriteria( joinColumn );
                    pathExpression.AddPathCriteria( rightKey );

                    #endregion

                    #region Left Key Join
                    Criteria leftKey = new Criteria( );
                    leftKey.Name = join.LeftKey;
                    leftKey.DeclaringType = Type;
                    leftKey.Condition = Condition.Equals;
                    leftKey.Unassigned = true;
                    leftKey.QueryPart = QueryPart.WHERE;  
                  
                    leftKey.JoinCriteria( ownerColumn );

                    AddPathCriteria( leftKey );
                    #endregion
                }
            }

            return pathExpression;
        }

        internal void CopyEssentials( PathExpression path )
        {
            if ( this.Base.IsNotNull( ) && path.HasBase )
            {
                this.Base.CopyEssentials( path.Base );
            }
            else if ( path.HasBase )
            {
                this.Base = path.Base;
            }

            foreach ( Criteria criteria in path.Criterias )
            {
                AddPathCriteria( criteria.UpdateContainerGuid( path.UniqueId , UniqueId ) );
            }

            foreach ( var reference in path.References )
            {
                AddReferences( reference.Value );
            }

            foreach ( var reference in path.Joins )
            {
                AddJoin( reference.Value );
            }

            foreach ( var embedded in path.Embeddeds )
            {
                if ( Embeddeds.ContainsKey( embedded.Key ) )
                {
                    Embeddeds[embedded.Key].CopyEssentials( embedded.Value );
                }
                else
                {
                    embedded.Value.Parent = this;
                    Embeddeds.Add( embedded.Key , embedded.Value );
                }
            }
        }

        internal bool HasGroupingCriterias( )
        {
            return Criterias.Count( c => c.IsGroupingCriteria( ) ) > 0;
        }

        internal bool HasGroupingScaledCriterias( )
        {
            return ScaledUpCriterias.Count( c => c.IsGroupingCriteria( ) ) > 0;
        }

        internal bool HasSelectProjections( )
        {
            return ( HasCriteriaSelectProjections( ) || HasScalableSelectProjections( ) );
        }

        internal bool HasCriteriaSelectProjections( )
        {
            return Criterias.Exists( c => c.IsSelectProjection( ) );
        }

        internal bool HasScalableSelectProjections()
        {
            return ScaledUpCriterias.Exists( c => c.IsSelectProjection( ) );
        }

        internal PropertyMetadata GetPropertyMetadata( string property )
        {
           return MetaData.GetPropertyMappingIncludeBase( property );
        }

        internal IList<string> IndexAll( AliasBuilder aliasBuilder )
        {
            SequentialProcessor processor = new SequentialProcessor( aliasBuilder );
            processor.Process( this );

            return processor.AllAliases;
        }

        public override string ToString( )
        {
            if ( Type.IsNotNull( ) )
            {
                return Type.FullName;
            }

            return base.ToString( );
        }         
    }
}