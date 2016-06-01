using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false)]
    public class OneToOne : Attribute, IOneBasedRelation, IRelationalAttribute
    {
        [XmlIgnore]
        public Type Type { get; set; }

        [XmlElement("relation-column")]
        public string RelationColumn { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("is-imported")]
        public bool IsImported { get; set; }

        public OneToOne(Type type)
        {
            Type = type;
        }

        [XmlElement("cascade")]
        public Cascade Cascade { get; set; }

        public OneToOne()
        {

        }

        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("is-split")]
        public bool IsSplit { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }

        [XmlElement("load-type")]
        public LoadType LoadType { get; set; }

        internal PropertyInfo Property { get; set; }

        [XmlArray]
        public RelationJoinAttribute[] RelationJoins { get; set; }
    }
}
