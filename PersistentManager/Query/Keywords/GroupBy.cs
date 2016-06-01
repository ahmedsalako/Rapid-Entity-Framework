using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query
{
    public class GroupBy : Keyword
    {
        internal GroupBy( PathExpressionFactory Path , AS As )
        {
            this.Path = Path;
            this.Identifier = As;
        }

        public OrderBy OrderBy( params object[] parameters )
        {
            return Identifier.OrderBy( parameters  );
        }

        public OrderBy OrderByDescending( params object[] parameters )
        {
            return Identifier.OrderByDescending( parameters  );
        }
    }
}
