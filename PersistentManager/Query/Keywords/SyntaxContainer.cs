using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PersistentManager.Descriptors;
using PersistentManager.Query.Sql;
using System.Data.Common;
using System.Data;
using PersistentManager.Util;
using System.Linq;
using PersistentManager.Query.QueryEngine;
using System.Runtime.CompilerServices;
using PersistentManager.Ghosting;
using System.Runtime.Remoting.Messaging;

namespace PersistentManager.Query.Keywords
{
    public class SyntaxContainer : SyntaxExecution , IDisposable
    {
        public SyntaxContainer( )
        {

        }

        internal SyntaxContainer( Type entity , QueryType queryType , string ALIAS )
            : this( )
        {
            QueryContext = new QueryContext( QueryType.Select );
            ReturnType = QueryContext.EntityType = entity;
            QueryContext.MetaStructure = EntityMetadata.GetMappingInfo( entity );
            AddQueryableEntityALIAS( entity.Name , ALIAS );
        }

        private IDictionary<QueryPart , List<Criteria>> parameters = new Dictionary<QueryPart , List<Criteria>>( );

        private IDictionary<string , Criteria> conditionalParameters = new Dictionary<string , Criteria>( );

        private IDictionary<string , SyntaxContainer> subQueries = new Dictionary<string , SyntaxContainer>( );

        private IDictionary<string , SyntaxContainer> fromClauseSubQueries = new Dictionary<string , SyntaxContainer>( );

        private IDictionary<string , string> inheritanceReference = new Dictionary<string , string>( );

        private IDictionary<string , KeyValuePair<string , Type>> joinEmbeddedReference = new Dictionary<string , KeyValuePair<string , Type>>( );

        private IDictionary<string , KeyValuePair<string , string>> joinEmbeddedRelation = new Dictionary<string, KeyValuePair<string , string>>( );

        internal IDictionary<string , Criteria> ConditionalParameters
        {
            get { return conditionalParameters; }
            set { conditionalParameters = value; }
        }

        private IDictionary<string , KeyValuePair<string , Type>> aliases = new Dictionary<string , KeyValuePair<string , Type>>( );

        public IDictionary<string , SyntaxContainer> SubQueries
        {
            get { return subQueries; }
            set { subQueries = value; }
        }

        public IDictionary<string , SyntaxContainer> FromClauseSubQueries
        {
            get { return fromClauseSubQueries; }
            set { fromClauseSubQueries = value; }
        }

        public IDictionary<string , string> InheritanceReference
        {
            get { return inheritanceReference; }
            set { inheritanceReference = value; }
        }

        public IDictionary<string , KeyValuePair<string , Type>> JoinEmbeddedReference
        {
            get { return joinEmbeddedReference; }
            set { joinEmbeddedReference = value; }
        }

        public IDictionary<string , KeyValuePair<string , string>> JoinEmbeddedRelation
        {
            get { return joinEmbeddedRelation; }
            set { joinEmbeddedRelation = value; }
        }

        private string entityALIAS = string.Empty;

        internal string EntityALIAS
        {
            get { return entityALIAS; }
            set { entityALIAS = value; }
        }

        internal IList<string> AllAliases { get; set; }

        internal bool HasProjectionBinding { get; set; }

        internal ProjectionFunction FunctionType  { get; set; }

        internal bool IsDistinct { get; set; }

        internal PathExpressionFactory Factory
        {
            get;
            set;
        }

        private int index = 0;

        internal IDictionary<string , KeyValuePair<string , Type>> ALIASES
        {
            get { return aliases; }
            set { aliases = value; }
        }

        private ORDERBY orderBy;

        internal ORDERBY OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        internal IDictionary<QueryPart , List<Criteria>> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public SyntaxContainer Range( int start , int end )
        {
            QueryContext.StartRange = start;
            QueryContext.EndRange = end;

            return this;
        }

        internal void Add( QueryPart queryPart )
        {
            if( ! parameters.ContainsKey( queryPart ) )
            {
                parameters.Add( queryPart , null );
            }
        }

        internal bool PartContains( QueryPart queryPart , Criteria criteria )
        {
            return PartContains( queryPart , criteria.Name );
        }

        internal bool PartContains( QueryPart queryPart , string name )
        {
            if ( parameters.Count > 0 && parameters.ContainsKey( queryPart ) )
            {
                ICollection<Criteria> criterias = parameters[queryPart];

                return criterias.Count( c => StringUtil.EraseAlias( c.Name ) == name ) > 0;
            }

            return false;
        }

        private void AddDistinct( QueryPart queryPart , params string[] names )
        {
            foreach ( string name in names )
            {
                if ( !PartContains( queryPart , StringUtil.EraseAlias( name ) ) )
                {
                    Add( queryPart , name , string.Empty );
                }
            }
        }

        private void AddDistinct( QueryPart queryPart , Criteria criteria )
        {
            if ( ! PartContains( queryPart , criteria ) )
            {
                Add( queryPart , criteria );
            }
        }

        internal void Add( QueryPart queryPart , Criteria criteria )
        {
            if ( queryPart == QueryPart.WHERE || queryPart == QueryPart.AND || queryPart == QueryPart.OR )
            {
                conditionalParameters.Add( Enum.GetName( typeof( QueryPart ) , queryPart ) + ++index , criteria );
            }

            if ( parameters.ContainsKey( queryPart ) )
            {
                if ( parameters[queryPart].IsNull( ) )
                    parameters[queryPart] = new List<Criteria>( );

                parameters[queryPart].Add( criteria );
            }
            else
            {
                parameters.Add( queryPart , new List<Criteria> { criteria } );
            }
        }

        internal void Add( QueryPart queryPart , FunctionCall function , string name )
        {
            Add( queryPart , Criteria.CreateCriteria( queryPart , name , Condition.Default , null , ProjectionFunction.NOTSET , GetPropertyType( name ) , function ) );
        }

        internal void Add( QueryPart queryPart , string name , Condition condition , object value , ProjectionFunction functionType )
        {
            Add( queryPart , Criteria.CreateCriteria( queryPart , name , condition , value , functionType , GetPropertyType( name ) , FunctionCall.None ) );
        }

        internal void Add( QueryPart queryPart , string name , Condition condition , object value )
        {
            Add( queryPart , Criteria.CreateCriteria( queryPart , name , condition , value , ProjectionFunction.NOTSET , GetPropertyType( name ) , FunctionCall.None ) );
        }

        internal void Add( QueryPart queryPart , string name , object value )
        {
            Add( queryPart , Criteria.CreateCriteria( queryPart , name , Condition.Default , value , ProjectionFunction.NOTSET , GetPropertyType( name ) , FunctionCall.None ) );
        }

        internal void Add( QueryPart queryPart , ProjectionFunction functionType , object value )
        {
            Add( queryPart , Criteria.CreateCriteria( queryPart , value.ToString( ) , Condition.Default , value , functionType , QueryContext.EntityType , FunctionCall.None ) );
        }

        internal void Add( QueryPart queryPart , ProjectionFunction functionType , IList<Criteria> criterias )
        {
            foreach ( Criteria criteria in criterias )
            {
                criteria.QueryPart = queryPart;
                criteria.FuntionType = functionType;
                Add( queryPart , criteria );
            }
        }

        internal Type GetPropertyType( string property )
        {
            try
            {
                string alias = GetMatchingAlias( property );

                if ( alias == property )
                {
                    return QueryContext.EntityType;
                }

                if ( alias == EntityALIAS )
                {
                    PropertyMetadata metadata = QueryContext.MetaStructure.GetPropertyMappingIncludeBase( RemoveMatchingAlias( property ) );
                    return metadata.DeclaringType;
                }

                return ALIASES[alias].Value;
            }
            catch ( Exception x )
            {
                return null;
            }
        }

        internal void Add( QueryPart queryPart , string[] names )
        {
            if ( !names.IsNull( ) )
            {
                foreach ( string name in names )
                {
                    Add( queryPart , name , Condition.Default , name );
                }
            }
            else
            {
                Add( queryPart , null , Condition.Default , null );
            }
        }

        internal void RemovePart( QueryPart queryPart )
        {
            parameters.Remove( queryPart );
        }

        internal void AddQueryableEntityALIAS( string entityName , string alias )
        {
            ALIASES.Add( alias , new KeyValuePair<string , Type>( entityName , QueryContext.EntityType ) );
            EntityALIAS = alias;
        }

        internal string GetMatchingAlias( string name )
        {
            return name.Split( '.' )[0];
        }

        internal string RemoveMatchingAlias( string value )
        {
            foreach ( string alias in ALIASES.Keys.ToArray( ) )
            {
                value = value.Replace( alias + "." , string.Empty );
            }

            return value;
        }

        internal IEnumerable<Criteria> GetQueryByHash( Guid Hash )
        {
            foreach( var parameter in Parameters )
            {
                Criteria criteria = parameter.Value.FirstOrDefault( cr => cr.Hash == Hash ) ;

                if( criteria.IsNotNull() )
                {
                    yield return criteria;
                }
            }
        }

        internal IEnumerable<Criteria> GetQueryByJoin( Guid Hash )
        {
            foreach ( var parameter in Parameters )
            {
                if ( parameter.Value.IsNotNull( ) )
                {
                    Criteria criteria = parameter.Value.FirstOrDefault( cr => cr.JoinWith == Hash );

                    if ( criteria.IsNotNull( ) )
                    {
                        yield return criteria;
                    }
                }
            }
        }

        internal List<Criteria> GetQueryByPart( QueryPart queryPart )
        {
            switch ( queryPart )
            {
                case QueryPart.CONDITIONS:
                    return GetConditions( ).ToList( );
                default:
                    return GetQueryPart( queryPart ).ToList( );
            }
        }

        internal IEnumerable<Criteria> GetConditions( )
        {
            foreach ( var criteria in GetQueryPart( QueryPart.WHERE ) )
                yield return criteria;

            foreach ( var criteria in GetQueryPart( QueryPart.AND ) )
                yield return criteria;

            foreach ( var criteria in GetQueryPart( QueryPart.OR ) )
                yield return criteria;

            foreach (var criteria in GetQueryPart(QueryPart.AND_NOT))
                yield return criteria;
        }

        private IEnumerable<Criteria> GetQueryPart( QueryPart queryPart )
        {
            foreach ( KeyValuePair<QueryPart , List<Criteria>> members in Parameters.Where( p => p.Key == queryPart ) )
            {
                if ( members.Value.IsNotNull( ) )
                {
                    foreach ( Criteria criteria in members.Value )
                    {
                        yield return criteria;
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose( )
        {
            
        }

        #endregion
    }
}
