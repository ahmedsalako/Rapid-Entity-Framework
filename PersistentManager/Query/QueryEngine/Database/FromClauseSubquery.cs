using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class FromClauseSubquery : ActionBase , IQueryBuilderStrategy
    {
        internal FromClauseSubquery() : base(QueryPart.NONE) { }

        internal override bool CanExecute
        {
            get { return Syntax.FromClauseSubQueries.Count > 0; }
        }

        public void Execute()
        {
            foreach ( KeyValuePair<string, SyntaxContainer> subQuery in Syntax.FromClauseSubQueries )
            {
                RDBMSDataStore dataStoreQuery = new RDBMSDataStore( new ContextData( subQuery.Value , subQuery.Value.QueryContext ) , CurrentProvider );
                string query = dataStoreQuery.ExecuteSelect( true ).SQLTokenizer.ToString();

                Tokens.AddFormatted( QueryPart.FROM , " , ({0}) {1} ", query, subQuery.Key );
                Tokens.Parameters.AddRange( dataStoreQuery.ContextData.SQLTokenizer.Parameters );
            }
        }
    }
}
