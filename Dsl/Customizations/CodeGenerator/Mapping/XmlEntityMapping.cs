using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace consist.RapidEntity.CodeGenerator
{
    [XmlRoot(ElementName = "xml-entity-mapping", Namespace = "http://rql-rapid.org")]
    public class XmlEntityMapping : Entity
    {
        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public KeyXml[] Keys { get; set; }

        [XmlElement("discriminator")]
        public DiscriminatorValueXml DiscriminatorValue { get; set; }

        [XmlArray("composite-keys")]
        [XmlArrayItem("composite-key")]
        public CompositeKeyXml[] CompositeKeys { get; set; }

        [XmlElement("field")]
        public FieldXml[] Fields { get; set; }

        [XmlArray("many-to-manys")]
        [XmlArrayItem("many-to-many")]
        public ManyToManyXml[] ManyToManys { get; set; }

        [XmlArray("one-to-ones")]
        [XmlArrayItem("one-to-one")]
        public OneToOneXml[] OneToOnes { get; set; }

        [XmlArray("many-to-ones")]
        [XmlArrayItem("many-to-one")]
        public ManyToOneXml[] ManyToOnes { get; set; }

        [XmlArray("one-to-manys")]
        [XmlArrayItem("one-to-many")]
        public OneToManyXml[] OneToManys { get; set; }

        [XmlArray("embedded-entities")]
        public EmbeddedEntity[] EmbeddedEntities { get; set; }

        public System.Xml.XmlDocument Serialize()
        {
            return XmlMappingSerializer.SerializeEntity(this);
        }
    }
}
