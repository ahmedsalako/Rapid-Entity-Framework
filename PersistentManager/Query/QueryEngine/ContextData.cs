using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query
{
    internal class ContextData
    {
        internal ContextData( SyntaxContainer syntax , QueryContext context )
        {
            Syntax = syntax;
            QueryContext = context;
            SQLTokenizer = new SQLTokenizer( ) 
                           { 
                               QueryContext = context , 
                               QueryableEntityType = context.EntityType ,
                               EntityALIAS = syntax.EntityALIAS
                           };
        }

        internal SyntaxContainer Syntax { get; set; }
        internal QueryContext QueryContext { get; set; }
        internal SQLTokenizer SQLTokenizer { get; set; }
    }
}
