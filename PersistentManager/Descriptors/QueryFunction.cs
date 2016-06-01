using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query;

namespace PersistentManager.Descriptors
{
    internal class QueryFunction : ICloneable
    {
        internal FunctionCall Function { get; set; }
        internal Guid CriteriaJoin { get; set; }
        internal string Name { get; set; }        
        internal object Value { get; set; }

        internal QueryFunction( FunctionCall function , object value , Guid join )
        {
            CriteriaJoin = join;
            Function = function;
            Value = value;
        }

        internal QueryFunction( FunctionCall function , object value )
        {
            Function = function;
            Value = value;
        }

        public object Clone( )
        {
            return this.MemberwiseClone( );
        }

        public QueryFunction CloneSpecial( )
        {
            return Clone( ) as QueryFunction;
        }
    }
}
