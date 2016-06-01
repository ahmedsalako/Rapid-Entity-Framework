using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PersistentManager.Util
{
    internal sealed class ArrayUtil
    {
        internal static T[] JoinArray<T>( T[] first , T[] second )
        {
            List<T> list = new List<T>( );
            list.AddRange( first );
            list.AddRange( second );

            return list.ToArray( );
        }
        internal static T[] ConvertAll<T>( Array array )
        {
            List<T> ts = new List<T>( );

            foreach ( object o in array )
            {
                ts.Add( ( T ) o );
            }

            return ts.ToArray( );
        }

        internal static string[] PrefixWith( string prefix , string[] array )
        {
            List<string> prefixed = new List<string>( );

            foreach ( string name in array )
            {
                prefixed.Add( prefix.Trim( ) + name.Trim( ) );
            }

            return prefixed.ToArray( );
        }

        internal static string GetFirstElement( string[] array )
        {
            if ( array.Length <= 0 )
                return string.Empty;

            return array[0];
        }

        internal static string[] RemoveNexElement( string[] array )
        {
            if ( array.Length <= 0 )
                return array;

            return array.Skip( 1 ).ToArray( );
        }

        internal static string[] RemoveOccurrences( string[] array , string value )
        {
            List<string> tempArray = new List<string>( );

            foreach ( string name in array )
            {
                if ( name.Trim( ) == value.Trim( ) )
                    continue;

                tempArray.Add( name );
            }

            return tempArray.ToArray( );
        }

        internal static string[] Remove( string[] array , string[] values )
        {
            List<string> tempArray = new List<string>( );

            if ( array.Length <= 0 )
                return tempArray.ToArray( );
            else if ( values.Length <= 0 )
                return array;

            foreach ( string name in array )
            {
                foreach ( string inner in values )
                {
                    if ( inner.Trim( ) == name.Trim( ) || tempArray.Contains( name ) )
                        continue;

                    tempArray.Add( name );
                }
            }

            return tempArray.ToArray( );
        }

        internal static string[] PrefixWith( string prefix , string[] array , bool ignoreLast )
        {
            List<string> prefixed = new List<string>( );
            int count = 0;

            foreach ( string name in array )
            {
                if ( ignoreLast && array.Length == ( count + 1 ) )
                {
                    prefixed.Add( name.Trim( ) );
                    continue;
                }

                prefixed.Add( prefix.Trim( ) + name.Trim( ) );
                ++count;
            }

            return prefixed.ToArray( );
        }

        internal static string ToString( string[] array )
        {
            string value = string.Empty;

            foreach ( string element in array )
            {
                value += element;
            }

            return value;
        }

        internal static bool ContentEquals( string[] array1 , string[] array2 )
        {
            array2 = array2.ToList( ).Select( c => c.ToLower( ) ).ToArray( );
            foreach ( string value in array1 )
            {
                if ( !array2.Contains( value.ToLower( ) ) )
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool Contains( string[] array , string element )
        {
            foreach ( string value in array )
            {
                if ( value.Trim( ) == element.Trim( ) )
                    return true;
            }

            return false;
        }

        internal static bool Contains( object[] array , string element )
        {
            return Contains( ArrayUtil.ConvertAll<string>( array ) , element );
        }

        internal static string[] ConcatenateBothSide( string front , string[] array , string back )
        {
            List<string> prefixed = new List<string>( );

            foreach ( string name in array )
            {
                prefixed.Add( front + name.Trim( ) + back );
            }

            return prefixed.ToArray( );
        }

        internal static string[] SeperateWith( string front , string[] array , string back , bool ignoreLast )
        {
            string[] result = PrefixWith( front , array );
            return SuffixWith( back , result , true );
        }

        internal static string[] SeperateWith( string delimeter , string[] array )
        {
            return SuffixWith( delimeter , array , true );
        }

        internal static string[] SeperateWith( string delimeter , string[] array , bool ignoreLast )
        {
            return SuffixWith( delimeter , array , ignoreLast );
        }

        internal static string[] SuffixWith( string suffix , string[] array , bool ignoreLast )
        {
            List<string> suffixed = new List<string>( );
            int count = 0;

            foreach ( string name in array )
            {
                if ( ignoreLast && array.Length == ( count + 1 ) )
                {
                    suffixed.Add( name.Trim( ) );
                    continue;
                }

                suffixed.Add( name.Trim( ) + suffix.Trim( ) );
                ++count;
            }

            return suffixed.ToArray( );
        }

        internal static string[] Trim( string[] array )
        {
            List<string> prefixed = new List<string>( );

            foreach ( string name in array )
            {
                prefixed.Add( name.Trim( ) );
            }

            return prefixed.ToArray( );
        }

        internal static T[] RemoveFirstElement<T>( T[] array )
        {
            int count = 0;
            List<T> newArray = new List<T>( );

            Array.ForEach( array , delegate( T t )
            {
                if ( count >= 1 )
                {
                    newArray.Add( t );
                }

                ++count;
            } );

            return newArray.ToArray( );
        }

        internal static T[] AsArray<T>( object value )
        {
            return new T[] { ( T ) value };
        }

        internal static bool Matches( string[] array1 , string[] array2 )
        {
            Array.Sort( array1 );
            Array.Sort( array2 );

            for ( int i = 0; i < array1.Length; i++ )
            {
                if ( array1[i] != array2[i] )
                    return false;
            }

            return true;
        }
    }
}
