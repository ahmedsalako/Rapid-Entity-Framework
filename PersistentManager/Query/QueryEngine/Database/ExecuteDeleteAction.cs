using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class ExecuteDeleteAction : ActionBase , IQueryBuilderStrategy
    {
        internal ExecuteDeleteAction( ) : base( QueryPart.NONE ) { }

        internal override bool CanExecute
        {
            get
            {
                return true;
            }
        }

        public void Execute( )
        {
            object[] uniqueKeysValues = null;
            string[] uniquekeyNames = null;
            bool hasWhereClause = false;
            Tokens.QueryableEntityType = QueryContext.EntityType;
            Tokens.AddFormatted( QueryPart.DELETE , "DELETE FROM {0} " , QueryContext.MetaStructure.SchemaName );

            if ( QueryContext.EntityInstance.IsNotNull( ) )
            {
                uniqueKeysValues = MetaDataManager.GetUniqueKeys( QueryContext.EntityInstance );
                uniquekeyNames = MetaDataManager.GetUniqueKeyNames( QueryContext.EntityType );
            }
            else if ( Syntax.ConditionalParameters.Count > 0 )
            {
                ICollection<Criteria> criterias = Syntax.ConditionalParameters.Values;
                uniquekeyNames = ( from criteria in criterias
                                   select criteria.Name ).ToArray( );

                uniqueKeysValues = ( from criteria in criterias
                                     select criteria.Value ).ToArray( );
            }

            Tokens.AddFormatted( QueryPart.DELETE , " WHERE {0} " ,
                SQLBuilderHelper.PrepareConditionalStatement( CurrentProvider , Tokens.QueryableEntityType , QueryContext.QueryType , ref hasWhereClause , string.Empty ,
                uniquekeyNames , uniqueKeysValues , Tokens.Parameters ).ToString( ) );
        }
    }
}
