using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using System.Linq;
using PersistentManager.Descriptors;
using PersistentManager.Ghosting;

namespace PersistentManager.Query
{
    public class JOIN : Keyword
    {
        PathExpression Relation { get; set; }

        internal JOIN( PathExpressionFactory Path , PathExpression relation )
        {
            this.Path = Path ;
            Relation = relation ;
        }

        public AS As( string alias )
        {
            Relation.CanonicalAlias = alias;
            Path.Participants.Add(alias, Relation);

            return new AS( Path );
        }

        public AS As( object entity )
        {
            return As( GetIdentifier( entity ) );
        }
    }
}
