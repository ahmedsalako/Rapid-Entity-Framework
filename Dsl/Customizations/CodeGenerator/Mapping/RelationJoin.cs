using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    public class RelationJoin
    {
        public string RelationColumn { get; set; }
        public string JoinColumn { get; set; }
        public string OwnerColumn { get; set; }
        public Type ColumnType { get; set; }
        public string LeftKey { get; set; }
        public string RightKey { get; set; }
    }
}
