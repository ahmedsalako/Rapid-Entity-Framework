using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    class OrderByAction : ActionBase , IQueryBuilderStrategy
    {
        internal OrderByAction( ) : base( QueryPart.ORDERBY ) { }

        public void Execute( )
        {
            foreach ( Criteria criteria in Criterias ) Tokens.Add( QueryPart.ORDERBY , GetAliasedColumn( criteria ) );            

            Tokens.AddFormatted( QueryPart.ORDERBY_DIRECTION , " {0}" , Enum.GetName( typeof( ORDERBY ) , Syntax.OrderBy ) );
        }
    }
}
