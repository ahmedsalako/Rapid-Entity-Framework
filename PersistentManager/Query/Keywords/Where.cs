using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Ghosting;

namespace PersistentManager.Query
{
    public class Where : Where<object>
    {
        internal Where(PathExpressionFactory Path, AS As) : base( Path , As )
        {

        }
    }

    public class Where<TEntity> : FunctionalKeywords<TEntity, Where<TEntity>>
    {
        internal Where( PathExpressionFactory Path , AS As ) 
        {
            this.Path = Path;
            this.Identifier = As;
            this.Current = this;
        }

        public AND And( object name , Condition condition , object value )
        {
            return Identifier.And( new[] { name } , condition , new[] { value } );
        }

        public OR Or( object name , Condition condition , object value )
        {
            return Identifier.Or( new[] { name } , condition , new[] { value } );
        }

        public virtual OR Or( object[] names , Condition condition , object[] values )
        {
            return Identifier.Or( GetParameterNames( names ) , condition , values );
        }

        public virtual AND And( object[] names , Condition condition , object[] values )
        {
            return Identifier.And( GetParameterNames( names ) , condition , values );
        }

        public AND And( object name , Condition condition )
        {
            return Identifier.And( name , condition );
        }

        public OR Or( object name , Condition condition )
        {
            return Identifier.Or( name , condition );
        }

        public OrderBy OrderBy( params object[] parameters )
        {
            return Identifier.OrderBy( parameters );
        }

        public OrderBy OrderByDescending( params object[] parameters )
        {
            return Identifier.OrderByDescending( parameters );
        }

        public GroupBy GroupBy( params object[] properties )
        {
            return Identifier.GroupBy( properties );
        }
    }
}
