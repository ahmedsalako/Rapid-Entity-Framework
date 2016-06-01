using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Ghosting;
using PersistentManager.Query.Sql;
using System.Collections;
using System.Reflection;

namespace PersistentManager.Query
{
    public class From : Keyword
    {
        internal From( PathExpressionFactory Path )
        {
            this.Path = Path;
        }

        public AS As( string alias )
        {
            Path.Main.CanonicalAlias = alias;

            return new AS( Path );
        }

        public AS<TEntity> As<TEntity>( TEntity t )
        {
            return FromEntity( t );
        }

        internal AS AsInternal( object entity )
        {
            return As( GetIdentifier( entity ) );
        }

        public From( Type entity )
        {
            Path = new PathExpressionFactory( entity , (Keyword)this );
        }

        internal static AS<TEntity> FromEntity<TEntity>( TEntity entity )
        {
            From from = new From( typeof( TEntity ) );
            from.Path.QueryableItem = entity;

            return new AS<TEntity>( from.AsInternal( entity ).Path );
        }
    }
}
