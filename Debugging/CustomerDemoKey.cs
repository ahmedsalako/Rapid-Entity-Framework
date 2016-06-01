using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Debugging
{
    public class CustomerDemoKey
    {
        [Key("CustomerID", AutoKey = false)]
        public virtual string CustomerID { get; set; }

        [Key("CustomerTypeID", AutoKey = false)]
        public virtual string CustomerTypeID { get; set; }
    }
}
