using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Collections;
using PersistentManager.Cache;
using PersistentManager.Descriptors;
using PersistentManager.Initializers;
using PersistentManager.Collections;

namespace PersistentManager.Query.Sql
{
    internal abstract class QueryExpression
    {
        internal virtual QueryContext QueryContext { get; set; }

        protected QueryContext Interpret( QueryContext queryContext )
        {
            QueryContext = queryContext;

            QueryContext.DataReader =  PrepareQuery( ).DataReader;

            return QueryContext;
        }

        protected abstract QueryContext PrepareQuery( );
    }
}
