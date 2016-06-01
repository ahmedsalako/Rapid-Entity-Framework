using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using System.Data.Common;

namespace PersistentManager.Query.Keywords
{
    public class DML
    {
        internal SyntaxContainer query;

        internal DML(SyntaxContainer query)
        {
            this.query = query;
        }

        internal DML(SyntaxContainer query, QueryType queryType)
        {
            this.query = query;
            query.QueryContext.QueryType = queryType;
        }

        public SyntaxContainer GetSyntax()
        {
            query.Dispose( );
            return query;
        }
    }
}
