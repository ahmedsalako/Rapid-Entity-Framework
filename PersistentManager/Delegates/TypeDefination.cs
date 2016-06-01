using System;
using PersistentManager.Ghosting.Event;
using System.Data;
using System.Data.Common;
namespace PersistentManager
{
    public delegate object GetAll( Type type , string[] relationalColumn , object[] findByValue );
    public delegate object GetAllMultipleCriteria( Type type , string joinTable , string[] relationalColumn , string[] ownerColumn , object[] findByValue );
    public delegate object GetType( Type type , string[] relationalColumn , object[] findByValue );

    //Ghost PropertyHandler Delegate
    internal delegate void PropertyChanged( string propertyName , GhostPropertyChangeEventArgs args );
    internal delegate bool FilterMember( string name );

    //Lazy Loading Read Completed
    public delegate void LazyLoadingReadCompleted( IDataReader dataReader );

    //Entity Manager through Runtime Manager. Notifies All Open DataReaders of the Close Attempt
    public delegate bool CanCloseConnection( );

    //Registers a data reader and enables the runtime engine. To operate on all registered datareaders.
    internal delegate void RegisterDataReader( IDataReader dataReader );

    //Activates transaction changing event.
    internal delegate void OnTransactionChanging( IDbTransaction transaction );

    internal delegate object LoadEntitySingle( Type entityType , IDataReader currentReader );
}