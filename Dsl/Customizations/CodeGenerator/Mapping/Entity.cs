using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace consist.RapidEntity.CodeGenerator
{
    public class Entity : TableSchema
    {
        [XmlElement("assembly-name")]
        public virtual string Assemblyname { get; set; }
    }
}
