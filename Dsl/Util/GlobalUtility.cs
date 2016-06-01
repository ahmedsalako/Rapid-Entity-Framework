using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace consist.RapidEntity.Util
{
    public static class GlobalUtility
    {
        internal static bool IsNull( this object value )
        {
            return ( null == value );
        }

        internal static bool IsEquals( this string value , string secondValue )
        {
            return ( value.ToLower( ) == secondValue.ToLower( ) );
        }

        internal static IEnumerable<string> LoadPrimitiveTypes( )
        {
            foreach ( Type type in LoadTypes( ) )
            {
                if ( type == typeof( byte[] ) )
                    yield return string.Format( "byte[]" );
                else
                    yield return type.Name;
            }
        }

        internal static string MakeUnique( this string value , int length )
        {
            value = value + Guid.NewGuid( ).ToString( ).Replace( "-" , string.Empty ).Substring( 0 , length );

            if ( value.Length > 30 )
                return value.Substring( 0 , 29 );

            return value;
        }

        internal static Type GetPrimitiveType( string type )
        {
            if ( type.IsNullOrEmpty( ) )
                return typeof( string );

            if ( type.Contains( "Nullable" ) )
            {
                type = type.Replace( "Nullable<" , string.Empty );
                type = type.Replace( ">" , string.Empty );
            }

            type = type.Replace( "System." , string.Empty );
            Type clrType = LoadTypes( ).Where( t => t.Name.ToLower( ).Equals( type.ToLower( ) ) )
                                       .FirstOrDefault( );

            if ( clrType.IsNull( ) )
            {
                if ( type == "long" )
                    return typeof( Int64 );
                else if ( type == "float" )
                    return typeof( Double );
            }

            return clrType;
        }

        internal static IEnumerable<Type> LoadTypes( )
        {
            yield return typeof( Int32 );
            yield return typeof( Double );
            yield return typeof( Decimal );
            yield return typeof( String );
            yield return typeof( long );
            yield return typeof( DateTime );
            yield return typeof( DateTimeOffset );
            yield return typeof( TimeSpan );
            yield return typeof( float );
            yield return typeof( Boolean );
            yield return typeof( Single );
            yield return typeof( SByte );
            yield return typeof( Guid );
            yield return typeof( Char );
            yield return typeof( Byte );
            yield return typeof( Int64 );
            yield return typeof( Int16 );
            yield return typeof( byte[] );
            yield return typeof( Enum );
            yield return typeof( Object );
        }
    }
}
