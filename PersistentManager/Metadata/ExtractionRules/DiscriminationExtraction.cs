using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class DiscriminationExtraction : AbstractExtraction
    {
        protected override EntityMetadata BeginExtraction(EntityMetadata metadata, XmlEntityMapping mapping)
        {
            if ( mapping.IsDiscriminating )
            {
                PropertyMetadata property = new PropertyMetadata()
                {
                    AllowNull = false ,
                    Cascade = Cascade.NOTSET ,
                    MappingName = mapping.DiscriminatorValue.Name ,
                    FieldValue = mapping.DiscriminatorValue.Value ,
                    IsDiscriminator = true ,
                    DeclaringType = mapping.Type 
                };

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
