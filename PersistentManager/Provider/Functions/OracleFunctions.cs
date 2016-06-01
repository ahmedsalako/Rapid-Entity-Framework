using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Provider.Functions
{
    internal class OracleFunctions : ProviderFunctions
    {
        protected override string Month(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'MM'))", leftOperand);
        }

        protected override string Year(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'YYYY'))", leftOperand);
        }

        protected override string Day(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'DD'))", leftOperand);
        }

        protected override string Hour(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'HH'))", leftOperand);
        }

        protected override string Seconds(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'SS'))", leftOperand);
        }

        protected override string Minute(object leftOperand)
        {
            return string.Format("TO_NUMBER(TO_CHAR({0}, 'MI'))", leftOperand);
        }

        protected override string Length( object leftOperand )
        {
            return string.Format( "LENGTH({0})" , leftOperand );
        }
    }
}
