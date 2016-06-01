using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.QueryEngine.Database;
using PersistentManager.Provider;

namespace PersistentManager.Query.QueryEngine
{
    internal class RDBMSDataStore : DataStoreQuery
    {
        internal RDBMSDataStore( ) { }

        internal RDBMSDataStore( ContextData c , IDatabaseProvider provider )
        {
            base.Initialise( c , provider );
        }

        internal override ContextData ExecuteSelect( bool distinct )
        {
            Strategies.Add( new AliasAction( ) );

            Strategies.Add( new FromAction( new JoinEmbeddedAction( ContextData , Provider ) ) );
            Strategies.Add( new FromClauseSubquery( ) );
            Strategies.Add( new ConditionAction( ) );
            Strategies.Add( new GroupByAction( ) );
            Strategies.Add( new OrderByAction( ) );            
            Strategies.Add( new SelectAction( distinct ) );
            Strategies.Add( new SelectFunctionAction( ) );

            Strategies.ToList( ).ForEach( s => ContextData = s.Execute( ContextData , Provider ) );

            return ContextData;
        }

        internal override ContextData ExecuteUpdate( )
        {
            Strategies.Add( new ExecuteUpdateAction( ) );
            Strategies.ToList( ).ForEach( s => ContextData = s.Execute( ContextData , Provider ) );

            return ContextData;
        }

        internal override ContextData ExecuteCreate( )
        {
            Strategies.Add( new ExecuteCreateAction( ) );
            Strategies.ToList( ).ForEach( s => ContextData = s.Execute( ContextData , Provider ) );

            return ContextData;
        }

        internal override ContextData ExecuteDelete( )
        {
            Strategies.Add( new ExecuteDeleteAction( ) );
            Strategies.ToList( ).ForEach( s => ContextData = s.Execute( ContextData , Provider ) );

            return ContextData;
        }
    }
}
