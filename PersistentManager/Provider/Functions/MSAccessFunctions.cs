using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Provider.Functions
{
    internal class MSAccessFunctions : ProviderFunctions
    {
        protected override string StringConcatenation(object leftOperand, object rightOperand)
        {
            return string.Format("{0} & {1}", leftOperand, rightOperand);
        }

        protected override string Coalesce(object leftOperand, object rightOperand)
        {
            return string.Format("Nz({0},{1})", leftOperand, rightOperand);
        }

        protected override string UpperCase(object leftOperand)
        {
            return string.Format("UCase({0}", leftOperand);
        }

        protected override string LowerCase(object leftOperand)
        {
            return string.Format("LCase({0})", leftOperand);
        }

        protected override string Month(object leftOperand)
        {
            return string.Format("Month({0})", leftOperand);
        }

        protected override string Year(object leftOperand)
        {
            return string.Format("Year({0})", leftOperand);
        }

        protected override string Day(object leftOperand)
        {
            return string.Format("Day({0})", leftOperand);
        }

        protected override string Hour(object leftOperand)
        {
            return string.Format("Hour({0})", leftOperand);
        }

        protected override string Minute(object leftOperand)
        {
            return string.Format("Minute({0})", leftOperand);
        }

        protected override string Seconds(object leftOperand)
        {
            return string.Format("Second({0})", leftOperand);
        }
    }
}
