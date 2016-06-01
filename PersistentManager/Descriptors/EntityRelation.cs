using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using PersistentManager.Mapping;
using System.Linq;
using PersistentManager.Initializers.Interfaces;
using PersistentManager.Initializers;

namespace PersistentManager.Descriptors
{
    internal class EntityRelation
    {
        internal PropertyMetadata PropertyMetadata { get; set; }
        internal RelationshipType RelationshipType { get; set; }
        internal PropertyInfo Property { get; set; }

        internal EntityRelation( PropertyInfo property , PropertyMetadata metadata , RelationshipType relationshipType)
        {
            PropertyMetadata = metadata;
            RelationshipType = relationshipType;
            Property = property;
        }

        internal static IEnumerable<EntityRelation> DeriveRelations( Type entity )
        {
            foreach ( PropertyMetadata mapping in MetaDataManager.MetaInfo( entity ).GetAllRelations().ToList() )
            {
                PropertyInfo propertyInfo = entity.GetProperty( mapping.ClassDefinationName );
                yield return new EntityRelation( propertyInfo , mapping , mapping.RelationshipType );
            }
        }

        internal static ILazyLoader CreateLazyHandlers( EntityRelation relation , Type entityType , object[] entityKeys )
        {
            switch ( relation.RelationshipType )
            {
                case RelationshipType.OneToMany:
                    return new OneToManyLazyHandler( entityType , relation.Property , relation.PropertyMetadata , entityKeys );
                case RelationshipType.OneToOne:
                    return new OneToOneLazyHandler( entityType , relation.Property , relation.PropertyMetadata , entityKeys );
                case RelationshipType.ManyToMany:
                    return new ManyToManyLazyHandler( entityType , relation.Property , relation.PropertyMetadata , entityKeys );
                default:
                    throw new Exception( "No Matching relationship defined for this property: " + relation.Property.Name );
            }
        }

    }
}
