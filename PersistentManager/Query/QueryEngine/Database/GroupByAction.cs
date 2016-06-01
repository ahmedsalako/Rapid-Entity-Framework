using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class GroupByAction : ActionBase , IQueryBuilderStrategy
    {
        internal GroupByAction( ) : base( QueryPart.GroupBy ) { }

        public void Execute( )
        {
            foreach ( Criteria criteria in Criterias )
            {
                Tokens.Add( QueryPart.GroupBy , SetFunctions( criteria , GetAliasedColumn( criteria ) ) );
            }
        }
    }
}
