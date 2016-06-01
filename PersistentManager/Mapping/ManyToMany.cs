using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToMany : Attribute, IRelationalAttribute , IManyBasedRelation
    {
        [XmlIgnore]
        public Type Type { get; set; }

        [XmlElement("join-table")]
        public string JoinTable { get; set; }

        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("owner-column")]
        public string OwnerColumn { get; set; }

        [XmlElement("cascade")]
        public Cascade Cascade { get; set; }

        [XmlElement("right-key")]
        public string RightKey {get; set; }

        [XmlElement("left-key")]
        public string LeftKey { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlElement("load-type")]
        public LoadType LoadType { get; set; }

        [XmlIgnore]
        internal PropertyInfo Property { get; set; }

        [XmlArray]
        public RelationJoinAttribute[] RelationJoins { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }
    }
}
