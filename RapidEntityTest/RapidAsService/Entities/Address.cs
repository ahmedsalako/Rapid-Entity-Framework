using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Runtime.Serialization;
using RapidAsService.Entities;

namespace Persistent.Entities
{
    [Entity("Address")]
    [DataContract(IsReference=true)]
    public class Address
    {
        [Key("Id", AutoKey = true)]
        [DataMember]
        public long Id { get; set; }

        [OneToOne(typeof(Customer), RelationColumn = "CustomerId")]
        [DataMember]
        public virtual Customer Customer { get; set; }

        [Field("Address", false, true)]
        [DataMember]
        public virtual string FullAddress { get; set; }

        [Field("PostCode", false, true)]
        [DataMember]
        public virtual string PostCode { get; set; }

        [Field("County", false, true)]
        [DataMember]
        public virtual string Country { get; set; }

        [DataMember]
        public Mini Mini { get; set; }


        public Address()
        {

        }
    }
}
