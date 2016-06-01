using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace consist.RapidEntity.CodeGenerator
{
    public class XmlMappingSerializer
    {
        public static XmlEntityMapping DeserializeEntity(String stringXML)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlEntityMapping));

            return (XmlEntityMapping) serializer.Deserialize( new StringReader( stringXML ) );
        }

        public static XmlDocument SerializeEntity( XmlEntityMapping mappingEntity )
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlEntityMapping));

            StringBuilder sb = new StringBuilder();
            UTF8StringWriter writer = new UTF8StringWriter( sb );
            serializer.Serialize( writer , mappingEntity );

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml( sb.ToString() );

            return xmlDocument;
        }
    }
}
