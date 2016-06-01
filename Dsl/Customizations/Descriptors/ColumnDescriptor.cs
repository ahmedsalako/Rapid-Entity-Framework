using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace consist.RapidEntity.Customizations.Descriptors
{
    public class ColumnDescriptor: BaseDescriptor
    {
        private string tableName;
        private string languageDataType;
        private string colunmName;
        private bool isForeignKey = false;
        private bool isPrimaryKey = false;
        private bool isReadOnly = false;
        private bool isNullable = false;
        private string foreignKeyName;
        private string foreignKeyTableName;
        private string foreignKeyColumnName;
        private string foreignKeyOwnerName;
        private string foreignKeyOwnerColumn;
        private bool isAutoIncrement = false;
        private string primaryKeyName;
        private int intSQLType;
        private bool isUnique;
        private int precision = 0;
        private int scale = 0;

        public string ForeignKeyOwnerColumn
        {
            get { return foreignKeyOwnerColumn; }
            set { foreignKeyOwnerColumn = value; }
        }

        public string ForeignKeyOwnerName
        {
            get { return foreignKeyOwnerName; }
            set { foreignKeyOwnerName = value; }
        }

        public string ForeignKeyColumnName
        {
            get { return foreignKeyColumnName; }
            set { foreignKeyColumnName = value; }
        }

        public string ForeignKeyTableName
        {
            get { return foreignKeyTableName; }
            set { foreignKeyTableName = value; }
        }

        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        
        public string LanguageDataType
        {
            get { return languageDataType; }
            set { languageDataType = value; }
        }        

        public string ColunmName
        {
            get { return colunmName; }
            set { colunmName = value; }
        }        

        public bool IsForeignKey
        {
            get { return isForeignKey; }
            set { isForeignKey = value; }
        }        

        public bool IsPrimaryKey
        {
            get { return isPrimaryKey; }
            set { isPrimaryKey = value; }
        }        

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }        

        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }        

        public int IntSQLType
        {
            get { return intSQLType; }
            set { intSQLType = value; }
        }        

        public bool IsAutoIncrement
        {
            get { return isAutoIncrement; }
            set { isAutoIncrement = value; }
        }        

        public string PrimaryKeyName
        {
            get { return primaryKeyName; }
            set { primaryKeyName = value; }
        }        

        public string ForeignKeyName
        {
            get { return foreignKeyName; }
            set { foreignKeyName = value; }
        }

        public int Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public int Scale
        {
            get { return scale; }
            set { scale = value; }
        }
    }
}
