using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class ManyToOneXml
    {
        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("relation-column")]
        public string RelationColumn { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlElement("cascade")]
        public string Cascade { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }

        [XmlElement("load-type")]
        public string LoadType { get; set; }

        [XmlArray]
        public RelationJoin[] RelationJoins { get; set; }
    }
}
