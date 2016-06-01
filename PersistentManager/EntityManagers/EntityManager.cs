using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Sql;
using System.Linq;
using PersistentManager.Query;
using System.Runtime.Remoting.Messaging;
using PersistentManager.Cache;
using PersistentManager.Util;
using PersistentManager.Exceptions;
using PersistentManager.Exceptions.EntityManagerException;
using PersistentManager.Query.Keywords;
using PersistentManager.Linq;
using PersistentManager.Descriptors;
using PersistentManager.Runtime;
using System.Linq.Expressions;
using PersistentManager.Ghosting;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using PersistentManager.Contracts;

namespace PersistentManager
{
    public class EntityManager : IEntityManager
    {
        event OnTransactionChanging TransactionChangingEvent;
        private Transaction transaction;
        private DbConnection connection;
        private IConfiguration configuration;
        private SessionRuntime session;
        private string name = string.Empty;

        public static EntityManager OpenOn( string name )
        {
            return new EntityManager( name );
        }

        public EntityManager( IConfiguration configuration )
        {
            this.configuration = configuration;
        }

        public EntityManager( )
        {
            this.configuration = ConfigurationBase.GetCurrentConfiguration( );
        }

        public EntityManager(string configurationName)
        {
            this.name = configurationName;
            this.configuration = ConfigurationBase.GetCurrentConfiguration( );
        }

        public virtual void OpenDatabaseSession( )
        {
            try
            {
                session = ConfigurationBase.CreateSession( name );
                connection = (DbConnection) session.DataBaseProvider.Connection;
                transaction = new Transaction( session.DataBaseProvider.Transaction , session );

                OnTransactionChangingEvent += new OnTransactionChanging( transaction.OnTransactionChangingHandler );             
            }
            catch ( Exception ex )
            {
                Throwable.ThrowException( "Could not open Database Session" , ex );
            }
        }

        public virtual void BeginTransaction( )
        {
            TransactionChangingEvent( connection.BeginTransaction( ) );       
        }

        public virtual void Commit( )
        {
            transaction.Commit();
        }

        public virtual void CloseDatabaseSession( )
        {
            session.CloseActiveReaders( );
            CloseConnection( );
        }

        public virtual void CommitAndClose( )
        {
            try
            {
                Commit( );
            }
            catch ( Exception ex )
            {
                throw ex;
            }
            finally
            {
                CloseConnection( );
            }
        }

        public virtual void RollBack( )
        {
            transaction.RollBack();
        }

        /// <summary>
        /// Undo Changes. Undos all the changes made in and out of a transactional scope
        /// </summary>
        /// <typeparam name="T"> An Entity Type </typeparam>
        /// <param name="entity">An entity instance</param>
        /// <returns> Returns the changed entity </returns>
        public virtual T UndoChanges<T>( T entity )
        {
            return ( T ) session.UndoAllChanges( entity );
        }

        /// <summary>
        /// Undo Transactional changes . This will undo changes made within a transaction context
        /// </summary>
        /// <typeparam name="T"> An Entity Type </typeparam>
        /// <param name="entity"> An entity instance </param>
        /// <returns> Returns the changed entity </returns>
        public virtual T UndoTransactionChanges<T>( T entity )
        {
            return ( T )session.UndoTransactionChanges( entity );
        }

        public virtual IList<T> UndoChanges<T>( IList<T> entities )
        {
            return ( IList<T> )session.UndoAllChanges( entities ).ToList( );
        }

        public virtual IList<T> UndoTransactionChanges<T>( IList<T> entities )
        {
            return ( IList<T> )session.UndoTransactionChanges( entities ).ToList( );
        }

        public virtual ICollection<IChangeCatalog> FetchChanges<T>( T entity )
        {
            return session.GetChanges( entity );
        }

        public virtual void RollBackAndClose( )
        {
            RollBack( );

            CloseConnection( );
        }

        private void CloseConnection( )
        {
            if ( connection != null && connection.State == ConnectionState.Open )
                connection.Close( );
        }

        public virtual object LoadEntity( Type type , params object[] entityKey )
        {
            return session.LoadEntity( type , entityKey );
        }

        public virtual T InterfaceProxy<T>( params object[] contructorArgs )
        {
            return session.CreateInstance<T>( contructorArgs );
        }

        public virtual T Attach<T>( T entity )
        {
            return ( T ) session.Merge( entity );
        }

        public virtual T Detach<T>( T entity )
        {
            return (T) session.Detach<T>( entity );
        }

        public virtual IEnumerable<T> Detach<T>( IList<T> entities )
        {
            foreach ( var entity in entities )
            {
                yield return Detach<T>( entity );
            }
        }

        public virtual T LoadEntity<T>( params object[] objectIds )
        {
            return ( T )LoadEntity( typeof( T ) , objectIds );
        }

        public XmlDocument GetXMLFromEntity<T>( params object[] objectIds )
        {
            T entity = Detach<T>( LoadEntity<T>( objectIds ) );
            return new EntityXMLSerializer( ).SerializeEntity( entity );
        }

        public XmlDocument GetXMLFromEntity<T>( T entity )
        {
            return new EntityXMLSerializer( ).SerializeEntity( Detach<T>( entity ) );
        }

        public T GetEntityFromXml<T>( XmlDocument xml )
        {
            return new EntityXMLSerializer( ).DeserializeEntity<T>( xml );
        }

        public virtual void SaveEntity( object entity )
        {
            session.SaveEntity( MetaDataManager.GetEntityType( entity ) , entity );
        }

        public virtual void SaveEntity( object entity , params object[] objectIds )
        {
            session.SaveEntity( MetaDataManager.GetEntityType( entity ) , entity , objectIds , false , true );
        }

        public virtual object CreateNewEntity( object entity )
        {
            return session.CreateNewEntity( entity.GetType( ) , ref entity , RuntimeBehaviour.DoNothing );
        }

        public virtual T CreateOrUpdate<T>( T entity )
        {
            return ( T ) session.CreateOrUpdate( entity );
        }

        public virtual T CreateNewEntity<T>( ref T entity )
        {
            entity = ( T )session.CreateNewEntityReturnsEntity( MetaDataManager.GetEntityType( entity ) , entity );

            return entity;
        }

        public virtual T AsAlias<T>( )
        {
            return CallStack.CreateAlias<T>( );
        }

        public virtual AS<TEntity> From<TEntity>()
        {
            return PersistentManager.Query.From.FromEntity<TEntity>(AsAlias<TEntity>());
        }

        public virtual string StringAlias( object entity )
        {
            return CallStack.GetIdentifier( entity );
        }

        public virtual IEnumerable<T> GetAll<T>( )
        {
            foreach ( T value in GetAllLazily( typeof( T ) ) )
                yield return value;
        }

        public IList<T> GetAllLazily<T>( string[] criterias , object[] values )
        {
            return session.GetAllLazily<T>( criterias , values );
        }

        public int Count<T>( string[] criterias , object[] values )
        {
            return session.Count( typeof(T) , criterias , values );
        }

        public int Count<T>()
        {
            return session.Count( typeof( T ) , null , null );
        }

        public IList<T> GetAllLazily<T>()
        {
            return (IList<T>) session.GetAllLazily( typeof( T ) , true );
        }

        public IList GetAllLazily( Type entity )
        {
            return (IList) session.GetAllLazily( entity , false );
        }

        public IEnumerable<T> Limit<T>( int Start , int End , string[] properties , object[] values )
        {
            foreach( T entity in session.GetRange<T>( Start , End , properties , values ) )
                yield return entity;
        }

        public IEnumerable<T> Limit<T>( int Start , int End )
        {
            return Limit<T>( Start , End , null , null );
        }        

        public virtual IList GetAll( Type entity )
        {
            return session.GetAll( entity );
        }

        public virtual IList FindEntitiesByFields( Type entity , string[] parameterNames , object[] parameterValues )
        {
            return session.FindEntityByProperties( entity , parameterNames , parameterValues );
        }

        public virtual IParameter<T> FindEntitiesByFields<T>( )
        {
            return new QueryParameter<T>( );
        }

        public virtual T First<T>( )
        {
            return ( T ) session.FindFirst( typeof( T ) , null , null );
        }

        public virtual void RemoveEntity( object entity , params object[] objectId )
        {
            session.RemoveEntity( entity.GetType( ) , entity , objectId );
        }

        public virtual void RemoveEntity( object entity )
        {
            session.RemoveEntity( entity.GetType( ) , entity );
        }

        /// <summary>
        /// This kind of remove will not cascade. even when cascade is set.
        /// It is best used on an entity without referencial integrity or an entity
        /// whose deletion does not trigger foreign key violations.
        /// </summary>
        /// <param name="type">Entity Type</param>
        /// <param name="names">properties to use in the where clause</param>
        /// <param name="values">values of properties in the where clause</param>
        /// <returns></returns>
        public virtual bool RemoveEntityNonCascade( Type type , string[] names , object[] values )
        {
            return session.RemoveEntityByFieldName(type, names, values);
        }

        public virtual IQueryResult GetSelectQueryResult( SyntaxContainer syntax )
        {
            return syntax.SelectResult( );
        }

        public virtual object GetScalarResult( SyntaxContainer syntax )
        {
            return syntax.ExecuteScalarInternal( ).ScalarResult;
        }

        public virtual bool ExecuteUpdateQuery( SyntaxContainer syntax )
        {
            return syntax.ExecuteUpdate( );
        }

        public string GetLastQueryLog()
        {
            return session.DataBaseProvider.QueryLog;
        }

        #region IDisposable Members

        public void Dispose( )
        {
            try
            {
                CommitAndClose( );
            }
            finally
            {
                session.Dispose( );
            }
        }

        #endregion

        public IQueryable<T> AsQueryable<T>( )
        {
            RapidLinqQueryProvider<T> provider = new RapidLinqQueryProvider<T>( );
            RapidLinqQueryable<T> queryable = new RapidLinqQueryable<T>( provider );

            return queryable;
        }

        internal virtual event OnTransactionChanging OnTransactionChangingEvent
        {
            add
            {
                if ( !TransactionChangingEvent.IsNull( ) )
                {
                    TransactionChangingEvent += value;
                }
                else
                {
                    TransactionChangingEvent = new OnTransactionChanging( value );
                }
            }
            remove
            {
                if ( !TransactionChangingEvent.IsNull( ) )
                {
                    TransactionChangingEvent -= value;
                }
            }
        }
    }
}
