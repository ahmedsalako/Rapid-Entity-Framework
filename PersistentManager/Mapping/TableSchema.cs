using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public class TableSchema : Attribute
    {
        [XmlElement("table-name")]
        public virtual string Name { get; set; }

        [XmlElement("relation-column")]
        public virtual string RelationColumn { get; set; }

        [XmlElement("join-column")]
        public virtual string JoinColumn { get; set; }

        [XmlElement("class")]
        public virtual string ClassName { get; set; }

        public TableSchema(string name)
        {
            Name = name;
        }

        public TableSchema()
        {

        }
    }
}
