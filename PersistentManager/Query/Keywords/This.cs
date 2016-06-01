using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using PersistentManager.Query.Keywords;
using PersistentManager.Ghosting;

namespace PersistentManager.Query
{
    internal abstract class This
    {
        internal Guid ScopeId { get; set; }

        internal Keyword Keyword { get; set; }

        internal This( )
        {
           AddScope( CreateScope( ) , this );
        }

        internal Guid CreateScope( )
        {
            return ScopeId = Guid.NewGuid( );
        }

        internal static This GetCurrentScopeObject( )
        {
            return GetCurrentScope( ).Value;
        }

        internal static Guid GetCurrentScopeId( )
        {
            return GetCurrentScope( ).Key;
        }

        internal static KeyValuePair<Guid , This> GetCurrentScope( )
        {
            if ( ScopeContext.GetData( Constant.QUERY_SCOPE ).IsNull( ) || GetScopes( ).IsEmpty( ) )
            {
                return new KeyValuePair<Guid , This>( );
            }

            return ( ( Stack<KeyValuePair<Guid , This>> )ScopeContext.GetData( Constant.QUERY_SCOPE ) ).Peek( );
        }

        internal static void AddScope( Guid scopeId , This _this )
        {
            KeyValuePair<Guid , This> scope = new KeyValuePair<Guid , This>( scopeId , _this );
            Stack<KeyValuePair<Guid , This>> scopes = GetScopes( );
            scopes.Push( scope );
        }

        internal static Stack<KeyValuePair<Guid , This>> GetScopes( )
        {
            if ( ScopeContext.GetData( Constant.QUERY_SCOPE ).IsNull( ) )
            {
                ScopeContext.SetData( Constant.QUERY_SCOPE , new Stack<KeyValuePair<Guid , This>>( ) );
            }

            return ( ( Stack<KeyValuePair<Guid , This>> )ScopeContext.GetData( Constant.QUERY_SCOPE ) );
        }

        internal static void RemoveCurrentScope( )
        {
            CallStack.ResetCurrentSessionCount( );
            if ( GetScopes( ).Count > 0 )
            {
                GetScopes( ).Pop( );
            }
        }

    }
}
