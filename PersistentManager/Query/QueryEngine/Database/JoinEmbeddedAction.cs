using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class JoinEmbeddedAction : ActionBase , IQueryBuilderStrategy
    {
        internal JoinEmbeddedAction( ) : base( QueryPart.JOINED_EMBEDDED ) { }

        internal JoinEmbeddedAction( ContextData contextData , IDatabaseProvider provider )
            : base( QueryPart.JOINED_EMBEDDED )
        {
            base.Execute( contextData , provider );
        }

        internal override bool CanExecute { get { return true; } }

        internal override ContextData Execute( ContextData ContextData , IDatabaseProvider provider )
        {
            return base.Execute( ContextData , provider );
        }

        public void Execute( )
        {
        }

        public void ExecuteLeftOuterJoin( string alias )
        {
            if ( Syntax.JoinEmbeddedReference.Count > 0 )
            {
                foreach ( var embedded in Syntax.JoinEmbeddedReference )
                {
                    EmbeddedEntity entityType = QueryContext.MetaStructure.GetEmbeddedIncludeBase( embedded.Value.Value );
                    KeyValuePair<string , string> reference = Syntax.JoinEmbeddedRelation[embedded.Key];

                    if ( reference.Key == alias )
                    {
                        string joinColumn = NamingStrategy.DecorateName( embedded.Key ).Dot( NamingStrategy.DecorateName( entityType.JoinColumn ) );
                        string relationColumn = NamingStrategy.DecorateName( reference.Key ).Dot( NamingStrategy.DecorateName( entityType.RelationColumn ) );

                        Tokens.AddFormatted( QueryPart.JOINED_INHERITANCE , " LEFT OUTER JOIN {0} {1} ON ({2}) " ,
                                     NamingStrategy.DecorateName( MetaDataManager.GetSchemaName( entityType.Type ) ) ,
                                     NamingStrategy.DecorateName( embedded.Key ) ,
                                     SQLBuilderHelper.PrepareJoin( relationColumn , joinColumn ) );
                    }
                }
            }
        }
    }
}
