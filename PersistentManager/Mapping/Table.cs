using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Table : TableSchema
    {
        public Table( string name ): base( name )
        {

        }

        public Table()
            : base()
        {
        }
    }
}
