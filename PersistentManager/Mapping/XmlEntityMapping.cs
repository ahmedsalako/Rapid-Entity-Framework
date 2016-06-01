using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using PersistentManager.Metadata;
using PersistentManager.Util;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace PersistentManager.Mapping
{
    [XmlRoot(ElementName = "xml-entity-mapping", Namespace = "http://rql-rapid.org")]
    public class XmlEntityMapping : Entity
    {
        public XmlEntityMapping()
        {
            Keys = new Key[0];
            Fields = new Field[0];
            CompositeKeys = new CompositeKey[0];
            ManyToManys = new ManyToMany[0];
            OneToOnes = new OneToOne[0];
            ManyToOnes = new ManyToOne[0];
            OneToManys = new OneToMany[0];
            EmbeddedEntities = new EmbeddedEntity[0];
        }

        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public Key[] Keys { get; set; }

        [XmlElement("discriminator")]
        public DiscriminatorValue DiscriminatorValue { get; set; }

        [XmlArray("composite-keys")]
        [XmlArrayItem("composite-key")]
        public CompositeKey[] CompositeKeys { get; set; }

        [XmlElement("field")]
        public Field[] Fields { get; set; }

        [XmlArray("many-to-manys")]
        [XmlArrayItem("many-to-many")]
        public ManyToMany[] ManyToManys { get; set; }

        [XmlArray("one-to-ones")]
        [XmlArrayItem("one-to-one")]
        public OneToOne[] OneToOnes { get; set; }

        [XmlArray("many-to-ones")]
        [XmlArrayItem("many-to-one")]
        public ManyToOne[] ManyToOnes { get; set; }

        [XmlArray("one-to-manys")]
        [XmlArrayItem("one-to-many")]
        public OneToMany[] OneToManys { get; set; }

        [XmlArray("embedded-entities")]
        public EmbeddedEntity[] EmbeddedEntities { get; set; }

        [XmlIgnore]
        internal bool HasInheritance { get; set; }

        [XmlIgnore]
        internal bool IsDiscriminating { get; set; }

        [XmlIgnore]
        internal Type BaseType { get; set; }

        [XmlIgnore]
        internal Type Type { get; set; }

        public static IEnumerable<T> GetCustomAttribute<T>( Type type , bool inherit )
        {
            foreach (T element in type.GetCustomAttributes(typeof(T), inherit))
                yield return (T)element;
        }

        public static IEnumerable<T> GetPropertyCustomAttribute<T>( Type type , bool inherit , bool isDicriminating , bool hasBaseEntity )
        {
            foreach ( PropertyInfo property in type.GetProperties() )
            {
                if ( hasBaseEntity && property.DeclaringType != type )
                    continue;

                foreach (T element in property.GetCustomAttributes(typeof(T), inherit))
                {
                    if (element is Field)
                    {
                        (element as Field).Property = property;
                    }
                    else if (element is CompositeKey)
                    {
                        (element as CompositeKey).Property = property;
                        (element as CompositeKey).Fields = GetPropertyCustomAttribute<Field>( property.PropertyType , true , false , false ).ToArray();
                    }
                    else if (element is OneToMany)
                    {
                        (element as OneToMany).Property = property;
                    }
                    else if (element is OneToOne)
                    {
                        (element as OneToOne).Property = property;
                    }
                    else if (element is ManyToOne)
                    {
                        (element as ManyToOne).Property = property;
                    }
                    else if (element is ManyToMany)
                    {
                        (element as ManyToMany).Property = property;
                    }

                    yield return (T)element;
                }
            }
        }


        /// <summary>
        /// We need to be able to extract a unique class by its namespace
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="typename"></param>
        /// <returns></returns>
        internal static Type FindTypeByName( Assembly assembly , string typename )
        {
            return assembly.GetTypes().FirstOrDefault(t => t.Name == typename);
        }

        internal static T AddPropertyInfo<T>( T element , Assembly assembly , PropertyInfo property )
        {
            if (element is Field)
            {
                (element as Field).Property = property;
            }
            else if (element is CompositeKey)
            {
                (element as CompositeKey).Property = property;
                (element as CompositeKey).Fields = GetPropertyCustomAttribute<Field>(property.PropertyType, true , false , false ).ToArray();
            }
            else if (element is OneToMany)
            {
                string relation = (element as OneToMany).RelationClass;
                (element as OneToMany).Type = FindTypeByName( assembly , relation);
                (element as OneToMany).Property = property;
            }
            else if (element is OneToOne)
            {
                string relation = (element as OneToOne).RelationClass;
                (element as OneToOne).Type = FindTypeByName(assembly, relation);
                (element as OneToOne).Property = property;
            }
            else if (element is ManyToOne)
            {
                string relation = (element as ManyToOne).RelationClass;
                (element as ManyToOne).Type = FindTypeByName(assembly, relation);
                (element as ManyToOne).Property = property;
            }
            else if (element is ManyToMany)
            {
                string relation = (element as ManyToMany).RelationClass;
                (element as ManyToMany).Type = FindTypeByName(assembly, relation);
                (element as ManyToMany).Property = property;
            }

            return (T)element;
        }

        public static XmlEntityMapping ExtractFromXmlFile( Type entityType )
        {
            Assembly assembly = entityType.Assembly;

            if ( null != assembly )
            {
                foreach( string mappingXml in assembly.GetManifestResourceNames().Where( e => StringUtil.Contains( e , entityType.Name + Constant.RAPID_EMBEDDED_XML_MAPPING_SUFFIX ) ) )
                {

                    if (StringUtil.Contains( mappingXml , entityType.Name + Constant.RAPID_EMBEDDED_XML_MAPPING_SUFFIX ))
                    {

                        Stream stream = assembly.GetManifestResourceStream( mappingXml );

                        StringReader reader = new StringReader(((TextReader)(new StreamReader( stream ))).ReadToEnd());
                        XmlDocument document = new XmlDocument();
                        document.Load( reader );

                        XmlEntityMapping mapping = XmlMappingSerializer.DeserializeEntity(  document.InnerXml );
                        mapping.Type = entityType;
                        mapping.BaseType = entityType.BaseType;

                        mapping.IsDiscriminating = mapping.DiscriminatorValue.IsNotNull();
                        mapping.HasInheritance = entityType.BaseType.IsNotNull() && ExtractFromXmlFile( entityType.BaseType ).IsNotNull() ? true : false;

                        foreach( PropertyInfo property in entityType.GetProperties())
                        {
                            if (mapping.Keys.IsNotNull())
                            {
                                mapping.Keys.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo<Key>(k, assembly, property));
                            }
                            else
                            {
                                mapping.Keys = new Key[0];
                            }

                            if (mapping.Fields.IsNotNull())
                            {
                                mapping.Fields.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo<Field>(k, assembly, property));
                            }
                            else
                            {
                                mapping.Fields = new Field[0];
                            }

                            if (mapping.CompositeKeys.IsNotNull())
                            {
                                mapping.CompositeKeys.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo(k, assembly, property));
                            }
                            else
                            {
                                mapping.CompositeKeys = new CompositeKey[0];
                            }

                            if (mapping.ManyToManys.IsNotNull())
                            {
                                mapping.ManyToManys.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo(k, assembly, property));
                            }
                            else
                            {
                                mapping.ManyToManys = new ManyToMany[0];
                            }

                            if (mapping.OneToOnes.IsNotNull())
                            {
                                mapping.OneToOnes.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo(k, assembly, property));
                            }
                            else
                            {
                                mapping.OneToOnes = new OneToOne[0];
                            }

                            if (mapping.ManyToOnes.IsNotNull())
                            {
                                mapping.ManyToOnes.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo(k, assembly, property));
                            }
                            else
                            {
                                mapping.ManyToOnes = new ManyToOne[0];
                            }

                            if (mapping.OneToManys.IsNotNull())
                            {
                                mapping.OneToManys.Where(k => k.PropertyName == property.Name).ToList().ForEach(k => AddPropertyInfo(k, assembly, property));
                            }
                            else
                            {
                                mapping.OneToManys = new OneToMany[0];
                            }
                        }

                        return mapping;
                    }
                }
                
            }


            return null;
        }

        public static XmlEntityMapping ExtractFromAttributes( Type entityType )
        {
            TableSchema entityAttribute = GetCustomAttribute<TableSchema>(entityType, true).FirstOrDefault();

            if ( entityAttribute == null )
                return null;

            XmlEntityMapping mapping = new XmlEntityMapping();
            mapping.ClassName = entityType.Name;
            mapping.Name = entityAttribute.Name;
            mapping.RelationColumn = entityAttribute.RelationColumn;
            mapping.JoinColumn = entityAttribute.JoinColumn;
            mapping.Type = entityType;
            mapping.BaseType = entityType.BaseType;
            
            mapping.DiscriminatorValue = GetCustomAttribute<DiscriminatorValue>( entityType , false ).FirstOrDefault();
            mapping.IsDiscriminating = mapping.DiscriminatorValue.IsNotNull();
            mapping.HasInheritance = (entityType.BaseType.IsNotNull() && GetCustomAttribute<Entity>(entityType.BaseType, true).FirstOrDefault().IsNotNull() ? true : false)
                                        && !mapping.IsDiscriminating;

            mapping.EmbeddedEntities = GetCustomAttribute<EmbeddedEntity>( entityType , false ).ToArray();


            mapping.Fields = GetPropertyCustomAttribute<Field>(entityType, false, mapping.IsDiscriminating , mapping.HasInheritance ).ToArray();
            mapping.CompositeKeys = GetPropertyCustomAttribute<CompositeKey>(entityType, false, mapping.IsDiscriminating , mapping.HasInheritance).ToArray();            
            mapping.ManyToManys = GetPropertyCustomAttribute<ManyToMany>(entityType, false, mapping.IsDiscriminating, mapping.HasInheritance ).ToArray();
            mapping.ManyToOnes = GetPropertyCustomAttribute<ManyToOne>(entityType, false, mapping.IsDiscriminating , mapping.HasInheritance ).ToArray();
            mapping.OneToManys = GetPropertyCustomAttribute<OneToMany>(entityType, false, mapping.IsDiscriminating , mapping.HasInheritance ).ToArray();
            mapping.OneToOnes = GetPropertyCustomAttribute<OneToOne>(entityType, false, mapping.IsDiscriminating, mapping.HasInheritance ).ToArray();  

            return mapping;
        }
    }
}
