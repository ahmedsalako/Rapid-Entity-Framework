using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query.Keywords
{
    public class Values
    {
        internal SyntaxContainer query;
        internal QueryType queryType;

        internal Values(SyntaxContainer query, QueryType queryType)
        {
            this.query = query;
            this.queryType = queryType;
        }

        public DML ParameterValues( object entity )
        {
            QueryContext context = query.QueryContext;
            context.EntityType = entity.GetType();
            context.EntityInstance = entity;
            context.MetaStructure = MetaDataManager.PrepareMetadata( entity.GetType() );
            context.Keys = MetaDataManager.GetUniqueKeys(entity);
            query.QueryContext = context;

            return new DML(query);
        }

        internal DML ParameterValues( object entity , QueryContext context )
        {
            query.QueryContext = context;
            return new DML(query);
        }
    }
}
