using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query.Keywords
{
    public class Insert : Keyword
    {
        SyntaxContainer query = new SyntaxContainer( );

        public Insert()
        {
            query.Add(QueryPart.INSERT);
            query.QueryContext = new QueryContext(QueryType.Insert);
        }


        public Insert(params object[] parameters) : this()
        {
            query.Add( QueryPart.ENTITY_PARAMETERS , GetParameterNames( parameters ) );
        }

        public Values Into( Type parameter )
        {
            query.QueryContext.EntityType = parameter;

            return new Values(query, QueryType.Insert);
        }
    }
}
