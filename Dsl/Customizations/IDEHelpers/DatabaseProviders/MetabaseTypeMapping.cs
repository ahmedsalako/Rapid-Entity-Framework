using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public class MetabaseTypeMapping
    {
        public string DatabaseType { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public string CLRType { get; set; }

        public string GetDatabaseType()
        {           
            if ( Precision > 0 )
            {
                if ( Scale.IsNull( ) )
                {
                    return string.Format( "{0}({1})" , DatabaseType , Precision ).Trim( );
                }
                else
                {
                    return string.Format( "{0}({1}{2})" , DatabaseType , Precision ,
                              ( Scale > -1 ) ? "," + Scale.ToString( ) : string.Empty ).Trim( );
                }
            }

            return DatabaseType;
        }
    }
}
