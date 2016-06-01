using System;
namespace PersistentManager.Contracts
{
    public interface IChangeCatalog
    {
        Type EntityType { get; }
        object NewValue { get; set; }
        object OriginalValue { get; }
        string PropertyName { get; }
        Guid TransactionId { get; }
    }
}
