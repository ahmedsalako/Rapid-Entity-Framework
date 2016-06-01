using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PersistentManager.Mapping
{
    interface IRelationalAttribute
    {
        Type Type { get; set;}        
        string JoinColumn { get; set; }

        [XmlArray]
        RelationJoinAttribute[] RelationJoins { get; set; }

        string RelationClass { get; set; }
    }
}
