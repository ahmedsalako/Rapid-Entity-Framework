using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PersistentManager.Mapping
{
    public class Key : Field
    {
        public Key()
            : base()
        {

        }

        public Key(string name) : base(name, true, false)
        {

        }

        [XmlAttribute("is-unique")]
        public override bool IsUnique
        {
            get
            {
                if(!base.IsUnique)
                    base.IsUnique = true;

                return true;
            }
            set
            {
                base.IsUnique = value;
            }
        }
    }
}
