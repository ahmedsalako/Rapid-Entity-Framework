using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;

namespace PersistentManager.Query.QueryEngine.Database
{
    class FromAction : ActionBase , IQueryBuilderStrategy
    {
        public FromAction( JoinEmbeddedAction embeddedAction ) : base( QueryPart.FROM ) 
        {
            this.EmbeddedAction = embeddedAction;
        }

        internal override bool CanExecute
        {
            get
            {
                return true;
            }
        }

        internal virtual JoinEmbeddedAction EmbeddedAction { get; set; }

        public void Execute( )
        {
            Type type = QueryContext.EntityType;
            string name = type.IsNotNull( ) ? MetaDataManager.GetSchemaName( type ) :
                                           QueryContext.EntityTypeName;

            Tokens.AddFormatted( QueryPart.FROM , " {0} {1} " ,
                        NamingStrategy.DecorateName( name ) ,
                        NamingStrategy.DecorateName( Syntax.EntityALIAS ) );

            EmbeddedAction.ExecuteLeftOuterJoin( Syntax.EntityALIAS );

            foreach ( KeyValuePair<string , string> value in Syntax.InheritanceReference )
            {
                Tokens.AddFormatted( QueryPart.FROM , ", {0} {1} " , NamingStrategy.DecorateName( value.Value ) , NamingStrategy.DecorateName( value.Key ) );
                EmbeddedAction.ExecuteLeftOuterJoin( value.Key );
            }
        }
    }
}
