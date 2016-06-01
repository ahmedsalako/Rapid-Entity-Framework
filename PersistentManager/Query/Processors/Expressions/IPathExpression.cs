using System;
using PersistentManager.Descriptors;
namespace PersistentManager.Query.Processors
{
    internal interface IPathExpression
    {
        void AddCriteria( Criteria criteria );
        EntityMetadata MetaData { get; }
    }
}
