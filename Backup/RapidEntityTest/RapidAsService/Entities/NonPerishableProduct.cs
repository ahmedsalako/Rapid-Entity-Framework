using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;

namespace Persistent.Entities
{
    [DiscriminatorValue("Discriminator", "NonPerishable")]
    public class NonPerishableProduct : Product
    {
        public NonPerishableProduct()
        {

        }
    }
}
