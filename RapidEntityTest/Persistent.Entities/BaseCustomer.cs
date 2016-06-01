using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    public class BaseCustomer
    {
        public BaseCustomer()
        {

        }

        [Key("Id", AutoKey = true)]
        public virtual long Id
        {
            get;
            set;
        }
    }
}
