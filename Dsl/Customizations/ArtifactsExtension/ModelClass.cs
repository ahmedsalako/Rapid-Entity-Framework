using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using consist.RapidEntity.Customizations.Descriptors;

namespace consist.RapidEntity
{
    public partial class ModelClass
    {
        TableDescriptor tableDescriptor = new TableDescriptor();

        public TableDescriptor TableDescriptor
        {
            get { return tableDescriptor; }
            set { tableDescriptor = value; }
        }
    }
}
