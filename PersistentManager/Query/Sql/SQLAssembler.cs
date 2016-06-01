using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using PersistentManager.Util;

namespace PersistentManager.Query.Sql
{
    internal class SQLAssembler
    {
        internal StringCollection selectClause = new StringCollection( );
        internal StringCollection fromClause = new StringCollection( );
        internal StringCollection inClause = new StringCollection( );
        internal StringCollection whereClause = new StringCollection( );
        internal StringCollection andClause = new StringCollection( );
        internal StringCollection joinClause = new StringCollection( );
        internal StringCollection orderByClause = new StringCollection( );
        internal StringCollection aggregateOrderBy = new StringCollection( );
        internal StringCollection appenderClause = new StringCollection( );
        internal StringCollection updateClause = new StringCollection( );
        internal StringCollection createClause = new StringCollection( );
        internal StringCollection deleteClause = new StringCollection( );
        internal StringCollection groupByClause = new StringCollection( );
        internal StringCollection relatedBaseJoins = new StringCollection( );
        internal StringCollection selectHeader = new StringCollection();
        internal StringCollection projections = new StringCollection();

        internal string orderDirection = string.Empty;       
        internal bool HasGroupBy = false;

        internal StringCollection GetTokensByPart( QueryPart part )
        {
            switch ( part )
            {
                case QueryPart.SELECT:
                case QueryPart.FUNCTION:
                    return selectClause;
                case QueryPart.Select_Projections:
                    return projections;
                case QueryPart.SELECT_HEADER:
                    return selectHeader;
                case QueryPart.JOINED_INHERITANCE:
                case QueryPart.FROM:
                case QueryPart.JOIN_WITH:
                    return fromClause;
                case QueryPart.JOINED_RELATED_INHERITANCE:
                    return relatedBaseJoins;
                case QueryPart.IN:
                    return inClause;
                case QueryPart.WHERE:
                    return whereClause;
                case QueryPart.OR:
                case QueryPart.AND:
                    return andClause;
                case QueryPart.JOIN:
                    return joinClause;
                case QueryPart.ORDERBY:
                    return orderByClause;
                case QueryPart.Aggregate_OrderBy :
                    return aggregateOrderBy;
                case QueryPart.GroupBy:
                    return groupByClause;
                case QueryPart.Appender:
                    return appenderClause;
                case QueryPart.UPDATE:
                    return updateClause;
                case QueryPart.DELETE:
                    return deleteClause;
                case QueryPart.INSERT:
                    return createClause;
                case QueryPart.ORDERBY_DIRECTION:
                    return new StringCollection( ) { orderDirection };
                default:
                    return new StringCollection( );
            }
        }

        internal void AddFormatted( QueryPart part , bool hasWhereClause , string value )
        {
            switch ( part )
            {
                case QueryPart.SELECT:
                    selectClause.Add( value );
                    break;
                case QueryPart.SELECT_HEADER:
                    selectHeader.Add( value );
                    break;
                case QueryPart.Select_Projections:
                    projections.Add(value);
                    break;
                case QueryPart.FUNCTION:
                    selectClause.Add( value );
                    break;
                case QueryPart.FROM:
                    fromClause.Add( value );
                    break;
                case QueryPart.JOINED_INHERITANCE:
                    fromClause.Add( value );
                    break;
                case QueryPart.JOINED_RELATED_INHERITANCE:
                    relatedBaseJoins.Add( value );
                    break;
                case QueryPart.IN:
                    inClause.Add( value );
                    break;
                case QueryPart.JOIN_WITH:
                    fromClause.Add( value );
                    break;
                case QueryPart.WHERE:
                    whereClause.Add( value );
                    break;
                case QueryPart.OR:
                    andClause.Add( value );
                    break;
                case QueryPart.AND:
                    andClause.Add( value );
                    break;
                case QueryPart.JOIN:
                    joinClause.Add( value );
                    break;
                case QueryPart.DISCRIMINATOR:
                    if ( !hasWhereClause )
                        andClause.Add( value );
                    else
                        whereClause.Add( value );
                    break;
                case QueryPart.ORDERBY:
                    orderByClause.Add( value );
                    break;
                case QueryPart.Aggregate_OrderBy:
                    aggregateOrderBy.Add( value );
                    break;
                case QueryPart.GroupBy:
                    HasGroupBy = true;
                    groupByClause.Add( value );
                    break;
                case QueryPart.Appender:
                    appenderClause.Add( value );
                    break;
                case QueryPart.UPDATE:
                    updateClause.Add( value );
                    break;
                case QueryPart.DELETE:
                    deleteClause.Add( value );
                    break;
                case QueryPart.INSERT:
                    createClause.Add( value );
                    break;
                case QueryPart.ORDERBY_DIRECTION:
                    orderDirection = value;
                    break;
            }
        }

        internal string Assemble( QueryType queryType )
        {
            switch ( queryType )
            {
                case QueryType.Insert:
                    return AssembleCreate( );
                case QueryType.Delete:
                    return AssembleDelete( );
                case QueryType.Update:
                    return AssembleUpdate( );
                default:
                    return AssembleSelect( );
            }
        }

        internal string AssembleSelect( )
        {
            return Dialect.SelectMethod( selectHeader , selectClause ) +
                   Dialect.FromMethod( fromClause ) +
                   Dialect.Parameters( relatedBaseJoins ) +
                   Dialect.Parameters( inClause ) +
                   Dialect.WhereMethod( whereClause ) +
                   Dialect.Parameters( andClause ) +
                   Dialect.GroupByMethod( groupByClause ) +

                   GetOrderBy() +

                   Dialect.Parameters( appenderClause );
        }

        internal string GetOrderBy( )
        {
            return ( aggregateOrderBy.Count > 0 ) ?
                     Dialect.OrderByMethod( aggregateOrderBy , orderDirection ) :
                     Dialect.OrderByMethod( orderByClause , orderDirection );
        }

        internal string AssembleUpdate( )
        {
            return updateClause.ElementsToString( );
        }

        internal string AssembleCreate( )
        {
            return createClause.ElementsToString( );
        }

        internal string AssembleDelete( )
        {
            return deleteClause.ElementsToString( );
        }
    }
}
