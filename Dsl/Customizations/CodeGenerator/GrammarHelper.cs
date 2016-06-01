using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace consist.RapidEntity.Customizations.CodeGenerator
{
    public static class GrammarHelper
    {
        static Regex clearRegex = new Regex( @"\s+|_|-|\." , RegexOptions.Compiled );
        static Regex clearIdRegex = new Regex( @"(_ID|_id|_Id|\.ID|\.id|\.Id|ID|Id)" , RegexOptions.Compiled );

        private static string Append( string word , Letters letters )
        {
            return string.Concat( word , Enum.GetName( typeof( Letters ) , letters ) );
        }

        public static string ClearName( string name )
        {
            return clearRegex.Replace( name , "" );
        }

        public static string CamelCase( string name )
        {
            string output = ClearName( name );

            output = MakeCasing( output );

            return char.ToLower( output[0] ) + output.Substring( 1 );
        }

        public static string PascalCase( string name )
        {
            string output = ClearName( name );

            output = MakeCasing( output );

            return char.ToUpper( output[0] ) + output.Substring( 1 );
        }

        private static string MakeCasing( string output )
        {
            if ( string.Equals( output , output.ToUpper( ) ) )
            {
                return output.ToLower( );
            }

            return output;
        }

        public static string MakePlural( string noun )
        {
            noun = MakeSingle( noun );

            Regex plural1 = new Regex( "(?<keep>[^aeiou])y$" );
            Regex plural2 = new Regex( "(?<keep>[aeiou]y)$" );
            Regex plural3 = new Regex( "(?<keep>[sxzh])$" );
            Regex plural4 = new Regex( "(?<keep>[^sxzhy])$" );

            if ( plural1.IsMatch( noun ) )
                return plural1.Replace( noun , "${keep}ies" );
            else if ( plural2.IsMatch( noun ) )
                return plural2.Replace( noun , "${keep}s" );
            else if ( plural3.IsMatch( noun ) )
                return plural3.Replace( noun , "${keep}es" );
            else if ( plural4.IsMatch( noun ) )
                return plural4.Replace( noun , "${keep}s" );

            return noun;
        }

        public static string MakeSingle( string noun )
        {
            Regex plural1 = new Regex( "(?<keep>[^aeiou])ies$" );
            Regex plural2 = new Regex( "(?<keep>[aeiou]y)s$" );
            Regex plural3 = new Regex( "(?<keep>[sxzh])es$" );
            Regex plural4 = new Regex( "(?<keep>[^sxzhyu])s$" );

            if ( plural1.IsMatch( noun ) )
                return plural1.Replace( noun , "${keep}y" );
            else if ( plural2.IsMatch( noun ) )
                return plural2.Replace( noun , "${keep}" );
            else if ( plural3.IsMatch( noun ) )
                return plural3.Replace( noun , "${keep}" );
            else if ( plural4.IsMatch( noun ) )
                return plural4.Replace( noun , "${keep}" );

            return noun;
        }
    }

    public enum Letters
    {
        es ,
        y ,
        s ,
        ies ,
        o ,
        f ,
        fe ,
        ves ,
    }
}
