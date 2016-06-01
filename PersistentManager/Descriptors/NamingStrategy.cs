using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace PersistentManager.Descriptors
{
    internal class NamingStrategy
    {
        internal static string DecorateName( string name )
        {
            string naming = SessionRuntime.GetInstance( ).DataBaseProvider.GetNamingStrategyString( );
            return string.Format( naming , name );
        }

        internal static string DecorateName( string alias , string name )
        {
            return DecorateName( alias ).Dot( DecorateName( name ) );
        }

        internal static string[] DecorateName( string alias , string[] names )
        {
            List<string> decoratedNames = new List<string>( );

            foreach ( string name in names )
            {
                decoratedNames.Add( DecorateName( alias ).Dot( DecorateName( name ) ) );
            }

            return decoratedNames.ToArray( );
        }

        internal static string DecorateNameAndAlias( string name )
        {
            if ( !name.IsNullOrEmpty( ) && name.Contains( "." ) )
            {
                string[] array = name.Split( '.' );

                return DecorateName( array[0] , array[1] );
            }

            return DecorateName( name );
        }

        private static string Strip( string fullyQualifiedName )
        {
            return string.Empty;
        }
    }
}
