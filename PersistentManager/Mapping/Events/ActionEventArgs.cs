using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping.Events
{
    public class ActionEventArgs : EventArgs
    {
        public State ActionState { get; set; }

        public enum State
        {
            None,
            Continue,
            Rollback,
        }
    }
}
