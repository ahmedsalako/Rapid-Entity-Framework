using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using PersistentManager.Runtime;
using PersistentManager.Descriptors;

namespace PersistentManager.Cache
{
    [Serializable]
    internal class CacheObject : ICacheObject
    {
        public long RequestCount { get; set; }
        public string Key { get; set; }
        public Type Type { get; set; }
        public object EntityInstance { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool MarkedForDeletion { get; set; }
        public DateTime Expiry { get; set; }

        public bool IsDirty { get; set; }

        public bool WasFlushed { get; set; }
        public bool CreatedUncommited { get; set; }
        public Guid CreatedTransactionId { get; set; }
        public bool PropertyChangedAssigned { get; set; }

        internal CacheObject( string key , Type type , object objectInstance , Guid transactionId ) 
            : this( key , objectInstance , transactionId , false )
        {            
            Type = type;                        
        }

        internal CacheObject( string key , object objectInstance , Guid transactionId , bool firePropertyChanges )
        {
            Key = key;            
            EntityInstance = objectInstance;
            TimeCreated = DateTime.Now;
            Expiry = TimeCreated.AddMinutes( ConfigurationBase.CacheDuration );

            if ( firePropertyChanges )
            {
                AddPropertyChangeListener( );
            }
        }

        public void AddPropertyChangeListener( )
        {
            if ( !PropertyChangedAssigned )
            {
                INotifyPropertyChanged notify = ( INotifyPropertyChanged )EntityInstance;
                notify.PropertyChanged += new PropertyChangedEventHandler( HandlePropertyChangeEvent );
                PropertyChangedAssigned = true;
            }
        }

        public void RemovePropertyChangeListener( )
        {
            INotifyPropertyChanged notify = ( INotifyPropertyChanged )EntityInstance;
            notify.PropertyChanged -= new PropertyChangedEventHandler( HandlePropertyChangeEvent );
            PropertyChangedAssigned = false;
        }

        internal void AddPropertyChangeListener( object objectInstance )
        {
            if ( !PropertyChangedAssigned )
            {
                INotifyPropertyChanged notify = ( INotifyPropertyChanged )objectInstance;
                notify.PropertyChanged += new PropertyChangedEventHandler( HandlePropertyChangeEvent );
                PropertyChangedAssigned = true;
            }
        }

        internal void RemovePropertyChangeListener( object objectInstance )
        {
            INotifyPropertyChanged notify = ( INotifyPropertyChanged )objectInstance;
            notify.PropertyChanged -= new PropertyChangedEventHandler( HandlePropertyChangeEvent );
            PropertyChangedAssigned = false;
        }

        public void UpdateWithAudit( DirtyTrail audit )
        {
            RemovePropertyChangeListener( );

            foreach ( DirtyState change in audit )
            {
                if ( change.Value.IsNotNull( ) )
                {
                    MetaDataManager.SetPropertyValue( change.PropertyName , EntityInstance , change.Value );
                }
            }

            AddPropertyChangeListener( );
        }

        public void HandlePropertyChangeEvent( Object sender , PropertyChangedEventArgs args )
        {
              IsDirty = true;
        }

        public void CleanseDirtyState()
        {
            IsDirty = false;
            WasFlushed = false;
        }

        public void ClearSelfChanges()
        {
            GhostGenerator.ClearSelfChanges( EntityInstance );
        }
    }
}
