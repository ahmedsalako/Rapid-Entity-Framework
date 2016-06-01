using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace PersistentManager
{
    /// <summary>
    /// This class Dynamically cast an object to another type at runtime.
    /// This class uses reflection + Generics to do type casting at runtime.
    /// </summary>
    public class DynamicCast
    {
        public T Cast<T>(object o)
        {
            return (T)o;
        }

        public static T GetDefault<T>()
        {
            return default(T);
        }

        public static object GetDefaultReflectively(Type type)
        {
            DynamicCast dynamiccast = new DynamicCast();
            MethodInfo castMethod = dynamiccast.GetType().GetMethod("GetDefault").MakeGenericMethod(type);
            return castMethod.Invoke(dynamiccast, new object[0]);
        }

        public static object CastReflectively(object obj, Type typeToCast)
        {
            DynamicCast dynamiccast = new DynamicCast();
            MethodInfo castMethod = dynamiccast.GetType().GetMethod("Cast").MakeGenericMethod(typeToCast);
            return castMethod.Invoke(dynamiccast, new object[] { obj });
        }

        public static Type MakeGenericType( Type genericType , Type argument )
        {
            return genericType.MakeGenericType( new Type[] { argument } );
        }

        public static object CallMethod( object owner , Type type , string method , params object[] arguments )
        {
            MethodInfo methodInfo = type.GetMethod( method );

            if ( methodInfo.IsNotNull( ) )
            {
                return methodInfo.Invoke ( owner , arguments );
            }

            return null;
        }

        public static T CastReflectively<T>( object obj , Type typeToCast )
        {
            DynamicCast dynamiccast = new DynamicCast( );
            MethodInfo castMethod = dynamiccast.GetType( ).GetMethod( "Cast" ).MakeGenericMethod( typeToCast );

            return (T) castMethod.Invoke( dynamiccast , new object[] { obj } );            
        }
    }
}
