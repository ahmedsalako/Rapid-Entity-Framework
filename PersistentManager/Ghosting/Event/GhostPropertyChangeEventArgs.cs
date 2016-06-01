using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PersistentManager.Ghosting.Event
{
    public class GhostPropertyChangeEventArgs : PropertyChangedEventArgs
    {
        public GhostPropertyChangeEventArgs(string propertyName, object value) : base(propertyName)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        private string propertyName;

        public override string PropertyName
        {
            get { return propertyName; }
        }

        private object value;

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
