using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [Table("Product")]
    public class Product 
    {
        public Product( )
        {

        }

        [Key("Id", AutoKey = true)]
        public virtual long Id { get; set; }

        [Field("ItemName", false, true)]
        public virtual string ItemName { get; set; }

        [Field("Price", false, true)]
        public virtual double Price { get; set; }

        [ManyToMany(Type = typeof(CustomerOrders), JoinTable = "OrderedProducts" ,
            JoinColumn = "OrderId", OwnerColumn = "ProductId", Cascade = Cascade.ALL)]
        public virtual IList<CustomerOrders> CustomerOrders
        {
            get;
            set;
        }
    }
}
