using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping
{
    internal class Embedded
    {
        internal Type Type { get; set; }
        internal string TableRef { get; set; }
        internal string ColumnName { get; set; }
        internal string PropertyName { get; set; }
    }
}
