using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Exceptions.EntityManagerException;
using System.Reflection;

namespace PersistentManager.Metadata
{
    internal class MetadataValidation
    {
        internal static void ValidateMetadata( EntityMetadata metadata )
        {
            ValidateIsEntity( metadata );
            ValidateHasFields( metadata );
            ValidateHasIdentifier( metadata );
            ValidatePersistentFieldsAreGhostable( metadata );
            ValidateRelationshipMappings( metadata );
        }

        internal static void ValidatePersistentFieldsAreGhostable( EntityMetadata metadata )
        {
            foreach ( PropertyMetadata column in metadata.GetAll( ) )
            {
                if ( column.IsRelationshipMapping )
                {
                    ValidatePropertyIsVirtual( column , metadata.Type );
                }
            }
        }

        internal static void ValidatePropertyIsVirtual( PropertyMetadata column , Type type )
        {
            PropertyInfo propertyInfo = type.GetProperty( column.ClassDefinationName );

            if ( !propertyInfo.IsNull( ) )
            {
                foreach ( MethodInfo methodInfo in propertyInfo.GetAccessors( true ) )
                {
                    if ( !methodInfo.IsVirtual )
                    {
                        throw new PersistentException( string.Format( "Property {0} of {1} entity should be virtual property!" , column.ClassDefinationName , type.Name ) );
                    }
                }
            }
        }

        internal static void ValidateRelationshipMappings( EntityMetadata metadata )
        {
            //ValidateOneToOne(metadata.GetRelationshipMappings(RelationshipType.OneToOne));
        }

        private static void ValidateOneToOne( IEnumerable<PropertyMetadata> oneToOnes )
        {

        }

        internal static void ValidateIsEntity( EntityMetadata metadata )
        {
            if ( metadata.IsNull( ) && metadata.Type.IsNull( ) )
                throw new PersistentException( "Entity is not persistable!. Mark with entity attribute!" );
        }

        internal static void ValidateHasFields( EntityMetadata metadata )
        {
            if ( metadata.GetAll( ).Count <= 0 )
                throw new PersistentException( "You must define at least one persistable field for this entity!" );
        }

        internal static void ValidateHasIdentifier( EntityMetadata metadata )
        {
            PropertyMetadata column = metadata.Keys.Where( c => c.IsUniqueIdentifier == true ).FirstOrDefault( );

            if ( column.IsNull( ) )
                throw new PersistentException( string.Format( "The entity {0} needs a valid Key or Identifier! Map with Key attribute!" , metadata.Type ) );
        }
    }
}
