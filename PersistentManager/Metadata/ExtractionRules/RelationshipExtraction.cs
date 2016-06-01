using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;
using System.Reflection;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal abstract class RelationshipExtraction : AbstractExtraction
    {
        protected PropertyMetadata AddJoin( PropertyMetadata propertyMetadata , PropertyInfo propertyInfo)
        {
            foreach ( RelationJoinAttribute relationJoinAttribute in propertyInfo.GetCustomAttributes(typeof(RelationJoinAttribute), true))
            {
                propertyMetadata.AddJoin(new JoinMetadata
                {
                    RelationColumn = relationJoinAttribute.RelationColumn,
                    JoinColumn = relationJoinAttribute.JoinColumn,
                    ColumnType = relationJoinAttribute.ColumnType,
                    OwnerColumn = relationJoinAttribute.OwnerColumn,
                    LeftKey = relationJoinAttribute.LeftKey,
                    RightKey = relationJoinAttribute.RightKey,
                }
                                         );
            }

            return propertyMetadata;
        }
    }
}
