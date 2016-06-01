using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PersistentManager.Metadata
{
    public class DataType
    {
        public static object ConvertValue( Type target , object value )
        {
            if ( value.IsNull( ) )
                return value;

            if ( value.GetType( ) == typeof( DBNull ) )
            {
                return Null.Default( target );
            }

            if ( target == value.GetType( ) )
            {
                return value;
            }
            else if ( target.IsGenericType && target.GetGenericTypeDefinition( ) == typeof( Nullable<> ) )
            {
                return ConvertValue( Nullable.GetUnderlyingType( target ) , value );
            }
            else if ( target == typeof( Guid ) )
            {
                if ( ( value is byte[] ) )
                    return new Guid( ( byte[] )value );
                else
                {
                    return new Guid( value.ToString( ) );
                }
            }
            else if ( value is DateTime && target == typeof( DateTimeOffset ) )
                return new DateTimeOffset( ( DateTime )value );
            else if ( target == typeof( Int64 ) )
                return Convert.ToInt64( value );
            else if ( target == typeof( Int32 ) )
                return Convert.ToInt32( value );
            else if ( target == typeof( Int16 ) )
                return Convert.ToInt16( value );
            else if ( target == typeof( Decimal ) )
                return Convert.ToDecimal( value );
            else if ( target == typeof( Double ) )
                return Convert.ToDouble( value );
            else if ( target == typeof( Char ) )
                return Convert.ToChar( value );
            else if ( target == typeof( String ) )
                return Convert.ToString( value );
            else if ( target == typeof( Byte ) )
                return Convert.ToByte( value );
            else if ( target == typeof( Boolean ) )
                return Convert.ToBoolean( value );
            else if ( target == typeof( Object ) )
                return value;
            else if ( target == typeof( DateTime ) )
                return Convert.ToDateTime( value );
            else
                return value;
        }

        public static bool IsNumber( Type value )
        {
            return IsDouble( value ) ? true : IsLong( value ) ? true : false;
        }

        public static bool IsDouble( Type value )
        {
            Type type = value.GetType( );

            if ( type == typeof( double ) || type == typeof( float ) )
                return true;

            return false;
        }

        public static object GetDefaultValue( Type type )
        {
            if ( type == typeof( string ) )
                return null;
            else if ( type == typeof( Guid ) )
                return Guid.Empty;
            else if ( type == typeof( DateTime ) || type == typeof( DateTimeOffset ) )
                return DateTime.MinValue;

            return Activator.CreateInstance( type );
        }

        public static bool IsValueType( object value )
        {
            return ( value is ValueType );
        }

        public static bool IsValueType( Type type )
        {
            return ( type.IsValueType );
        }

        public static bool IsLong( Type value )
        {
            if ( value == typeof( Int64 ) || value == typeof( Int32 ) || value == typeof( Int16 ) )
                return true;

            return false;
        }

        public static bool IsPrimitive( Type type )
        {
            return ( type.IsPrimitive );
        }

        public static bool ValueEquals( object value , object value2 )
        {
            if ( IsNumber( value.GetType( ) ) )
            {
                long number = 0;
                long.TryParse( value.ToString( ) , out number );

                long number2 = 0;
                long.TryParse( value2.ToString( ) , out number2 );

                return ( number == number2 );
            }
            else if ( IsGuid( value.GetType( ) ) )
            {
                return ( value.ToString( ) == value2.ToString( ) );
            }

            return false;
        }

        public static bool IsGuid( Type value )
        {
            if ( value == typeof( Guid ) )
                return true;

            return false;
        }
    }
}
