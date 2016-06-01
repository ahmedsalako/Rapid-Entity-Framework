using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Descriptors
{
    internal class MethodMetadata
    {
        internal string MethodName { get; set; }
        internal QueryType QueryType { get; set; }

        internal MethodMetadata( string methodName , QueryType queryType )
        {
            MethodName = methodName;
            QueryType = queryType;
        }
    }
}
