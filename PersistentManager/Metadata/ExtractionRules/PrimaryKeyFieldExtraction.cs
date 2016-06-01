using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class PrimaryKeyFieldExtraction : FieldExtraction
    {
        protected override EntityMetadata BeginExtraction(Descriptors.EntityMetadata metadata, Mapping.XmlEntityMapping mapping)
        {
            foreach ( Field field in mapping.Keys )
            {
                PropertyMetadata property = ExtractField(metadata, mapping, field);
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
