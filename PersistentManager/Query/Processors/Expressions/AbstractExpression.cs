using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query
{
    internal abstract class AbstractExpression
    {
        protected List<Criteria> scaledUpCriterias = new List<Criteria>( );
        protected List<Criteria> criterias = new List<Criteria>( );
        protected IDictionary<string , PathExpression> references;
        protected IDictionary<Type , PathExpression> embeddeds;
        protected IDictionary<Type , PathExpression> joins;
        internal static PathExpression Empty = null;

        internal Guid UniqueId { get; set; }        

        internal IDictionary<string , PathExpression> References
        {
            get { return references; }
            set { references = value; }
        }

        internal IDictionary<Type , PathExpression> Embeddeds
        {
            get { return embeddeds; }
            set { embeddeds = value; }
        }

        internal IDictionary<Type , PathExpression> Joins
        {
            get { return joins; }
            set { joins = value; }
        }        

        internal List<Criteria> Criterias
        {
            get { return criterias; }
        }

        internal List<Criteria> ScaledUpCriterias
        {
            get { return scaledUpCriterias; }
        }

        internal List<Criteria> AllCriterias
        {
            get { return GetAllCriteriasIncludeScales( ).ToList( ); }
        }

        internal IEnumerable<Criteria> GetAllCriteriasIncludeScales( )
        {
            foreach ( Criteria criteria in Criterias )
                yield return criteria;

            foreach ( Criteria criteria in ScaledUpCriterias )
                yield return criteria;
        }

        internal bool HasReferenceWithCanonicalAlias( string alias )
        {
            foreach ( var value in References )
            {
                if ( value.Value.CanonicalAlias.AreEquals( alias ) )
                {
                    return true;
                }
            }

            return false;
        }

        internal PathExpression FindPath( Guid PathId )
        {
            foreach ( PathExpression reference in References.Values )
            {
                if ( reference.UniqueId == PathId )
                    return reference;

                PathExpression path = reference.FindPath( PathId );

                if ( path.IsNotNull( ) )
                {
                    return path;
                }
            }

            foreach ( PathExpression join in Joins.Values )
            {
                if ( join.UniqueId == PathId )
                    return join;

                PathExpression path = join.FindPath( PathId );

                if ( path.IsNotNull( ) )
                {
                    return path;
                }
            }

            return null;
        }

        internal PathExpression FindPathExpressionByType( Type type )
        {
            foreach ( var value in References )
            {
                if ( value.Value.Type == type )
                {
                    return value.Value;
                }
            }

            foreach ( var value in Joins )
            {
                if ( value.Value.Type == type )
                {
                    return value.Value;
                }
            }

            return null;
        }

        internal PathExpression FindReferenceByCanonicalAlias( string alias )
        {
            foreach ( var value in References )
            {
                if ( value.Value.CanonicalAlias.AreEquals( alias ) )
                {
                    return value.Value;
                }
            }

            return null;
        }

        internal void RemoveFromTree( QueryPart part )
        {
            RemoveIn( part );

            foreach ( PathExpression reference in References.Values )
            {
                reference.RemoveFromTree( part );
            }

            foreach ( PathExpression join in Joins.Values )
            {
                join.RemoveFromTree( part );
            }
        }

        internal void RemovePathExpressionFromTree( Guid hash )
        {
            var references = new List<PathExpression>( References.Values.ToArray( ) );
            foreach ( PathExpression reference in references )
            {
                reference.RemovePathExpressionFromTree( hash );
                References.Remove( reference.Property );
            }

            var joins = new List<PathExpression>( Joins.Values.ToArray( ) );
            foreach ( PathExpression join in joins )
            {
                join.RemovePathExpressionFromTree( hash );
                Joins.Remove( join.Type );
            }
        }

        internal void RemoveFromTree( Guid hash )
        {
            RemoveIn( hash );

            foreach ( PathExpression reference in References.Values )
            {
                reference.RemoveFromTree( hash );
            }

            foreach ( PathExpression join in Joins.Values )
            {
                join.RemoveFromTree( hash );
            }
        }

        internal bool ContainsInTree( Guid hash )
        {
            if ( ContainsIn( hash ) )
            {
                return true;
            }

            foreach ( PathExpression reference in References.Values )
                reference.ContainsInTree( hash );

            foreach ( PathExpression join in Joins.Values )
                join.ContainsInTree( hash );

            return false;
        }

        internal bool ContainsInTree( QueryPart part )
        {
            if ( ContainsIn( part ) )
            {
                return true;
            }

            foreach ( PathExpression reference in References.Values )
                reference.ContainsInTree( part );

            foreach ( PathExpression join in Joins.Values )
                join.ContainsInTree( part );

            return false;
        }

        internal bool ContainsIn( Guid hash )
        {
            return Criterias.Count( c => c.Hash == hash ) > 0;
        }

        internal bool ContainsIn( QueryPart part )
        {
            return Criterias.Count( c => c.QueryPart == part ) > 0;
        }

        internal IEnumerable<Criteria> GetCriterias( QueryPart part )
        {
            return Criterias.Where( c => c.QueryPart == part );
        }

        internal void RemoveIn( QueryPart part )
        {
            List<Criteria> criterias = Criterias.Where( c => c.QueryPart == part ).ToList( );

            foreach ( var criteria in criterias )
            {
                Criterias.Remove( criteria );
            }
        }

        internal void RemoveIn( Guid hash )
        {
            List<Criteria> criterias = Criterias.Where( c => c.Hash == hash ).ToList( );

            foreach ( var criteria in criterias )
            {
                Criterias.Remove( criteria );
            }
        }

        internal void RemoveJoin( Guid RelatedId )
        {
            Criteria criteria = criterias.Where( c => c.JoinWith == RelatedId ).FirstOrDefault( );

            if ( criteria.IsNotNull( ) )
            {
                criterias.Remove( criteria );
            }
        }
    }
}
