using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [Entity("Address")]
    public class Address
    {
        [Key("Id", AutoKey = true)]
        public long Id { get; set; }

        [OneToOne(typeof(Customer), RelationColumn = "CustomerId", JoinColumn = "Id")]
        public virtual Customer Customer { get; set; }

        [Field("Address", false, true)]
        public virtual string FullAddress { get; set; }

        [Field("PostCode", false, true)]
        public virtual string PostCode { get; set; }

        [Field("County", false, true)]
        public virtual string Country { get; set; }


        public Address()
        {

        }
    }
}
