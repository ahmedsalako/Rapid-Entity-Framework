using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping
{
    interface IManyBasedRelation
    {
        string RightKey { get; set; }
        string LeftKey { get; set; }
        string PropertyName { get; set; }
    }
}
