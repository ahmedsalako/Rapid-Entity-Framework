using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PersistentManager.Metadata;
using PersistentManager.Descriptors;
using PersistentManager.Runtime;
using PersistentManager.Contracts;

namespace PersistentManager.Cache
{
    public static class RREChangeListeners
    {
        public static object GetInstanceProperty( string propertyName , object instance )
        {
            //if( RuntimeManager.IsInScope( ) )
            //{
            //    RuntimeTransactionScope scope = RuntimeTransactionScope.Current;
            //    {
            //        if( scope.IsAuditTrailed( instance ) )
            //        {
            //            return scope.GetCurrentValue( propertyName , instance );
            //        }
            //    }
            //}

            return null;
        }

        public static object SetInstanceProperty( string propertyName , object instance , object value )
        {
            if ( MetaDataManager.IsEntityLoaded( instance ) )
            {
                object originalValue = MetaDataManager.GetPropertyValue( propertyName , instance );
                IDictionary<string , IChangeCatalog> changes = GhostGenerator.GetSelfChanges( instance );

                if ( !value.Equals( originalValue ) )
                {
                    if ( changes.ContainsKey( propertyName ) )
                    {
                        changes[propertyName].NewValue = value;
                    }
                    else
                    {
                        Type type = MetaDataManager.GetEntityType(instance).GetMetataData()
                                                        .GetPropertyMappingIncludeBase(propertyName)
                                                        .DeclaringType;

                        Guid transactionId = RuntimeTransactionScope.IsInScope ? RuntimeTransactionScope.Current.TransactionId : Guid.Empty;

                        changes.Add( propertyName , new ChangeCatalog( propertyName , originalValue , value , type , transactionId ) );
                    }
                }
            }

            return null;
        }
    }
}
