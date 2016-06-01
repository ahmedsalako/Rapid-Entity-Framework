using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Provider.Functions
{
    internal class Db2Functions : ProviderFunctions
    {
        protected override string Month( object leftOperand )
        {
            return string.Format( "MONTH({0})" , leftOperand );
        }

        protected override string Year( object leftOperand )
        {
            return string.Format( "YEAR({0})" , leftOperand );
        }

        protected override string Day( object leftOperand )
        {
            return string.Format( "DAY({0})" , leftOperand );
        }

        protected override string Hour( object leftOperand )
        {
            return string.Format( "HOUR({0})" , leftOperand );
        }

        protected override string Seconds( object leftOperand )
        {
            return string.Format( "SECOND({0})" , leftOperand );
        }

        protected override string Minute( object leftOperand )
        {
            return string.Format( "MINUTE({0})" , leftOperand );
        }

        protected override string Length( object leftOperand )
        {
            return string.Format( "LENGTH({0})" , leftOperand );
        }

        protected override string TrimLeft( object leftOperand )
        {
            return string.Format( "LTRIM({0})" , leftOperand );
        }

        protected override string TrimRight( object leftOperand )
        {
            return string.Format( "RTRIM({0})" , leftOperand );
        }

        protected override string Trim( object leftOperand )
        {
            return TrimLeft( TrimRight( leftOperand ) );
        }
    }
}
