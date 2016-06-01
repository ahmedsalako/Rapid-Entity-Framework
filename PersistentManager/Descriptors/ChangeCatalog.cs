using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Contracts;

namespace PersistentManager.Descriptors
{
    [Serializable]
    public class ChangeCatalog : IChangeCatalog
    {
        public Type EntityType { get; internal set; }
        public object OriginalValue { get; private set; }
        public string PropertyName { get; internal set; }
        public Guid TransactionId { get; internal set; }
        public object NewValue { get; set; }

        public ChangeCatalog(string propertyName, object originalValue, object newValue, Type entityType, Guid transactionId)
        {
            TransactionId = transactionId;
            OriginalValue = originalValue;
            PropertyName = propertyName;
            EntityType = entityType;
            NewValue = newValue;
        }
    }
}
