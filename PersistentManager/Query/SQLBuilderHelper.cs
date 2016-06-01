using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Exceptions;
using PersistentManager.Query.Sql;
using PersistentManager.Util;
using System.Data.Common;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query
{
    internal static class SQLBuilderHelper
    {
        internal static string SetQueryField(string value)
        {
            return NamingStrategy.DecorateName(value.ToString());
        }

        internal static DbParameter CreateADOParameter( IDatabaseProvider provider , string name, object value)
        {
            return provider.GetCommandParameter( name , value );
        }

        internal static string CreateNamedParameter(Type entity, QueryType queryType, string fieldName)
        {
            return ProviderResourceAllocator.CreateNamedParameter(entity, queryType, fieldName);
        }

        internal static StringBuilder PrepareConditionalStatement( IDatabaseProvider provider , Type entity, QueryType queryType, ref bool hasWhereClause, 
            string ALIAS, string[] fields, object[] values, List<DbParameter> parameters)
        {
            ALIAS = string.IsNullOrEmpty(ALIAS)? string.Empty : string.Format("{0}.", ALIAS);
            StringBuilder WhereClause = new StringBuilder();
            int count = 0;

            WhereClause.Append((!hasWhereClause) ? string.Empty : Dialect.AND);
            hasWhereClause = true;

            foreach (string value in fields)
            {
                string name = CreateNamedParameter(entity, queryType, value);

                if (parameters.Count(p => p.ParameterName == name) <= 0)
                    parameters.Add(CreateADOParameter( provider , name, values[count]));

                WhereClause.AppendFormat(" {0} {1}{2} = {3}",
                    (count > 0) ? Dialect.AND : string.Empty, ALIAS, 
                    SetQueryField(value), name);

                ++count;
            }

            return WhereClause;
        }

        //This is when we are comparing a field to a field
        //For example (a.PaymentDate = b.PaymentDate)
        internal static string PrepareJoin( string field , object value )
        {
            return string.Format(" {0} = {1}", field  , value );
        }

        internal static string PrepareJoin( string[] fields , object[] values )
        {
            StringBuilder JoinClause = new StringBuilder( );
            int count = 0;

            foreach ( string value in values )
            {
                JoinClause.Append( PrepareJoin( fields[count++] , value ) );
            }

            return JoinClause.ToString( );
        }
    }
}
