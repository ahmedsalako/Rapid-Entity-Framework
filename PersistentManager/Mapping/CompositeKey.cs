using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false )]
    public class CompositeKey : Attribute
    {
        [XmlIgnore]
        internal PropertyInfo Property { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("key-class")]
        public string KeyClass { get; set; }

        public Field[] Fields { get; set; }
    }
}
