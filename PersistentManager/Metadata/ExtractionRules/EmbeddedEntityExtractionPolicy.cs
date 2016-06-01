using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;
using System.Reflection;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class EmbeddedEntityExtractionPolicy : AbstractExtraction
    {
        protected override Descriptors.EntityMetadata BeginExtraction(Descriptors.EntityMetadata metadata, Mapping.XmlEntityMapping mapping)
        {
            List<Embedded> properties = new List<Embedded>();
            metadata.EmbeddedTypes = new List<EmbeddedEntity>();

            foreach ( EmbeddedEntity embedded in mapping.EmbeddedEntities )
            {
                PropertyMetadata property = metadata.Keys.FirstOrDefault( k => k.MappingName.AreEquals( embedded.JoinColumn ?? metadata.Keys[0].MappingName ) );

                embedded.RelationColumn = embedded.RelationColumn ?? property.MappingName;
                embedded.JoinColumn = embedded.JoinColumn ?? property.ClassDefinationName;

                Embedded key = new Embedded
                {
                    Type = property.PropertyType,
                    ColumnName = embedded.RelationColumn ?? property.MappingName,
                    PropertyName = embedded.JoinColumn ?? property.ClassDefinationName,
                    TableRef = embedded.Name
                };


                foreach (Embedded embeddedProperty in GetEmbeddedProperties( mapping ) )
                {
                    properties.Add( embeddedProperty );
                }

                Type embeddedType = GhostGenerator.CreateGhostType(embedded.Name.Trim(), string.Format("Embedded_{0}_{1}", metadata.Type.Name, embedded.Name),
                                                new[] { key },
                                                properties.ToArray());

                embedded.Type = embeddedType;
                EntityMetadata.GetMappingInfo( embeddedType );

                metadata.EmbeddedTypes.Add( embedded );
            }

            return metadata;
        }

        IEnumerable<Embedded> GetEmbeddedProperties( XmlEntityMapping mapping )
        {
            foreach ( Field field in mapping.Fields.Where( f => f.IsEmbedded()) )
            {
                yield return new Embedded
                {
                    Type = field.Property.PropertyType,
                    ColumnName = field.Name,
                    TableRef = field.TableRef,
                    PropertyName = field.Property.Name
                };
            }
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
