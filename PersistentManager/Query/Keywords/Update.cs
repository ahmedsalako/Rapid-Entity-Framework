using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query.Keywords
{
    public class Update : Keyword
    {
        SyntaxContainer query = new SyntaxContainer( );

        public Update()
        {
            query.Add( QueryPart.UPDATE );
            query.QueryContext = new QueryContext(QueryType.Update);
        }

        public Update( params object[] parameters ) : this()
        {
            query.Add( QueryPart.ENTITY_PARAMETERS , GetParameterNames( parameters ) );
        }

        public Values Into( Type parameter )
        {
            query.QueryContext.EntityType = parameter;

            return new Values( query , QueryType.Update );
        }
    }
}
