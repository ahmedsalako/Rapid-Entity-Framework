using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class ManyToManyExtractionPolicy : RelationshipExtraction
    {
        protected override Descriptors.EntityMetadata BeginExtraction( EntityMetadata metadata ,  XmlEntityMapping mapping )
        {
            foreach ( ManyToMany manyToMany in mapping.ManyToManys )
            {
                PropertyMetadata property = new PropertyMetadata()
                {
                    ClassDefinationName = manyToMany.Property.Name,
                    PropertyType = manyToMany.Property.PropertyType,
                    IsManyToMany = true,
                    DeclaringType = mapping.Type,
                    RelationType = manyToMany.Type,
                    JoinTable = manyToMany.JoinTable,
                    Cascade = manyToMany.Cascade,
                };

                if (manyToMany.OwnerColumn.IsNotNull())
                {
                    property.AddJoin(new JoinMetadata
                    {
                        OwnerColumn = manyToMany.OwnerColumn,
                        JoinColumn = manyToMany.JoinColumn,
                        LeftKey = manyToMany.LeftKey,
                        RightKey = manyToMany.RightKey,
                    }
                  );
                }

                property = AddJoin( property , manyToMany.Property );
                
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
