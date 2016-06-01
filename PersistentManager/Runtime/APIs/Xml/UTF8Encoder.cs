using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Runtime.APIs.Xml
{
    internal static class UTF8Encoder
    {
        public static string ByteArrayToString( Byte[] bytes )
        {
            UTF8Encoding encoding = new UTF8Encoding( );
            return encoding.GetString( bytes ).Trim( );
        }

        public static Byte[] StringToByeteArray( String value )
        {
            UTF8Encoding encoding = new UTF8Encoding( );
            return encoding.GetBytes( value );
        }
    }
}
