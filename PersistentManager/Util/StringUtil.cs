using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace PersistentManager.Util
{
    internal static class StringUtil
    {
        internal static string Trim( string value )
        {
            return value.Trim( );
        }

        internal static string[] Split( string value , string split )
        {
            return value.Split( new string[] { split } , StringSplitOptions.RemoveEmptyEntries );
        }

        internal static bool IsAliased( this string value )
        {
            if( value.IsNullOrEmpty())
                return false;

            return value.Contains( "." );
        }

        internal static string EraseAlias( string value )
        {
            if ( value.IsNullOrEmpty( ) )
                return value;

            if ( value.Contains( "." ) )
                return value.Split( '.' )[1];

            return StringUtil.Split( value , "." )[0];
        }

        internal static bool AreEquals(string one, string two)
        {
            return one.ToLower() == two.ToLower();
        }

        internal static bool Contains(string text, string search)
        {
            if (text.IsNull() || search.IsNull())
                return false;

            return text.ToLower().Trim().Contains(search.ToLower().Trim());
        }

        internal static string StripQuotes( this string value )
        {
            return value.Replace( "\"" , "" );
        }

        internal static string GetAlias( string value )
        {
            if ( value.Contains( "." ) )
            {
                return value.Split( '.' )[0];
            }

            return string.Empty;
        }

        internal static string ToString<T>( T[] array , Quote quote )
        {
            StringBuilder elements = new StringBuilder( );

            foreach ( int index in array.GetIndices() )
            {
                if ( index != 0 )
                {
                    Generate<T>( array , "," , quote , elements , index );
                }
                else
                {
                    Generate<T>( array , "" , quote , elements , index );
                }
            }

            return elements.ToString( );
        }

        private static void Generate<T>( T[] array , string separator , Quote quote , StringBuilder elements , int index )
        {
            if ( quote == Quote.Double )
            {
                elements.AppendFormat( "{0}\"{1}\"" , separator , array[index] );
            }
            else if ( quote == Quote.Single )
            {
                elements.AppendFormat( "{0}'{1}'" , separator , array[index] );
            }
            else
            {
                elements.AppendFormat( "{0} {1}" , separator , array[index] );
            }
        }

        internal static string RemoveFirstElement( string text , string seperator )
        {
            string[] array = ArrayUtil.SuffixWith( "." , ArrayUtil.RemoveFirstElement<string>( Split( text , seperator ) ) , true );

            return ArrayUtil.ToString( array );
        }

        internal static string RemoveElements( string text , string seperator , int elements )
        {
            string current = text;
            for ( int i = 0 ; i < elements ; i++ )
            {
                current = RemoveFirstElement( current , seperator );
            }

            return current;
        }

        internal static string ToString( StringCollection collection )
        {
            StringBuilder builder = new StringBuilder( );
            foreach ( var current in collection )
            {
                builder.Append( current );
            }

            return builder.ToString( );
        }

        internal static string LowerFirstLetter( string value )
        {
            string temp = string.Empty;

            for ( int i = 0 ; i < value.Length ; i++ )
            {
                if ( i == 0 )
                    temp += char.ToLower( value[i] );
                else
                    temp += value[i];
            }

            return temp;
        }

        internal static string CapitaliseFirstLetter( string value )
        {
            string temp = string.Empty;

            for ( int i = 0 ; i < value.Length ; i++ )
            {
                if ( i == 0 )
                    temp += char.ToUpper( value[i] );
                else
                    temp += value[i];
            }

            return temp;
        }
    }
}
