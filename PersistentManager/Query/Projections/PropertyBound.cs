using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Projections
{
    internal class PropertyBound
    {
        internal PropertyMetadata Metadata;
        internal ClassResultHandler ResultHandler { get; set; }
        internal bool IsExtendedProjection { get; set; }
        internal Criteria Criteria { get; set; }        
        internal string RightName { get; set; }
        internal string LeftName { get; set; }
        internal bool IsDefered { get; set; }
        internal object Value { get; set; }
        internal Type LeftType { get; set; }        

        internal PropertyBound( string leftName , Type leftType , string rightName  , object value )
        {
            RightName = rightName;
            LeftName = leftName;
            LeftType = leftType;
            Value = value;
        }

        internal PropertyBound( string leftName , Type leftType , string rightName , object value , PropertyMetadata metadata , Criteria criteria ) : 
            this( leftName , leftType , rightName , null )
        {
            this.Criteria = criteria;
            this.Metadata = metadata;
        }

        internal PropertyBound( string leftName , Type leftType , string rightName , object value , PropertyMetadata metadata , ClassResultHandler handler ) :
            this( leftName , leftType , rightName , null )
        {
            this.IsExtendedProjection = true;
            this.ResultHandler = handler;
            this.Metadata = metadata;            
        }

        internal PropertyBound( string leftName , Type leftType , bool IsDefered , object value )
        {
            this.Value = value;
            this.LeftName = leftName;
            this.IsDefered = IsDefered;
            this.LeftType = leftType;
        }
    }
}
