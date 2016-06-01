using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace consist.RapidEntity.CodeGenerator
{
    public class TableSchema
    {
        [XmlElement("table-name")]
        public virtual string Name { get; set; }

        [XmlElement("relation-column")]
        public virtual string RelationColumn { get; set; }

        [XmlElement("join-column")]
        public virtual string JoinColumn { get; set; }

        [XmlElement("class")]
        public virtual string ClassName { get; set; }
    }
}
