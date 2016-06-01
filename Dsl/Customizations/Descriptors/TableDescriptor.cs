using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace consist.RapidEntity.Customizations.Descriptors
{
    public class TableDescriptor : BaseDescriptor
    {
        private string tableName;
        private List<ColumnDescriptor> columns = new List<ColumnDescriptor>( );

        public IEnumerable<ColumnDescriptor> Keys
        {
            get { return columns.Where( c => c.IsPrimaryKey == true ); }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        public List<ColumnDescriptor> Columns
        {
            get { return columns; }
            set { columns = value; }
        }

        public IEnumerable<ColumnDescriptor> ForeignKeys
        {
            get { return columns.Where( c => c.IsForeignKey == true ); }
        }
    }
}
