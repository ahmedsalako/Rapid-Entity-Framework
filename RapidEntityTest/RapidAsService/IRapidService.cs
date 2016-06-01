using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Persistent.Entities;

namespace RapidAsService
{
    // NOTE: If you change the interface name "IRapidService" here, you must also update the reference to "IRapidService" in Web.config.
    [ServiceContract]
    public interface IRapidService
    {
        [OperationContract]
        Customer GetCustomer();

        [OperationContract]
        [DataContractFormat]
        Address GetAddress();

        [OperationContract]
        Customer GetByCustomerId(long Id);

        [OperationContract]
        void UpdateCustomer(Customer customer);
    }
}
