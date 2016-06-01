using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PersistentManager.Metadata;
using PersistentManager.Util;
using System.Collections;

namespace PersistentManager
{
    internal static class Null
    {
        internal static object NOTHING = null;
        public static bool IsNull<T>( object value )
        {
            return ( null == value );
        }

        public static bool IsNull( this object value )
        {
            return ( null == value );
        }

        public static bool IsString( object value )
        {
            if ( value.IsNull( ) ) return false;

            return value.GetType( ).IsString( );
        }

        public static bool IsTrue( this bool value )
        {
            return value;
        }

        public static bool IsNotTrue( this bool value )
        {
            return !IsTrue( value );
        }

        public static bool IsDBNull( this object value )
        {
            if( value.IsNull() ) return false;
            if ( value.GetType( ) == typeof( DBNull ) ) return true;
            else return false;
        }

        public static bool IsEmpty( this ICollection enumerable )
        {
            if ( enumerable.IsNull( ) )
                return true;

            return enumerable.Count <= 0;
        }

        public static bool IsNotNull( this object value )
        {
            return !value.IsNull( );
        }

        public static bool IsNullOrEmpty( this object value )
        {
            if ( value.IsNull( ) )
                return true;

            return string.IsNullOrEmpty( value.ToString( ) );
        }

        public static bool IsDefault( object value )
        {
            return ( value == ToDefault( value ) );
        }

        public static bool IsDefault<T>( T value )
        {
            return ( value.Equals( ToDefaultGeneric<T>( value ) ) );
        }

        public static T ToDefaultGeneric<T>( T value )
        {
            return default( T );
        }

        public static T ToDefaultGeneric2<T>( )
        {
            return default( T );
        }

        public static object ToDefault( object value )
        {
            return Default( value );
        }

        public static object Default( object value )
        {
            MethodInfo castMethod = typeof( Null ).GetMethod( "ToDefaultGeneric" ).MakeGenericMethod( value.GetType( ) );
            return castMethod.Invoke( null , new object[] { value } );
        }

        public static object Default( Type type )
        {
            MethodInfo castMethod = typeof( Null ).GetMethod( "ToDefaultGeneric2" ).MakeGenericMethod( type );
            return castMethod.Invoke( null , new object[] { } );
        }
    }
}
