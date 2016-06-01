using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToOne : Attribute, IOneBasedRelation, IRelationalAttribute
    {
        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("relation-column")]
        public string RelationColumn { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlElement("cascade")]
        public Cascade Cascade { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }

        [XmlElement("load-type")]
        public LoadType LoadType { get; set; }

        [XmlIgnore]
        public Type Type { get; set; }

        [XmlIgnore]
        internal PropertyInfo Property { get; set; }

        [XmlArray]
        public RelationJoinAttribute[] RelationJoins { get; set; }

        public ManyToOne(Type type)
        {
            Type = type;
        }

        public ManyToOne()
        {
        }
    }
}
