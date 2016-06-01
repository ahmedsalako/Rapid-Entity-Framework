using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class ManyToManyXml
    {
        [XmlElement("join-table")]
        public string JoinTable { get; set; }

        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("owner-column")]
        public string OwnerColumn { get; set; }

        [XmlElement("cascade")]
        public string Cascade { get; set; }

        [XmlElement("right-key")]
        public string RightKey { get; set; }

        [XmlElement("left-key")]
        public string LeftKey { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlArray]
        public RelationJoin[] RelationJoins { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }

        [XmlElement("load-type")]
        public string LoadType { get; set; }
    }
}
