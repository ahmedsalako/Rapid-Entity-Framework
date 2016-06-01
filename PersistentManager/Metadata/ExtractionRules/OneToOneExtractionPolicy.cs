using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class OneToOneExtractionPolicy : RelationshipExtraction
    {
        protected override Descriptors.EntityMetadata BeginExtraction(Descriptors.EntityMetadata metadata, Mapping.XmlEntityMapping mapping)
        {
            foreach ( OneToOne oneToOne in mapping.OneToOnes )
            {
                PropertyMetadata property = new PropertyMetadata()
                {
                    PropertyType = oneToOne.Property.PropertyType,
                    ClassDefinationName = oneToOne.Property.Name,
                    DeclaringType = mapping.Type,

                    EntityPropertyType = oneToOne.Property.PropertyType,
                    RelationType = oneToOne.Type ?? oneToOne.Property.PropertyType,
                    Cascade = oneToOne.Cascade,
                    IsOneToOne = true,
                    IsManyToOne = false,
                    IsImported = oneToOne.IsImported,
                    IsEntitySplitJoin = oneToOne.IsSplit,
                };

                if ( oneToOne.RelationColumn.IsNotNull() )
                {
                    property.AddJoin(new JoinMetadata
                                        {
                                            RelationColumn = oneToOne.RelationColumn,
                                            JoinColumn = oneToOne.JoinColumn
                                        }
                                  );
                }

                property = AddJoin( property , oneToOne.Property );

                if (property.IsImported || property.IsManyToOne)
                {
                    metadata.ImportedBag.Add(property);
                }
                else
                {
                    metadata.ColumnInfoBag.Add(property);
                }
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
