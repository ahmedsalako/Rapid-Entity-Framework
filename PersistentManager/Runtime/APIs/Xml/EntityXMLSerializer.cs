using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PersistentManager.Descriptors;
using System.Collections;
using System.IO;
using PersistentManager.Runtime.APIs.Xml;

namespace PersistentManager.Runtime
{
    internal class EntityXMLSerializer
    {
        internal bool IsOverridenForType( Type type , XmlAttributeOverrides overrides )
        {
            return null != overrides[type];
        }

        internal XmlAttributeOverrides AddOverridenAttributes( Type type , PropertyMetadata property , XmlAttributes attributes , XmlAttributeOverrides overrides )
        {
            if ( !IsOverridenForType( type , overrides ) )
            {
                try
                {
                    overrides.Add( type , property.ClassDefinationName , attributes );
                }
                catch
                {

                }
            }

            return overrides;
        }

        internal XmlAttributeOverrides OverrideReferenceProperty( Type type , PropertyMetadata property , XmlAttributeOverrides overrides )
        {
            XmlAttributes attributes = overrides[type] ?? new XmlAttributes( );
            XmlElementAttribute element = new XmlElementAttribute( GetSerializableType( property.RelationType ) );
            attributes.XmlElements.Add( element );  

            return AddOverridenAttributes( type , property , attributes , overrides );
        }

        internal Type GetSerializableType( Type type )
        {
            if ( MetaDataManager.IsPersistentable( type ) )
            {
                if ( type.IsInterface )
                {
                    return GetType( type );
                }
            }

            return type;
        }

        internal XmlAttributeOverrides OverrideCollectionProperty( Type type , PropertyMetadata property , XmlAttributeOverrides overrides )
        {
            if ( !property.PropertyType.IsGenericType )
            {
                XmlAttributes attributes = overrides[type] ?? new XmlAttributes( );

                attributes.XmlElements.Add( new XmlElementAttribute( GetSerializableType( property.RelationType ) ) );                
                overrides = AddOverridenAttributes( type , property , attributes , overrides );
            }
            else if( property.RelationType.IsGenericType )
            {
                XmlAttributes attributes = overrides[type] ?? new XmlAttributes( );
                attributes.XmlIgnore = true;
                overrides = AddOverridenAttributes( type , property , attributes , overrides );
            }

            return overrides;
        }

        internal XmlAttributeOverrides IgnoreElement( Type type , PropertyMetadata property , XmlAttributeOverrides overrides )
        {
            XmlAttributes attributes = overrides[type] ?? new XmlAttributes( );
            attributes.XmlIgnore = true;

            return AddOverridenAttributes( type , property , attributes , overrides );        
        }

        internal XmlAttributeOverrides IgnoreAttributes( Type type , XmlAttributeOverrides overrides )
        {
            EntityMetadata classMetadata = EntityMetadata.GetMappingInfo( type );

            foreach ( PropertyMetadata property in classMetadata )
            {
                if ( property.IsOneSided || property.IsManySided )
                {
                    overrides = IgnoreElement( type , property , overrides );
                }
            }

            return overrides;
        }

        internal XmlAttributeOverrides OverrideProperties( Type type , XmlAttributeOverrides overrides )
        {
            EntityMetadata classMetadata = EntityMetadata.GetMappingInfo( type );

            foreach ( PropertyMetadata property in classMetadata )
            {
                if ( property.IsManySided && !property.PropertyType.IsInterface )
                {
                    overrides = OverrideCollectionProperty( type , property , overrides );
                    IgnoreAttributes( property.RelationType , overrides );
                }
                else if ( property.IsOneSided && !property.PropertyType.IsInterface )
                {
                    overrides = OverrideReferenceProperty( type , property , overrides );
                    IgnoreAttributes( property.RelationType , overrides );
                }
                else if( property.IsManySided || property.IsOneSided )
                {
                    IgnoreElement( type , property , overrides );
                }
            }

            return overrides;
        }

        public T DeserializeEntity<T>( String stringXML )
        {
            Type type = GetType( typeof( T ) );
            XmlAttributeOverrides overrides = new XmlAttributeOverrides( );
            overrides = OverrideProperties( type , overrides );

            XmlSerializer serializer = new XmlSerializer( type , overrides );
            MemoryStream memoryStream = new MemoryStream( UTF8Encoder.StringToByeteArray( stringXML ) );

            return (T) serializer.Deserialize( memoryStream );
        }

        public T DeserializeEntity<T>( XmlDocument xml )
        {
            return DeserializeEntity<T>( xml.InnerXml );
        }        

        public XmlDocument SerializeEntity<T>( T entity )
        {
            if ( entity.IsNotNull( ) )
            {
                Type type = GetType( typeof( T ) );
                XmlAttributeOverrides overrides = new XmlAttributeOverrides( );
                
                overrides = OverrideProperties( type , overrides );

                MemoryStream buffer = new MemoryStream( );
                XmlSerializer serializer = new XmlSerializer( type , overrides );
                XmlTextWriter xmlTextWriter = new XmlTextWriter( buffer , Encoding.UTF8 );

                serializer.Serialize( xmlTextWriter , entity );

                buffer = ( MemoryStream ) xmlTextWriter.BaseStream;

                XmlDocument xmlDocument = new XmlDocument( );
                xmlDocument.Load( new StringReader( UTF8Encoder.ByteArrayToString( buffer.ToArray( ) ) ) );

                return xmlDocument ;
            }

            return null;
        }

        private Type GetType( Type type )
        {
            if ( type.IsClass )
                return type;

            if ( type.IsInterface )
                return GhostGenerator.CreateGhostType( type );

            return null;
        }
    }
}
