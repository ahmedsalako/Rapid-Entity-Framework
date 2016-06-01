using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class CompositeKeyXml
    {
        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("key-class")]
        public string KeyClass { get; set; }

        public FieldXml[] Fields { get; set; }
    }
}
