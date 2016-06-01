using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using PersistentManager.Util;

namespace PersistentManager.Query.Sql
{
    public static class Dialect
    {
        internal const string SELECT = " SELECT ";
        internal const string SELECT_ALL = " * ";
        internal const string WHERE = " WHERE ";
        internal const string FROM = " FROM ";
        internal const string AND = " AND ";
        internal const string AND_NOT = " AND NOT ";
        internal const string OR = " OR ";
        internal const string ORDERBY = " ORDER BY ";
        internal const string COUNT = " COUNT({0}) ";
        internal const string TOP = " TOP ";
        internal const string DISTINCT = " DISTINCT  ";
        internal const string GROUPBY = " GROUP BY ";
        internal const string DELIMITER = ",";

        internal static string PrepareCount( string parameter )
        {
            return string.Format( COUNT , parameter );
        }

        internal static string SelectMethod( StringCollection headers , StringCollection parameters )
        {
            if (parameters.Count > 0)
            {
                return Concat(string.Concat( SELECT , headers.ElementsToString(" ") + " " ) , parameters.ElementsToString( DELIMITER ) );
            }

            return string.Concat( SELECT , headers.ElementsToString(" ") );
        }

        internal static string WhereMethod( StringCollection parameters )
        {
            return Concat( WHERE , parameters.ElementsToString() );
        }

        internal static string GroupByMethod(StringCollection parameters)
        {
            return Concat( GROUPBY , parameters.ElementsToString( DELIMITER ));
        }

        internal static string OrderByMethod( StringCollection parameters , string direction )
        {
            return Concat( ORDERBY , parameters.ElementsToString( DELIMITER ) ) + direction;
        }

        internal static string Parameters( StringCollection parameters )
        {
            return parameters.ElementsToString();
        }

        internal static string FromMethod(StringCollection parameters)
        {
            return Concat( FROM , parameters.ElementsToString() );
        }

        internal static string Concat( string header , params string[] args )
        {
            string toString = ArrayUtil.ToString( args );

            if ( ! toString.IsNullOrEmpty() )
            {
                return string.Concat(header, toString );
            }

            return String.Empty;
        }
    }
}
