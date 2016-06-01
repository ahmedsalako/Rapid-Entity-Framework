using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Descriptors;

namespace PersistentManager.Query
{
    public class AliasBuilder
    {
        internal string ALIAS = string.Empty;
        int ALIASCount = 0;
        const string ALIASJOIN = ".";

        internal AliasBuilder( string alias )
        {
            ALIAS = alias;
        }

        internal static string Build( string ALIAS , string column )
        {
            return string.Concat( ALIAS + ALIASJOIN , column );
        }

        internal static string[] Build2( string ALIAS , params string[] column )
        {
            List<string> list = new List<string>( );
            column.ToList( ).ForEach( c => list.Add( Build( ALIAS , c ) ) );

            return list.ToArray( );
        }

        internal string GetNextAlias( )
        {
            return string.Concat( ALIAS , ALIASCount++ );
        }
    }
}
