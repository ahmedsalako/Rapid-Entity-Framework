using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;

namespace PersistentManager.Query.Keywords
{
    public class DeleteWhere : Keyword
    {
        SyntaxContainer query;
        Type entityType;

        internal DeleteWhere(SyntaxContainer query, Type entityType)
        {
            this.query = query;
            this.entityType = entityType;
        }

        public DML Where( object[] names , object[] values )
        {
            int i = 0;
            foreach (string name in GetParameterNames( names ))
            {
                if (i == 0)
                {
                    query.Add(QueryPart.WHERE, name, Condition.Equals, values[i]);
                }
                else
                {
                    query.Add(QueryPart.WHERE, name, Condition.Equals, values[i]);
                }
                i++;
            }

            AddContextData(null);
            return new DML(query);
        }

        internal DML Where( object[] names , object[] values , QueryContext context )
        {
            int i = 0;
            foreach (string name in GetParameterNames( names ))
            {
                if (i == 0)
                {
                    query.Add(QueryPart.WHERE, name, Condition.Equals, values[i]);
                }
                else
                {
                    query.Add(QueryPart.WHERE, name, Condition.Equals, values[i]);
                }
                i++;
            }

            query.QueryContext = context;
            return new DML(query);
        }

        public DML Where(object entity)
        {
            AddContextData(entity);
            return new DML(query, QueryType.Delete);
        }

        internal DML Where(object entity, QueryContext context)
        {
            query.QueryContext = context;
            return new DML(query, QueryType.Delete);
        }

        private void AddContextData(object entity)
        {
            QueryContext context = new QueryContext(QueryType.Delete);
            context.EntityType = entityType;
            context.EntityInstance = entity;

            if ( !entity.IsNull( ) )
                context.MetaStructure = MetaDataManager.PrepareMetadata( entityType );
            else
                context.MetaStructure = MetaDataManager.PrepareMetadata( entityType );

            query.QueryContext = context;
        }
    }
}
