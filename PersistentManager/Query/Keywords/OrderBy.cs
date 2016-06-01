using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;
using System.Data.Common;
using PersistentManager.Query.Sql;
using System.Data;

namespace PersistentManager.Query
{
    public class OrderBy : Keyword
    {
        internal OrderBy( PathExpressionFactory Path , AS As , ORDERBY orderBy )
        {
            Path.ORDERBY = orderBy;
            this.Path = Path;
            this.Identifier = As;            
        }
    }
}
