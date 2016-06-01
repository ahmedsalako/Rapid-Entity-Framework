using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace PersistentManager
{
    internal class ScopeContext
    {
        internal static void SetData(string name, object data)
        {
            IConfiguration config = ConfigurationBase.GetCurrentConfiguration();

            //CallContext.SetData(name, data);

            if ( !ItemContains( name ) )
            {

                if (config.IsNotNull() && config is ConfigurationFactory)
                {
                    CallContext.SetData(name, data);
                }
                else if (config.IsNotNull() && config is WebConfigurationFactory)
                {
                    HttpContext.Current.Items.Add(name, data);
                }
                else
                {
                    throw new Exception("no valid configuration is set. Set either ConfigurationFactory or WebConfigurationFactory");
                }
            }
        }

        internal static void RemoveData( string name )
        {
            IConfiguration config = ConfigurationBase.GetCurrentConfiguration();

           // CallContext.FreeNamedDataSlot(name);
            if ( ItemContains( name ) )
            {
                if (config.IsNotNull() && config is ConfigurationFactory)
                {
                    CallContext.FreeNamedDataSlot(name);
                }
                else if (config.IsNotNull() && config is WebConfigurationFactory)
                {
                    HttpContext.Current.Items.Remove(name);
                }
            }
        }

        internal static object GetData(string name)
        {
            return GetData<object>( name );
        }

        internal static T GetData<T>( string name )
        {

            //return (T)CallContext.GetData(name);

            if ( ItemContains( name ) )
            {
                IConfiguration config = ConfigurationBase.GetCurrentConfiguration();

                if (config.IsNotNull() && config is ConfigurationFactory)
                {
                    return (T)CallContext.GetData(name);
                }
                else if (config.IsNotNull() && config is WebConfigurationFactory)
                {
                    return (T)HttpContext.Current.Items[name];
                }
            }

            return default(T);
        }

        internal static bool ItemContains( string name )
        {
            IConfiguration config = ConfigurationBase.GetCurrentConfiguration();

            if (config.IsNotNull() && config is ConfigurationFactory)
            {
                return CallContext.GetData(name) != null;
            }
            else if (config.IsNotNull() && config is WebConfigurationFactory)
            {
                return HttpContext.Current.Items.Contains(name);
            }

            return false;
        }
    }
}
