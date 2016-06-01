using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping
{
    [AttributeUsage( AttributeTargets.Class , AllowMultiple = true )]
    public class EmbeddedEntity : Attribute
    {
        public string Name { get; set; }
        public string RelationColumn { get; set; }
        public string JoinColumn { get; set; }
        internal Type Type { get; set; }
    }
}
