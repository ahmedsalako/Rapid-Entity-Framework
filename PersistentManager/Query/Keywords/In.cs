using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Util;
using System.Linq;
using PersistentManager.Ghosting;

namespace PersistentManager.Query
{
    public class IN : Keyword
    {
        PathExpression Relation { get; set; }

        internal IN( PathExpressionFactory Path , PathExpression relation )
        {
            this.Path = Path;
            Relation = relation;
        }

        public new AS As( string alias )
        {
            Relation.CanonicalAlias = alias;
            Path.Participants.Add( alias , Relation );

            return new AS( Path );
        }

        public new AS As( object entity )
        {
            return As( GetIdentifier( entity ) );
        }
    }
}
