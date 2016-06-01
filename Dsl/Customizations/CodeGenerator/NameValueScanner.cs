using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace consist.RapidEntity.Customizations.CodeGenerator
{
    public static class NameValueScanner
    {
        public static string[] SplitSpecial( string value , char splitValue , bool stripQuotes )
        {
            string[] values = ( stripQuotes ) ? value.Replace( "\"" , string.Empty ).Split( splitValue )
                : value.Split( splitValue );

            return values;
        }

        public static string GetTypeName( string value )
        {
            value = value.Replace( "typeof(" , string.Empty );
            return value.Replace( ")" , string.Empty ).Trim( );
        }

        private static string[] GetNameValue( string current )
        {
            string currentName = string.Empty;
            string currentValue = string.Empty;

            if ( current.Contains( "=" ) )
            {
                return current.Split( '=' );
            }

            return null;
        }

        private static bool HasNameValue( string current )
        {
            return !GetNameValue( current ).IsNull( );
        }

        private static string GetValue( string current , string name )
        {
            if ( HasNameValue( current ) )
            {
                string[] values = GetNameValue( current );
                string value = values[0].Trim( );

                if ( value == name )
                {
                    return values[1].Trim( );
                }
            }
            return null;
        }

        public static string GetValueByName( string name , string[] values , int index )
        {
            try
            {
                foreach ( string current in values )
                {
                    string value = GetValue( current , name );

                    if ( !value.IsNull( ) )
                        return value.Trim( );
                }

                return values[index - 1].Trim( );
            }
            catch ( Exception x )
            {

            }

            return null;
        }
    }
}
