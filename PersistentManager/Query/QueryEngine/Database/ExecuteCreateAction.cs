using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;
using PersistentManager.Runtime;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class ExecuteCreateAction : ActionBase , IQueryBuilderStrategy
    {
        internal ExecuteCreateAction( ) : base( QueryPart.NONE ) { }

        internal override bool CanExecute
        {
            get
            {
                return true;
            }
        }

        public void Execute( )
        {
            StringBuilder Values = new StringBuilder( );
            Tokens.QueryableEntityType = QueryContext.EntityType;
            Tokens.AddFormatted( QueryPart.INSERT , "Insert Into {0} (" , NamingStrategy.DecorateName( QueryContext.MetaStructure.SchemaName ) );
            Values.Append( ") values(" );
            int count = 0;

            List<Criteria> criterias = Syntax.GetQueryByPart( QueryPart.ENTITY_PARAMETERS );

            bool columnRestriction = criterias.Count( v => v.Value.IsNotNull( ) ) > 0;

            RuntimeTransactionScope scope = RuntimeTransactionScope.Current;
            {
                DirtyTrail audit = QueryContext.Audit;

                if ( audit.IsNull( ) )
                    return;

                foreach ( DirtyState change in audit )
                {
                    if ( Tokens.IsNotInSelectArgument( change.ColumnName ) )
                    {
                        Tokens.AddFormatted( QueryPart.INSERT , "{0}{1}" , ( count > 0 ) ? "," : string.Empty ,
                            SQLBuilderHelper.SetQueryField( change.ColumnName ) );

                        string parameterName = SQLBuilderHelper.CreateNamedParameter( Tokens.QueryableEntityType , QueryContext.QueryType , change.ColumnName );
                        object parameterValue = change.Value ?? DBNull.Value;

                        Tokens.CreateProviderParameter( CurrentProvider , parameterName , parameterValue );
                        Tokens.SelectArguments.Add( change.ColumnName );
                        Values.AppendFormat( "{0}{1}" , ( count > 0 ) ? "," : string.Empty , parameterName );
                        count++;
                    }
                }
            }

            Tokens.AddFormatted( QueryPart.INSERT , " {0} )" , Values.ToString( ) );
        }
    }
}
