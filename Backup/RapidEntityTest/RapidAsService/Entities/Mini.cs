using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace RapidAsService.Entities
{
    [DataContract]
    public class Mini
    {
        [DataMember]
        public string Name { get; set; }
    }
}
