using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Descriptors
{
    internal class ProviderDescriptor
    {
        internal string ProviderName { get; set; }
        internal string AssemblyName { get; set; }
        internal Guid UniqueId { get; set; }
        internal string Type { get; set; }
        internal string ConnectionQualifiedName { get; set; }
        internal string CommandQualifiedName { get; set; }
        internal string ParameterQualifiedName { get; set; }
    }
}
