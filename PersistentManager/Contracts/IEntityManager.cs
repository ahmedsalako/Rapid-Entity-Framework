using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Contracts;
using PersistentManager.Query.Keywords;
using System.Collections;
using PersistentManager.Descriptors;
using System.Xml;
using PersistentManager.Query;

namespace PersistentManager
{
    public interface IEntityManager : IDisposable
    {
        T AsAlias<T>();
        IQueryable<T> AsQueryable<T>();
        T Attach<T>(T entity);
        void BeginTransaction();
        void CloseDatabaseSession();
        void Commit();
        void CommitAndClose();
        int Count<T>();
        int Count<T>(string[] criterias, object[] values);
        object CreateNewEntity(object entity);
        T CreateNewEntity<T>(ref T entity);
        T CreateOrUpdate<T>(T entity);
        IEnumerable<T> Detach<T>(IList<T> entities);
        T Detach<T>(T entity);
        bool ExecuteUpdateQuery(SyntaxContainer syntax);
        ICollection<IChangeCatalog> FetchChanges<T>(T entity);
        IList FindEntitiesByFields(Type entity, string[] parameterNames, object[] parameterValues);
        IParameter<T> FindEntitiesByFields<T>();
        T First<T>();
        IList GetAll(Type entity);
        IEnumerable<T> GetAll<T>();
        IList GetAllLazily(Type entity);
        IList<T> GetAllLazily<T>();
        IList<T> GetAllLazily<T>(string[] criterias, object[] values);
        T GetEntityFromXml<T>(XmlDocument xml);
        object GetScalarResult(SyntaxContainer syntax);
        IQueryResult GetSelectQueryResult(SyntaxContainer syntax);
        XmlDocument GetXMLFromEntity<T>(params object[] objectIds);
        XmlDocument GetXMLFromEntity<T>(T entity);
        T InterfaceProxy<T>(params object[] contructorArgs);
        IEnumerable<T> Limit<T>(int Start, int End);
        IEnumerable<T> Limit<T>(int Start, int End, string[] properties, object[] values);
        object LoadEntity(Type type, params object[] entityKey);
        T LoadEntity<T>(params object[] objectIds);
        void OpenDatabaseSession();
        void RemoveEntity(object entity);
        void RemoveEntity(object entity, params object[] objectId);
        void RollBack();
        void RollBackAndClose();
        void SaveEntity(object entity);
        void SaveEntity(object entity, params object[] objectIds);
        string StringAlias(object entity);
        IList<T> UndoChanges<T>(IList<T> entities);
        T UndoChanges<T>(T entity);
        IList<T> UndoTransactionChanges<T>(IList<T> entities);
        T UndoTransactionChanges<T>(T entity);
        string GetLastQueryLog();
    }
}
