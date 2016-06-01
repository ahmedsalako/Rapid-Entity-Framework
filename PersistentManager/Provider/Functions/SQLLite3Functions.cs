using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Provider.Functions
{
    internal class SQLLite3Functions : ProviderFunctions
    {
        protected override string Coalesce( object leftOperand , object rightOperand )
        {
            return string.Format( "coalesce({0},{1})" , leftOperand , rightOperand );
        }

        protected override string Month( object leftOperand )
        {
            return string.Format( "strftime(\"%m\", {0})" , leftOperand );
        }

        protected override string Year( object leftOperand )
        {
            return string.Format( "strftime(\"%Y\", {0})" , leftOperand );
        }

        protected override string Day( object leftOperand )
        {
            return string.Format( "strftime(\"%d\", {0})" , leftOperand );
        }

        protected override string Hour( object leftOperand )
        {
            return string.Format( "strftime(\"%H\", {0})" , leftOperand );
        }

        protected override string Seconds( object leftOperand )
        {
            return string.Format( "strftime(\"%S\", {0})" , leftOperand );
        }

        protected override string Minute( object leftOperand )
        {
            return string.Format( "strftime(\"%M\", {0})" , leftOperand );
        }

        protected override string Length( object leftOperand )
        {
            return string.Format( "length({0})" , leftOperand );
        }

        protected override string TrimLeft( object leftOperand )
        {
            return string.Format( "ltrim({0})" , leftOperand );
        }

        protected override string TrimRight( object leftOperand )
        {
            return string.Format( "rtrim({0})" , leftOperand );
        }

        protected override string Trim( object leftOperand )
        {
            return TrimLeft( TrimRight( leftOperand ) );
        }
    }
}
