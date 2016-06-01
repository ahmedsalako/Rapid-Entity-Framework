using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class CompositeKeyExtraction : FieldExtraction
    {
        protected override Descriptors.EntityMetadata BeginExtraction( EntityMetadata metadata, XmlEntityMapping mapping)
        {
            foreach ( CompositeKey compositeKey in mapping.CompositeKeys )
            {
                PropertyMetadata property = new PropertyMetadata();
                property.DeclaringType = mapping.Type;
                property.RelationType = compositeKey.Property.PropertyType;
                property.PropertyType = compositeKey.Property.PropertyType;
                property.ClassDefinationName = compositeKey.Property.Name;
                property.IsPlaceHolding = true;
                property.CompositeId = Guid.NewGuid();

                metadata.Placeholders.Add( property );

                foreach ( Field field in compositeKey.Fields )
                {
                    PropertyMetadata key = ExtractField( metadata , mapping , field );
                    key.DeclaringType = mapping.Type;
                    key.CompositeId = property.CompositeId;
                    key.IsUniqueIdentifier = true;
                    key.IsCompositional = true;

                    metadata.ColumnInfoBag.Add(key);
                }
            }

            return metadata;
        }
    }
}
