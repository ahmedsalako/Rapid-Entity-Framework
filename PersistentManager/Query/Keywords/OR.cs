using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query
{
    public class OR : Where
    {
        internal OR( PathExpressionFactory Path , AS As ) : base( Path , As )
        {

        }
    }
}
