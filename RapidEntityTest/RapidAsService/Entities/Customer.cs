using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using System.Collections;
using System.Runtime.Serialization;

namespace Persistent.Entities
{
    [Entity("Customer")]
    [DataContract(IsReference=true)]
    public class Customer : BaseCustomer
    {
        private string fullname;
        private IList<CustomerOrders> customerOrders;
        private Address address;

        [DataMember]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
            set { fullname = value; }
        }

        [Field("FirstName", false, true)]
        [DataMember]
        public virtual string FirstName
        {
            get;
            set;
        }        

        [Field("LastName", false, true)]
        [DataMember]
        public virtual string LastName
        {
            get;
            set;
        }

        [OneToMany(typeof(CustomerOrders), RelationColumn = "CustomerId", Cascade = Cascade.ALL)]
        [DataMember]
        public virtual IList<CustomerOrders> CustomerOrders
        {
            get
            {
                return customerOrders;
            }
            set { customerOrders = value; }
        }

        [OneToOne(typeof(Address), RelationColumn = "CustomerId", IsImported = true, Cascade = Cascade.ALL)]
        [DataMember]
        public virtual Address Address
        {
            get { return address; }
            set { address = value; }
        }

        public Customer()
        {

        }
    }
}
