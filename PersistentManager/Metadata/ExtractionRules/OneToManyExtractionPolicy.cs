using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class OneToManyExtractionPolicy : RelationshipExtraction
    {
        protected override EntityMetadata BeginExtraction( EntityMetadata metadata , XmlEntityMapping mapping )
        {
            foreach ( OneToMany oneToMany in mapping.OneToManys )
            {
                PropertyMetadata property = new PropertyMetadata()
                {
                    DeclaringType = mapping.Type,
                    PropertyType = oneToMany.Property.PropertyType,
                    ClassDefinationName = oneToMany.Property.Name,
                    IsOneToMany = true,
                    RelationType = oneToMany.Type,
                    Cascade = oneToMany.Cascade,
                };

                if (oneToMany.RelationColumn.IsNotNull())
                {
                    property.AddJoin(new JoinMetadata
                                    {
                                        RelationColumn = oneToMany.RelationColumn,
                                        JoinColumn = oneToMany.JoinColumn
                                    }
                    );
                }

                property = AddJoin( property , oneToMany.Property );

                metadata.LazyBag.Add( property );
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
