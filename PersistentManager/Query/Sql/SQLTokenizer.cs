using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Data.Common;
using PersistentManager.Util;
using System.Collections.Specialized;

namespace PersistentManager.Query.Sql
{
    internal class SQLTokenizer
    {           
        private List<DbParameter> parameterCollection = new List<DbParameter>();
        private List<string> selectArguments = new List<string>();
        private bool hasWhereClause = false;

        internal string EntityALIAS { get; set; }
        internal bool HasWhereClause { get { return hasWhereClause; } set { hasWhereClause = value; } }

        internal bool IsColumnBasedQuery { get; set; }
        internal Type QueryableEntityType { get; set; }
        internal List<DbParameter> Parameters { get { return parameterCollection; } set { parameterCollection = value; } }
        internal QueryContext QueryContext { get; set; }
        internal bool HasUpdatableStatement { get; set; }
        internal EntityMetadata EntityMetadata { get; set; }
        internal SQLAssembler sqlAssembler = new SQLAssembler();

        internal List<string> SelectArguments
        {
            get { return selectArguments; }
            set { selectArguments = value; }
        }

        internal StringBuilder Tokens
        {
            get { return new StringBuilder( sqlAssembler.Assemble( QueryContext.QueryType ) ); }
        }

        internal void Add( QueryPart part , string alias , string name )
        {
            Add( part , AliasBuilder.Build( alias , NamingStrategy.DecorateName( name ) ) );
        }

        internal void Add( QueryPart part , string[] names )
        {
            foreach ( string name in names )
            {
                Add( part , name );
            }
        }

        internal void Add( QueryPart part , string alias , string[] names )
        {
            foreach ( string name in names )
            {
                Add( part , alias , name );
            }
        }

        internal void Add(QueryPart part, string value)
        {
            sqlAssembler.AddFormatted(part, hasWhereClause, value);
        }

        internal void AddDistinctHeader()
        {
            Add( QueryPart.SELECT_HEADER , Dialect.DISTINCT );
        }

        internal void ClearTokens()
        {
            sqlAssembler = new SQLAssembler();
        }

        internal void ClearTokens( QueryPart part )
        {
            sqlAssembler.GetTokensByPart( part ).Clear( );
        }

        internal void AddFormatted(QueryPart part,  string value, params object[] values)
        {            
            sqlAssembler.AddFormatted( part , hasWhereClause , string.Format( value , values ) );       
        }

        internal string Format( string value , params object[] values )
        {
            return string.Format( value , values ) ;
        }

        internal bool HasOrderBy
        {
            get { return sqlAssembler.orderByClause.Count > 0; }
        }

        internal void CopyFrom( QueryPart queryFrom , QueryPart queryTo , bool addAs )
        {
            StringCollection rightTokens = sqlAssembler.GetTokensByPart( queryFrom );
            StringCollection leftTokens = sqlAssembler.GetTokensByPart( queryTo );

            foreach ( string copy in rightTokens )
            {
                string text = addAs ? copy.AddAs( ) : copy;

                if ( !leftTokens.ElementsContains( text ) )
                {
                    leftTokens.Add( text );
                }
            }
        }

        internal bool ElementsContainedIn( QueryPart From , QueryPart To )
        {
            StringCollection rightTokens = sqlAssembler.GetTokensByPart( From );
            StringCollection leftTokens = sqlAssembler.GetTokensByPart( To );

            foreach ( string text in rightTokens )
            {
                if ( !leftTokens.ElementsContains( text ) )
                {
                    return false;
                }
            }

            return rightTokens.Count > 0 ? true : false ;
        }

        internal bool QueryPartContains( QueryPart part , string column )
        {
            foreach ( string select in sqlAssembler.GetTokensByPart( part ) )
            {
                if ( select.GetAliased( ).SpecializedContains( column.GetAliased() ) )
                {
                    return true;
                }
            }

            return false;
        }

        internal void Add( PropertyMetadata[] properties , string ALIAS , QueryPart part )
        {
            foreach( int index in properties.GetIndices() )
            {
                PropertyMetadata property = properties[index];
                string Column = AliasBuilder.Build( NamingStrategy.DecorateName( ALIAS ) , NamingStrategy.DecorateName( property.MappingName ) );
                Column = part == QueryPart.SELECT ? Column.AddAs( ) : Column;

                if ( !QueryPartContains( part , Column ) )
                {                    
                    Add( part , Column );
                }
            }
        }

        internal StringCollection GetSelectArguments( string newAlias )
        {
            StringCollection newSelect = new StringCollection( );
            foreach ( string select in sqlAssembler.selectClause )
            {
                newSelect.Add( AliasBuilder.Build( newAlias , EraseAlias( select ) ) );
            }

            return newSelect;
        }

        internal StringCollection GetOrderBys( string newAlias )
        {
            StringCollection newOrderBy = new StringCollection( );
            foreach ( string order in GetOrderBys( ) )
            {
                newOrderBy.Add( AliasBuilder.Build( newAlias , EraseAlias( order ) ) );
            }

            return newOrderBy;
        }

        internal IEnumerable<string> GetOrderBys( )
        {
            foreach ( string column in sqlAssembler.orderByClause )
                yield return column;
        }

        internal string OrderByDirection( )
        {
            return sqlAssembler.orderDirection;
        }

        internal void SetOrderByDirection( string direction )
        {
            sqlAssembler.orderDirection = direction;
        }

        internal bool HasGroupBy
        {
            get { return sqlAssembler.HasGroupBy; }
        }

        internal bool ContainsInValue( List<Criteria> criterias , PropertyMetadata column )
        {
            return criterias.Count(c => c.Name == column.ClassDefinationName || c.Name == column.MappingName) > 0;            
        }

        internal string EraseAlias( string value )
        {
            return StringUtil.EraseAlias( value );
        }

        internal string CreateProviderParameter( IDatabaseProvider provider , string name , object value )
        {
            if ( Parameters.Count( p => p.ParameterName == name ) <= 0 )
            {
                name = SQLBuilderHelper.CreateNamedParameter( QueryContext.EntityType , QueryContext.QueryType , name );
                Parameters.Add( SQLBuilderHelper.CreateADOParameter( provider , name , value ) );
            }

            return name;
        }

        internal bool IsNotInSelectArgument( PropertyMetadata column )
        {
            return IsNotInSelectArgument( column.MappingName );
        }

        internal bool IsNotInSelectArgument( string name )
        {
            foreach ( string argument in selectArguments )
            {
                if ( argument == name )
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return sqlAssembler.AssembleSelect();
        }
    }
}
