using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface , AllowMultiple = false)]
    public class Entity : TableSchema
    {
        public Entity( string name ): base(name)
        {

        }                

        public Entity() : base()
        {

        }

        [XmlElement("assembly-name")]
        public virtual string Assemblyname { get; set; }
    }
}
