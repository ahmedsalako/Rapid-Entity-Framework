using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PersistentManager.Mapping;
using PersistentManager.Descriptors;

namespace PersistentManager.Metadata.ExtractionRules
{
    internal class InterceptorExtractionPolicy : AbstractExtraction
    {
        protected override Descriptors.EntityMetadata BeginExtraction(Descriptors.EntityMetadata metadata, Mapping.XmlEntityMapping mapping)
        {
            foreach ( MethodInfo method in mapping.Type.GetMethods() )
            {
                foreach ( InterceptorAttribute attribute in method.GetCustomAttributes(typeof(InterceptorAttribute), true) )
                {
                    metadata.Methods.Add( new MethodMetadata( method.Name , GetQueryType(attribute) ) );
                }
            }

            return metadata;
        }

        QueryType GetQueryType(InterceptorAttribute attribute)
        {
            if (attribute is OnUpdateAttribute)
                return QueryType.Update;

            if (attribute is OnDeleteAttribute)
                return QueryType.Delete;

            if (attribute is OnCreateAttribute)
                return QueryType.Insert;

            if (attribute is OnLoadAttribute)
                return QueryType.Select;

            return QueryType.NOTSET;
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
