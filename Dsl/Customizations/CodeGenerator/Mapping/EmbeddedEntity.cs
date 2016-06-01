using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace consist.RapidEntity.CodeGenerator
{
    public class EmbeddedEntity
    {
        public string Name { get; set; }
        public string RelationColumn { get; set; }
        public string JoinColumn { get; set; }
        internal Type Type { get; set; }
    }
}
