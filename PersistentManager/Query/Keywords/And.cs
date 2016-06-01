using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query
{
    public class AND : Where
    {
        internal AND( PathExpressionFactory Path , AS As ) : base( Path , As )
        {

        }
    }
}
