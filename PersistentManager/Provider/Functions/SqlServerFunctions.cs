using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Provider.Functions
{
    internal class SqlServerFunctions : ProviderFunctions
    {
        private DatabaseProvider Provider { get; set; }

        internal SqlServerFunctions( DatabaseProvider provider )
        {
            this.Provider = provider;
        }

        protected override string StringConcatenation( object leftOperand , object rightOperand )
        {
            return MakeBasicArithmetic( leftOperand , "+" , rightOperand ) ;
        }

        protected override string Trim(object leftOperand)
        {
            return TrimLeft( TrimRight( leftOperand ) );
        }

        protected override string Month(object leftOperand)
        {
            return string.Format("DATEPART(MONTH, {0})", leftOperand);
        }

        protected override string Year(object leftOperand)
        {
            return string.Format("DATEPART(YEAR, {0})", leftOperand);
        }

        protected override string Day(object leftOperand)
        {
            return string.Format("DATEPART(DAY, {0})", leftOperand);
        }

        protected override string Hour(object leftOperand)
        {
            return string.Format("DATEPART(HOUR, {0})", leftOperand);
        }

        protected override string Minute(object leftOperand)
        {
            return string.Format("DATEPART(MINUTE, {0})", leftOperand);
        }

        protected override string Seconds(object leftOperand)
        {
            return string.Format("DATEPART(SECOND, {0})", leftOperand);
        }
    }
}
