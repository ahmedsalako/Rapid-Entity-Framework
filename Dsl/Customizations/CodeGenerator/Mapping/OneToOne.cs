using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class OneToOneXml
    {
        [XmlElement("relation-column")]
        public string RelationColumn { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("is-imported")]
        public bool IsImported { get; set; }

        [XmlElement("join-column")]
        public string JoinColumn { get; set; }

        [XmlElement("is-split")]
        public bool IsSplit { get; set; }

        [XmlAttribute("relation-class")]
        public string RelationClass { get; set; }

        [XmlElement("load-type")]
        public string LoadType { get; set; }

        [XmlArray]
        public RelationJoin[] RelationJoins { get; set; }
    }
}
