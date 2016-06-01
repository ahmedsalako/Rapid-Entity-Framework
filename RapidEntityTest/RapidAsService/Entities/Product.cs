using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Runtime.Serialization;

namespace Persistent.Entities
{
    [Table("Product")]
    [DataContract(IsReference = true)]
    public class Product
    {
        [Key("Id", AutoKey = true)]
        [DataMember]
        public virtual long Id { get; set; }

        [Field("ItemName", false, true)]
        [DataMember]
        public virtual string ItemName { get; set; }

        [Field("Price", false, true)]
        [DataMember]
        public virtual double Price { get; set; }

        [ManyToMany(Type = typeof(CustomerOrders), JoinTable = "OrderedProduct",
            JoinColumn = "OrderId", OwnerColumn = "ProductId", Cascade = Cascade.ALL)]
        [DataMember]
        public virtual IList<CustomerOrders> CustomerOrders
        {
            get;
            set;
        }

        public Product()
        {

        }
    }
}
