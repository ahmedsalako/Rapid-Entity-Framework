using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal abstract class AbstractExtraction
    {
        static List<AbstractExtraction> extractions = new List<AbstractExtraction>( CreateExtractionPolicies() );

        static internal EntityMetadata BeginIntrospection( XmlEntityMapping mapping )
        {
            EntityMetadata metadata = new EntityMetadata( mapping.Type )
            {
                SchemaName = mapping.Name,
                HasDiscriminator = mapping.IsDiscriminating ,
            };

            extractions.ForEach( e => metadata = e.BeginExtraction( metadata , mapping ) );


            return metadata;
        }

        private static IList<AbstractExtraction> CreateExtractionPolicies()
        {
            IList<AbstractExtraction> extractions = new List<AbstractExtraction>
            {
                new TablePerTypeExtraction() ,
                new DiscriminationExtraction() ,
                new PrimaryKeyFieldExtraction() ,
                new FieldExtraction() ,
                new CompositeKeyExtraction() ,
                new OneToOneExtractionPolicy() ,
                new ManyToOneExtractionPolicy() , 
                new ManyToManyExtractionPolicy() ,
                new OneToManyExtractionPolicy() ,
                new InterceptorExtractionPolicy() ,
                new EmbeddedEntityExtractionPolicy() ,
            };

            return extractions;
        }

        protected abstract EntityMetadata BeginExtraction( EntityMetadata metadata , XmlEntityMapping mapping );

        protected abstract void EndExtraction();

        protected abstract void ValidateMapping();
    }
}
