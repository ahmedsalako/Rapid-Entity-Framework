using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PersistentManager.Descriptors;
using PersistentManager.Mapping;
using PersistentManager.Util;
using PersistentManager.Metadata.ExtractionRules;

namespace PersistentManager.Metadata
{
    internal abstract class MappingBase
    {
        internal static EntityMetadata Introspect( Type entity )
        {
            XmlEntityMapping mapping =  XmlEntityMapping.ExtractFromAttributes( entity );

            if (null == mapping) 
            {
                mapping = XmlEntityMapping.ExtractFromXmlFile( entity );
            }

            if (null == mapping) return null;

            return AbstractExtraction.BeginIntrospection( mapping );
            
            //MetadataGenerator.Introspection( mapping );
        }

        internal static void DeriveJoinKeysForRelatedProperties( Type entity , EntityMetadata metadata )
        {
            foreach ( PropertyMetadata property in metadata.GetAll( ).Where( p => p.IsRelationshipMapping ) )
            {
                if ( property.IsManyToMany )
                {
                    List<string> properties = new List<string>();
                    List<Type> types = new List<Type>();

                    foreach( JoinMetadata joinMetadata in property.JoinDetails )
                    {
                        if( joinMetadata.LeftKey.IsNull() )
                        {
                            joinMetadata.LeftKey = metadata.Keys[0].MappingName;
                            joinMetadata.ColumnType = metadata.Keys[0].PropertyType;
                        }

                        properties.Add( joinMetadata.OwnerColumn );

                        if (joinMetadata.ColumnType == null)
                        {
                            PropertyMetadata key = metadata.GetKeyByMapping(joinMetadata.LeftKey);

                            if (null != key)
                            {
                                joinMetadata.ColumnType = key.PropertyType;
                            }
                            else
                            {
                                joinMetadata.ColumnType = typeof(object);
                            }
                        }

                        types.Add(joinMetadata.ColumnType);

                        if( joinMetadata.RightKey.IsNull() )
                        {
                            PropertyMetadata key = EntityMetadata.GetMappingInfo( property.RelationType ).Keys[0];

                            joinMetadata.RightKey = key.MappingName ;
                            joinMetadata.ColumnType = key.PropertyType;
                        }

                        properties.Add(joinMetadata.JoinColumn);
                        types.Add(joinMetadata.ColumnType);
                    }

                    property.JoinTableType = CreateJoinTableType( property.JoinTable , properties.ToArray() , types.ToArray() );
                }
                else if ( property.IsOneToMany && property.JoinDetails.Count == 1 )
                {
                    foreach ( JoinMetadata join in property.JoinDetails )
                    {
                        join.JoinColumn = join.JoinColumn ?? EntityMetadata.GetMappingInfo(entity).Keys[0].MappingName;
                    }                    
                }
                else if ( property.JoinDetails.Count == 1 && property.IsOneSided )
                {
                    foreach ( JoinMetadata join in property.JoinDetails )
                    {
                        join.JoinColumn = join.JoinColumn ?? EntityMetadata.GetMappingInfo(property.RelationType).Keys[0].MappingName;
                    }
                }
            }
        }

        private static Type CreateJoinTableType( string typename , string[] properties , Type[] propertyTypes )
        {
            Type joinType = GhostGenerator.CreateGhostType( typename , string.Format( "JOIN_VIRTUAL_TABLE_{0}" , typename ) , CreateEmbeddeds( properties , propertyTypes ).ToArray( ) , new List<Embedded>( ).ToArray( ) );

            EntityMetadata.GetMappingInfo( joinType );

            return joinType;
        }

        private static IEnumerable<Embedded> CreateEmbeddeds( string[] properties , Type[] propertyTypes )
        {
            foreach ( int index in properties.GetIndices( ) )
            {
                yield return new Embedded{
                                            ColumnName = properties[index] ,
                                            PropertyName = properties[index] ,
                                            Type = propertyTypes[index]
                                          };
            }
        }

    }
}
