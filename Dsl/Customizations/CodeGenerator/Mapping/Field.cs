using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class FieldXml
    {
        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlAttribute("exclude")]
        public string Exclude { get; set; }

        [XmlAttribute("allow-null")]
        public bool AllowNullValue { get; set; }

        [XmlAttribute("is-auto-key")]
        public virtual bool AutoKey { get; set; }

        [XmlAttribute("is-unique")]
        public virtual bool IsUnique { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("db-type")]
        public string DbDataType { get; set; }

        [XmlAttribute("length")]
        public string Length { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("table-ref")]
        public string TableRef { get; set; }
    }
}
