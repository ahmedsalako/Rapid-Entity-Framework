using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Exceptions.EntityManagerException;

namespace PersistentManager.Exceptions
{
    internal class Throwable
    {
        internal static PersistentException ThrowEntityLoadException( string message , Exception innerException )
        {
            throw new EntityLoadException( message , innerException );
        }

        internal static PersistentException ThrowEntityPersistException( string message , Exception innerException )
        {
            throw new EntityPersistException( message , innerException );
        }

        internal static T ThrowException<T>( string message , Exception innerException )
        {
            throw ( PersistentException )Activator.CreateInstance( typeof( T ) , message , innerException );
        }

        internal static void ThrowException( string message )
        {
            throw new Exception( message );
        }

        internal static void ThrowException( string message , Exception exception )
        {
            throw new Exception( message , exception );
        }

        internal static void ThrowOnTrue( bool isTrue , string message )
        {
            if ( isTrue )
            {
                throw new Exception( message );
            }
        }
    }
}
