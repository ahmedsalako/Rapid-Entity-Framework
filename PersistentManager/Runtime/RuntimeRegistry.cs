using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Runtime
{
    public class RuntimeRegistry
    {
        private RuntimeRegistry()
        {
            Registrations = new List<Type>();
            IsMainConnection = false;
        }

        internal static RuntimeRegistry GetRuntimeRegistry()
        {
            return new RuntimeRegistry();
        }

        internal string RegistryName { get; set; }
        internal ProviderDialect ProviderDialect { get; set; }
        internal string ConnectionString { get; set; }
        internal bool IsMainConnection { get; set; }
        internal List<Type> Registrations { get; set; }
    }
}
