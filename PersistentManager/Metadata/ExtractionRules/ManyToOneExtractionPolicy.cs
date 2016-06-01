using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class ManyToOneExtractionPolicy : RelationshipExtraction
    {
        protected override EntityMetadata BeginExtraction( EntityMetadata metadata , XmlEntityMapping mapping )
        {
            foreach ( ManyToOne manyToOne in mapping.ManyToOnes )
            {
                PropertyMetadata property = new PropertyMetadata()
                {
                    PropertyType = manyToOne.Property.PropertyType,
                    ClassDefinationName = manyToOne.Property.Name,
                    DeclaringType = mapping.Type,

                    EntityPropertyType = manyToOne.Property.PropertyType,
                    RelationType = manyToOne.Type ?? manyToOne.Property.PropertyType,
                    Cascade = manyToOne.Cascade,
                    IsOneToOne = false,
                    IsManyToOne = true,
                };

                if (manyToOne.RelationColumn.IsNotNull())
                {
                    property.AddJoin(new JoinMetadata
                        {
                            RelationColumn = manyToOne.RelationColumn,
                            JoinColumn = manyToOne.JoinColumn
                        }
                  );
                }

                property = AddJoin(property, manyToOne.Property);
                metadata.ColumnInfoBag.Add( property );
            }

            return metadata;
        }

        protected override void EndExtraction()
        {
            return;
        }

        protected override void ValidateMapping()
        {
            return;
        }
    }
}
