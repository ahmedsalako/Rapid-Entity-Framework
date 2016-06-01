using System;
using PersistentManager.Descriptors;
namespace PersistentManager.Cache
{
    public interface ICacheObject
    {
        Guid CreatedTransactionId { get; set; }
        bool CreatedUncommited { get; set; }
        object EntityInstance { get; set; }
        DateTime Expiry { get; }
        bool IsDirty { get; set; }
        string Key { get; set; }
        long RequestCount { get; set; }
        DateTime TimeCreated { get; set; }
        Type Type { get; set; }
        bool WasFlushed { get; set; }
        bool MarkedForDeletion { get; set; }
        bool PropertyChangedAssigned { get; set; }

        void AddPropertyChangeListener( );
        void RemovePropertyChangeListener( );
        void UpdateWithAudit( DirtyTrail audit );
        void ClearSelfChanges();
        void CleanseDirtyState();
    }
}
