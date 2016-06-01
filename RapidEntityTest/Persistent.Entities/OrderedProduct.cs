using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [Entity("OrderedProducts")]
    public class OrderedProduct
    {
        [Key("Id", AutoKey=true)]
        public virtual int Id
        { get; set; }
       
        [ManyToOne(typeof(CustomerOrders), RelationColumn= "OrderId")]
        public virtual CustomerOrders Order
        {
            get;
            set;
        }

        [ManyToOne(typeof(Product), RelationColumn = "ProductId")]
        public virtual Product Product
        {
            get;
            set;
        }
    }
}
