using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Mapping
{
    internal class JoinColumn : Attribute
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private bool _isImported;

        public bool IsImported
        {
            get { return _isImported; }
            set { _isImported = value; }
        }

        private string _ownerTable;

        public string OwnerTable
        {
            get { return _ownerTable; }
            set { _ownerTable = value; }
        }
    }
}
