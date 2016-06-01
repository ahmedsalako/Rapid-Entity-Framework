using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface , AllowMultiple = false)]
    public class DiscriminatorValue : Attribute
    {
        private string name;
        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public DiscriminatorValue(string name, object value)
        {
            this.name = name;
            this._value = value;
        }

        public DiscriminatorValue()
        {

        }
    }
}
