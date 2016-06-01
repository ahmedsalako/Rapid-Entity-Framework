using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;
using PersistentManager;
using Persistent.Entities;
using System.Collections;

namespace RapidAsService
{
    // NOTE: If you change the class name "RapidService" here, you must also update the reference to "RapidService" in Web.config.
    public class RapidService : IRapidService
    {
        static ConfigurationFactory factory;

        static RapidService()
        {
            factory = ConfigurationFactory.GetInstance(ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString);
            factory.ProviderDialect = ProviderDialect.OleDbProvider;
            factory.CacheSettings(2, 0);
        }

        public void UpdateCustomer( Customer customer )
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();
                manager.SaveEntity( customer );
            }
        }

        public Customer GetByCustomerId(long Id)
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();
                return manager.Detach<Customer>( manager.LoadEntity(typeof(Customer), Id) as Customer );
            }
        }

        public Customer GetCustomer()
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();
                var customer = (from cus in manager.AsQueryable<Customer>() select cus).First();

                customer = manager.Detach<Customer>(customer);                          

                return customer;
            }
        }

        public Address GetAddress()
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();
                var address = (from cus in manager.AsQueryable<Address>() select cus).First();

                address = manager.Detach<Address>(address);                                 

                return address;
            }
        }
    }
}
