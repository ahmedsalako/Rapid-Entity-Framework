using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [DiscriminatorValue("Discriminator", "Perishable")]
    public class PerishableProduct : Product
    {
        public PerishableProduct()
            : base()
        {

        }
    }
}
