using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Runtime.Serialization;

namespace Persistent.Entities
{
    [Entity("OrderedProducts")]
    [DataContract(IsReference = true)]
    public class OrderedProduct
    {
        [Key("Id", AutoKey=true)]
        [DataMember]
        public int Id
        { get; set; }
       
        [ManyToOne(typeof(CustomerOrders), RelationColumn= "OrderId")]
        [DataMember]
        public virtual CustomerOrders Order
        {
            get;
            set;
        }

        [ManyToOne(typeof(Product), RelationColumn = "ProductId")]
        [DataMember]
        public virtual Product Product
        {
            get;
            set;
        }
    }
}
