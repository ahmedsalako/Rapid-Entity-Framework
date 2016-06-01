using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Collections;

namespace Persistent.Entities
{
    [Entity("Customer")]
    public class Customer : BaseCustomer
    {
        private IList<CustomerOrders> customerOrders;
        private Address address;
        private string firstname;

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        [Field("FirstName", false, true)]
        public virtual string FirstName
        {
            get { return firstname; }
            set { firstname = value; }
        }        

        [Field("LastName", false, true)]
        public virtual string LastName
        {
            get;
            set;
        }

        [OneToMany(typeof(CustomerOrders), RelationColumn = "CustomerId", Cascade=Cascade.ALL)]
        public virtual IList<CustomerOrders> CustomerOrders
        {
            get
            {
                return customerOrders;
            }
            set { customerOrders = value; }
        }

        [OneToOne(typeof(Address), RelationColumn = "CustomerId", JoinColumn ="Id" , IsImported = true, Cascade = Cascade.ALL)]
        public virtual Address Address
        {
            get { return address; }
            set { address = value; }
        }
    }
}
