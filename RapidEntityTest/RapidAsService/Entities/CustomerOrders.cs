using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Collections;
using System.Runtime.Serialization;

namespace Persistent.Entities
{
    [Table("CustomerOrders")]
    [DataContract(IsReference= true)]
    public class CustomerOrders
    {
        [Key("Id", AutoKey = true)]
        public long Id
        {
            get;
            set;
        }

        [Field("OrderDate", false, true)]
        [DataMember]
        public DateTime? Orderdate
        {
            get;
            set;
        }

        [ManyToOne(typeof(Customer), RelationColumn = "CustomerId")]
        [DataMember]
        public virtual Customer Customer
        {
            get;
            set;
        }

        [ManyToMany(JoinTable = "OrderedProduct",
            Type = typeof(Product), JoinColumn = "ProductId",
            OwnerColumn = "OrderId", Cascade = Cascade.ALL)]
        [DataMember]
        public virtual IList<Product> Products
        {
            get;
            set;
        }

        [OneToMany(typeof(OrderedProduct), RelationColumn="OrderId", Cascade= Cascade.ALL)]
        [DataMember]
        public virtual IList<OrderedProduct> OrderedProducts
        {
            get;
            set;
        }

        public string OrderName
        {
            get
            {
                if (null == Orderdate)
                    return "Null Order Date";

                return Orderdate.Value.ToLongDateString();
            }
        }

        public CustomerOrders()
        {

        }
    }
}
