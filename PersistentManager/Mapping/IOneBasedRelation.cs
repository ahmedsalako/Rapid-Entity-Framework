using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping
{
    interface IOneBasedRelation
    {
        string RelationColumn { get; set; }
        string PropertyName { get; set; }
    }
}
