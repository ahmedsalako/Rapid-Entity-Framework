#region Using directives

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

#endregion

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle(@"")]
[assembly: AssemblyDescription(@"")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(@"consist")]
[assembly: AssemblyProduct(@"RapidEntity")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: System.Resources.NeutralResourcesLanguage("en")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion(@"1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: ReliabilityContract(Consistency.MayCorruptProcess, Cer.None)]

//
// Make the Dsl project internally visible to the DslPackage assembly
//
[assembly: InternalsVisibleTo(@"consist.RapidEntity.DslPackage, PublicKey=00240000048000009400000006020000002400005253413100040000010001002B4243836874FD2DFC630618298D73B6FE50C6888548FDFC226198CE28BE2AF0019EB91AE5B4B487E92913CD957BA8876A9DEFEC2E9EAA9AD58EFE91F65158D08A65F17F8F4023BE1C549BDB23DF5796D4946450A7F226221F2A92125241B152E1B878E6DDB721DD279A22CE563B166ABCFCA3FF893729AB38EE66B7C574C398")]