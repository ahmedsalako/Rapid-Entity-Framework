using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Collections;

namespace Persistent.Entities
{
    [Table("CustomerOrders")]
    public class CustomerOrders
    {
        [Key("Id", AutoKey = true)]
        public long Id
        {
            get;
            set;
        }

        [Field("OrderDate", false, true)]
        public DateTime? Orderdate
        {
            get;
            set;
        }

        [ManyToOne(typeof(Customer), RelationColumn = "CustomerId")]
        public virtual Customer Customer
        {
            get;
            set;
        }

        [ManyToMany(JoinTable = "OrderedProducts" ,
            Type = typeof(Product), JoinColumn = "ProductId",
            OwnerColumn = "OrderId", Cascade = Cascade.ALL)]
        public virtual IList Products
        {
            get;
            set;
        }

        [OneToMany(typeof(OrderedProduct), RelationColumn="OrderId", Cascade= Cascade.ALL)]
        public virtual IList OrderedProducts
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
    }
}
