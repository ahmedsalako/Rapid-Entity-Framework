using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.QueryEngine.Database;
using PersistentManager.Provider;

namespace PersistentManager.Query.QueryEngine
{
    internal abstract class DataStoreQuery
    {
        internal protected ContextData ContextData {get; set;}
        internal protected IDatabaseProvider Provider { get; set; }
        internal protected IList<ActionBase> Strategies {get; set;}

        internal void Initialise( ContextData contextData , IDatabaseProvider provider )
        {
            this.Provider = provider;
            this.ContextData  = contextData;            
            this.Strategies = new List<ActionBase>();
        }

        internal static DataStoreQuery GetInstance<T>( ContextData ctx , IDatabaseProvider provider ) where T : DataStoreQuery
        {
            if(typeof(T) == typeof(RDBMSDataStore))
                return new RDBMSDataStore( ctx , provider );

            return null;
        }

        internal abstract ContextData ExecuteSelect( bool distinct );
        internal abstract ContextData ExecuteUpdate();
        internal abstract ContextData ExecuteCreate();
        internal abstract ContextData ExecuteDelete();     
    } 
}
