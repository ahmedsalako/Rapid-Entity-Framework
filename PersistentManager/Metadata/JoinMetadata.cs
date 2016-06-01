using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Metadata
{
    public class JoinMetadata : IMetadataDescriptor , ICloneable
    {
        public string RelationColumn { get; set; }
        public string JoinColumn { get; set; }
        public string OwnerColumn { get; set; }

        public Type ColumnType { get; set; }
        public string LeftKey { get; set; }
        public string RightKey { get; set; }
        public object Value { get; set; }

        internal JoinMetadata( ) { }

        internal JoinMetadata( string RelationColumn , string JoinColumn , Type ColumnType ) : this()
        {
            this.RelationColumn = RelationColumn;
            this.JoinColumn = JoinColumn;
            this.ColumnType = ColumnType;
        }

        //For Many - To - Many
        internal JoinMetadata( string OwnerColumn , string JoinColumn , Type ColumnType , string LeftKey , string RightKey ) : this()
        {
            this.OwnerColumn = OwnerColumn;
            this.JoinColumn = JoinColumn;
            this.ColumnType = ColumnType;
            this.RightKey = RightKey;
            this.LeftKey = LeftKey;
        }

        #region ICloneable Members

        public object Clone( )
        {
            return this.MemberwiseClone( );
        }

        #endregion
    }
}
