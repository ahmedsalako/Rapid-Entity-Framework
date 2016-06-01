using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Runtime.Serialization;

namespace Persistent.Entities
{
    [DataContract(IsReference=true)]
    public class BaseCustomer
    {
        public BaseCustomer()
        {

        }

        [Key("Id", AutoKey = true)]
        [DataMember]
        public virtual long Id
        {
            get;
            set;
        }
    }
}
