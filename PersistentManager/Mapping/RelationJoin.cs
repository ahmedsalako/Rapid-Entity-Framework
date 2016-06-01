using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=true, Inherited=true)]
    public class RelationJoinAttribute : Attribute
    {
        public string RelationColumn { get; set; }
        public string JoinColumn { get; set; }
        public string OwnerColumn { get; set; }
        public Type ColumnType { get; set; }
        public string LeftKey { get; set; }
        public string RightKey { get; set; }

        [XmlIgnore]
        internal PropertyInfo Property { get; set; }
    }
}
