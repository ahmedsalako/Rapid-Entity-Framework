using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping.Events;
using PersistentManager.Descriptors;
using System.Reflection;
using PersistentManager.Exceptions;

namespace PersistentManager.Runtime
{
    internal class PersistenceInterceptor
    {
        internal static void Intercept<T>( T entity , EntityMetadata metadata , QueryType queryType )
        {
            IEnumerable<MethodMetadata> methods = metadata.Methods.Where( m => m.QueryType == queryType );

            foreach ( MethodMetadata methodMetadata in methods )
            {
                ActionEventArgs eventArgs = new ActionEventArgs( );
                object[] args = { eventArgs };

                metadata.Type.GetMethod( methodMetadata.MethodName ).Invoke( entity , args );

                if ( eventArgs.ActionState == ActionEventArgs.State.Rollback )
                {
                    Throwable.ThrowException( "The transaction was rolled back" );
                }
            }
        }

    }
}
