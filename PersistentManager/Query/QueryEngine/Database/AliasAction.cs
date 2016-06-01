using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    class AliasAction : ActionBase , IQueryBuilderStrategy
    {
        internal AliasAction( ) : base( QueryPart.NONE ) { }

        internal override bool CanExecute { get { return true; } }

        public void Execute( )
        {
            Tokens.EntityMetadata = QueryContext.MetaStructure;
            Tokens.EntityALIAS = Syntax.EntityALIAS;

            if ( Syntax.IsDistinct )
            {
                Tokens.AddDistinctHeader();
            }
        }
    }
}
