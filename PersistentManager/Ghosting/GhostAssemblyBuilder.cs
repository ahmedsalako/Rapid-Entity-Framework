using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace PersistentManager.Ghosting
{
    internal class GhostAssemblyBuilder
    {
        internal const string VIRTUAL_ASSEMBLY_NAME = "ConsistentSoft.Rapid.Entity.GhostAssembly.Persistentce";
        internal const string VIRTUAL_ASSEMBLY_TRANSACTION_NAME = "ConsistentSoft.Rapid.Entity.GhostAssemblyTransaction.Persistentce"; 
        const string version = "1.0.0.0";

        static AssemblyName assemblyName;
        static ModuleBuilder moduleBuilder;

        static AssemblyBuilder assembly;
        static AppDomain curAppDomain;
        static IDictionary<string, Type> GhostBag;

        //Static initializer
        // Get the application domain for the current thread.
        // Create new assembly (Probably a Ghost One) within the current AppDomain.
        static GhostAssemblyBuilder()
        {
            assemblyName = new AssemblyName(VIRTUAL_ASSEMBLY_NAME);
            assemblyName.Version = new Version(version);            
            curAppDomain = Thread.GetDomain();            
            assembly = curAppDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assembly.DefineDynamicModule(VIRTUAL_ASSEMBLY_NAME);
            GhostBag = new Dictionary<string, Type>();
        }

        internal static IDictionary<string, Type> GhostTypes
        {
            get { return GhostBag; }
        }

        public static AssemblyBuilder Assembly
        {
            get { return GhostAssemblyBuilder.assembly; }
            set { GhostAssemblyBuilder.assembly = value; }
        }

        internal static ModuleBuilder ModuleBuilder
        {
            get { return moduleBuilder; }
        }

        internal static AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        internal static AssemblyBuilder AssemblyBuilder
        {
            get { return assembly; }
        }

        internal static AppDomain AppDomain
        {
            get { return curAppDomain; }
        }

        internal static void AddGhost(string key, Type livingType)
        {
            if(!GhostBag.ContainsKey(key))
                GhostBag.Add(key, livingType);
        }

        internal static bool HasGhost( string key )
        {
            return GhostBag.ContainsKey( key );
        }

        internal static Type GetGhostType(string key)
        {
            return GhostBag[ key ].GetType();
        }
    }
}
