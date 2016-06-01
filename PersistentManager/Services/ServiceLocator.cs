using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Services.Interfaces;

namespace PersistentManager
{
    internal static class ServiceLocator
    {
        static Dictionary<string , object> pool = new Dictionary<string , object>( );

        static ServiceLocator()
        {

        }

        internal static void AddService<T>( T instance )
        {
            pool.Add( typeof( T ).FullName , instance );
        }

        internal static void ChangeService<T>( T instance )
        {
            pool[typeof( T ).FullName] = instance;
        }

        internal static T Locate<T>( )
        {
            return (T) pool[typeof( T ).FullName];
        }

        private static bool IsPooled( string key )
        {
            return pool.ContainsKey( key );
        }
    }
}
