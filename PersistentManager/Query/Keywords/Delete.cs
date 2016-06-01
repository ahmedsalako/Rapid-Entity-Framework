using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query.Keywords
{
    public class Delete
    {
        SyntaxContainer query = new SyntaxContainer( );

        public Delete()
        {
            query.QueryContext = new QueryContext(QueryType.Delete);
        }

        public DeleteWhere From( Type parameter )
        {
            query.QueryContext.EntityType = parameter;

            return new DeleteWhere(query, parameter);
        }

        public DeleteWhere From( object entity )
        {
            return From( entity.GetType() );
        }
    }
}
