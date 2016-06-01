using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using PersistentManager.Collections;

namespace PersistentManager
{
    public class ConcreteCollectionDiscovery
    {
        private const string GENERIC_ILIST_NAME = "System.Collections.Generic.IList`1";
        private const string GENERIC_LIST_NAME = "System.Collections.Generic.List`1";
        private const string GENERIC_IENUMERABLE_NAME = "System.Collections.Generic.IEnumerable`1";
        private const string GENERIC_ICOLLECTION_NAME = "System.Collections.Generic.ICollection`1";
        private const string GENERIC_FRAMEWORK_COLLECTION = "PersistentManager.Collections.IFrameworkList`1";

        private const string ILIST_NAME = "System.Collections.IList";
        private const string ARRAYLIST_NAME = "System.Collections.ArrayList";
        private const string ENUMERABLE_NAME = "System.Collections.IEnumerable";
        private const string ICOLLECTION_NAME = "System.Collections.ICollection";
        private const string FRAMEWORK_COLLECTION = "PersistentManager.Collections.FrameworkList";

        public static Type GetConcreteFrameworkImplementor( Type type )
        {
            if ( type.IsGenericType )
                return GetGenericImplementor( type , true );

            return GetNonGenericImplementor( type , true );
        }

        public static Type GetConcreteImplementor( Type type )
        {
            if ( type.IsGenericType )
                return GetGenericImplementor( type , false );

            return GetNonGenericImplementor( type , false );
        }

        public static Type GetConcreteImplementorWithGenericType( Type type )
        {
            if ( type.IsGenericType )
            {
                Type[] arguments = type.GetGenericArguments( );
                return MakeGenericBaseClassConcrete( GENERIC_LIST_NAME , arguments[0] );

            }

            return GetNonGenericImplementor( type , false );
        }

        public static bool IsGeneric( Type type )
        {
            return ( type.IsGenericType );
        }

        private static Type GetGenericImplementor( Type type , bool shouldUseDefault )
        {
            if ( shouldUseDefault )
            {
                Type[] arguments = type.GetGenericArguments( );
                return GetGenericImplementation( type , arguments[0] );
            }
            else
            {
                Type[] arguments = type.GetGenericArguments( );
                return GetGenericImplementation( type , arguments[0].FullName );
            }
        }

        private static Type GetNonGenericImplementor( Type type , bool shouldUseDefault )
        {
            return GetNonGenericImplementation( shouldUseDefault );
        }

        private static Type GetGenericImplementation( Type type , string genericArgument )
        {
            switch ( type.FullName )
            {
                case GENERIC_ILIST_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_LIST_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_ICOLLECTION_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_IENUMERABLE_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );
                default:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );
            }
        }

        private static Type GetGenericImplementation( Type type , Type genericArgument )
        {
            switch ( type.FullName )
            {
                case GENERIC_ILIST_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_LIST_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_ICOLLECTION_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );

                case GENERIC_IENUMERABLE_NAME:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );
                default:
                    return MakeGeneric( GENERIC_LIST_NAME , genericArgument );
            }
        }

        public static Type MakeGeneric( string genericName , string genericArgument )
        {
            Type implementor = Type.GetType( genericName );
            return implementor.MakeGenericType( new Type[] { Type.GetType( "System.Object" ) } );
        }

        public static Type MakeGenericBaseClassConcrete( string genericName , Type type )
        {
            Type implementor = Type.GetType( genericName );
            return implementor.MakeGenericType( new Type[] { type } );
        }

        public static Type MakeGeneric( string genericName , Type type )
        {
            Type implementor = Type.GetType( GENERIC_FRAMEWORK_COLLECTION );
            return implementor.MakeGenericType( new Type[] { type } );
        }

        private static Type MakeNoNGeneric( string typeName )
        {
            return Type.GetType( typeName );
        }

        private static Type GetNonGenericImplementation( bool shouldUseDefault )
        {
            if ( shouldUseDefault )
                return MakeNoNGeneric( FRAMEWORK_COLLECTION );

            return MakeNoNGeneric( ARRAYLIST_NAME );
        }

        public static object MakeInstance( Type type , params object[] parameters )
        {
            if ( null == parameters )
                return Activator.CreateInstance( type );

            return Activator.CreateInstance( type , parameters );
        }

        public static void AddElement( object instance , object element )
        {
            MethodInfo addMethod = instance.GetType( ).GetMethod( "Add" );
            addMethod.Invoke( instance , new object[] { element } );
        }

        public static bool Contains( object instance , object element )
        {
            MethodInfo containsMethod = instance.GetType( ).GetMethod( "Contains" );
            return ( bool ) containsMethod.Invoke( instance , new object[] { element } );
        }

        public static object GenericCreate( Type genericType , Type argument , params object[] args )
        {
            Type type = genericType.MakeGenericType( new Type[] { argument } );
            return Activator.CreateInstance( type );
        }

        public static IEnumerator GetEnumerator( object instance )
        {
            MethodInfo getenumeratorMethod = instance.GetType( ).GetMethod( "GetEnumerator" );
            return ( IEnumerator ) getenumeratorMethod.Invoke( instance , null );
        }
    }
}
