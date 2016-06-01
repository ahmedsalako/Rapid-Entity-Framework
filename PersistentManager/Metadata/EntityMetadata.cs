using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Mapping;
using System.Collections;
using PersistentManager.Metadata;
using PersistentManager.Util;
using System.Linq;
using PersistentManager.Initializers.Interfaces;
using PersistentManager.Initializers;
using System.Reflection;

namespace PersistentManager.Descriptors
{
    internal class EntityMetadata : IEnumerable , ICloneable
    {
        private IList<PropertyMetadata> placeholders = new List<PropertyMetadata>( );
        private IList<PropertyMetadata> columnInfoBag = new List<PropertyMetadata>( );
        private IList<MethodMetadata> methods = new List<MethodMetadata>( );
        private IList<PropertyMetadata> lazyBag = new List<PropertyMetadata>( );
        private IList<PropertyMetadata> importedBag = new List<PropertyMetadata>( );
        private PropertyMetadata inheritanceRelation;
        private PropertyMetadata[] keys = null;
        private Type type;

        internal IList<MethodMetadata> Methods
        {
            get { return methods; }
            set { methods = value; }
        }
        public IList<PropertyMetadata> LazyBag
        {
            get { return lazyBag; }
            set { lazyBag = value; }
        }

        //Rem: Table per classes in hierachy uses on primary key down the inheritance chain
        public PropertyMetadata InheritanceRelation
        {
            get { return inheritanceRelation; }
            set { inheritanceRelation = value; }
        }

        public bool HasBaseEntity
        {
            get { return inheritanceRelation.IsNotNull( ); }
        }

        public bool HasEmbeddedEntities
        {
            get { return EmbeddedTypes.IsNotNull( ) && EmbeddedTypes.Count > 0; }
        }

        public bool IsEmbedded( Type type )
        {
            return EmbeddedTypes.FirstOrDefault( e => e.Type == type ).IsNotNull( );
        }

        public EmbeddedEntity GetEmbedded( Type type )
        {
            return EmbeddedTypes.FirstOrDefault( e => e.Type == type );
        }

        public EmbeddedEntity GetEmbeddedIncludeBase( Type type )
        {
            EmbeddedEntity entity = GetEmbedded( type );

            if ( null == entity && HasBaseEntity )
            {
                entity = MetaDataManager.MetaInfo( BaseEntity ).GetEmbeddedIncludeBase( type );
            }

            return entity;
        }

        public Type BaseEntity
        {
            get
            {
                if ( HasBaseEntity )
                {
                    return inheritanceRelation.RelationType;
                }

                return null;
            }
        }

        public List<EmbeddedEntity> EmbeddedTypes { get; set; }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public IList<PropertyMetadata> ColumnInfoBag
        {
            get
            {
                return columnInfoBag;
            }
            set { columnInfoBag = value; }
        }

        public IList<PropertyMetadata> ImportedBag
        {
            get { return importedBag; }
            set { importedBag = value; }
        }

        public IList<PropertyMetadata> Placeholders
        {
            get { return placeholders; }
            set { placeholders = value; }
        }

        public PropertyMetadata this[int i]
        {
            get
            {
                List<PropertyMetadata> allBags = new List<PropertyMetadata>( );
                allBags.AddRange( columnInfoBag );
                allBags.AddRange( lazyBag );

                return allBags[i];
            }
        }

        internal EntityMetadata( Type type )
            : this( type , new List<PropertyMetadata>( ) )
        {
            this.type = type;
        }

        internal EntityMetadata( Type type , IList<PropertyMetadata> columnInfoBag )
        {
            this.type = type;
            this.columnInfoBag = columnInfoBag;
        }

        internal string SchemaName
        {
            get;
            set;
        }

        internal bool AlwaysReferesh { get; set; }

        internal bool HasDiscriminator { get; set; }

        internal PropertyMetadata[] Keys
        {
            get
            {
                if ( keys.IsNull( ) )
                {
                    keys = ColumnInfoBag .Where( p => p.IsUniqueIdentifier ).OrderBy( k => k.KeyPriority ).ToArray<PropertyMetadata>( );
                }

                return keys;
            }
        }

        internal PropertyMetadata GetKeyByMapping( string mapping )
        {
            return Keys[GetKeyIndexByMapping( mapping )];
        }

        internal int GetKeyIndexByMapping( string mapping )
        {
            foreach ( int index in Keys.GetIndices( ) )
            {
                if ( Keys[index].MappingName.ToLowerEquals( mapping ) )
                {
                    return index;
                }
            }

            return -1;
        }

        public PropertyMetadata PropertyMapping( PropertyInfo propertyInfo )
        {
            return GetAll( ).Where( p => p.IsMapped( propertyInfo ) ).FirstOrDefault( );
        }

        public EntityMetadata GetPropertyClassMetadata( string property )
        {
            if ( PropertyMapping( property ).IsNotNull( ) )
            {
                return this;
            }

            if ( HasBaseEntity )
            {
                EntityMetadata baseMetadata = MetaDataManager.MetaInfo( BaseEntity ).GetPropertyClassMetadata( property );

                if ( baseMetadata.IsNotNull( ) )
                {
                    return baseMetadata;
                }
            }

            if ( HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    EntityMetadata metadata = MetaDataManager.MetaInfo( type.Type ).GetPropertyClassMetadata( property );

                    if ( metadata.IsNotNull( ) )
                    {
                        return metadata;
                    }
                }
            }

            return this;
        }

        public PropertyMetadata PropertyMapping( string propertyName )
        {
            return GetAll( ).Where( p => p.ClassDefinationName == propertyName ).FirstOrDefault( );
        }

        public PropertyMetadata PropertyMappingByColumnName( string columnName )
        {
            return GetAll( ).Where( p => p.MappingName == columnName ).FirstOrDefault( );
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator( )
        {
            List<PropertyMetadata> allBag = new List<PropertyMetadata>( );

            if ( allBag.Count <= 0 )
            {
                allBag.AddRange( columnInfoBag );
                allBag.AddRange( lazyBag );
                allBag.AddRange( importedBag );
            }

            return allBag.Distinct( ).GetEnumerator( );
        }

        public List<PropertyMetadata> GetAll( )
        {
            List<PropertyMetadata> allBag = new List<PropertyMetadata>( );

            if ( allBag.Count <= 0 )
            {
                allBag.AddRange( columnInfoBag );
                allBag.AddRange( lazyBag );
                allBag.AddRange( importedBag );
                allBag.AddRange( placeholders );
            }

            return allBag;
        }

        public IEnumerable<PropertyMetadata> GetAllIncludeBase( )
        {
            foreach ( var column in GetAll( ) )
            {
                yield return column;
            }

            if ( HasBaseEntity )
            {
                foreach ( var column in MetaDataManager.MetaInfo( BaseEntity ).GetAllIncludeBase( ) )
                    yield return column;
            }

            if ( HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    foreach ( var column in MetaDataManager.MetaInfo( type.Type ).GetAllIncludeBase( ) )
                        yield return column;
                }
            }
        }

        public IEnumerable<PropertyMetadata> GetAllPersistentFieldIncludeBase( )
        {
            foreach ( var column in columnInfoBag )
            {
                yield return column;
            }

            if ( HasBaseEntity )
            {
                foreach ( var column in MetaDataManager.MetaInfo( BaseEntity ).GetAllPersistentFieldIncludeBase( ) )
                    yield return column;
            }

            if ( HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    foreach ( var column in MetaDataManager.MetaInfo( type.Type ).GetAllPersistentFieldIncludeBase( ) )
                        yield return column;
                }
            }
        }

        public IEnumerable<PropertyMetadata> GetAllRelations( )
        {
            foreach ( PropertyMetadata property in GetAll().Where( p => p.IsRelationshipMapping ) )
                yield return property;

            if ( HasBaseEntity )
            {
                foreach ( PropertyMetadata property in MetaDataManager.MetaInfo( inheritanceRelation.RelationType ).GetAllRelations( ) )
                {
                    yield return property;
                }
            }

            if ( HasEmbeddedEntities )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    foreach ( PropertyMetadata property in MetaDataManager.MetaInfo( type.Type ).GetAllRelations( ) )
                    {
                        yield return property;
                    }
                }
            }
        }

        public string GetAutoGenKeyName( )
        {
            PropertyMetadata columnInfo = Keys.Where( c => c.IsAutoGenerated ).FirstOrDefault( );

            if ( columnInfo.IsNotNull( ) )
            {
                return columnInfo.MappingName;
            }

            return string.Empty;
        }

        internal static PropertyMetadata GetProperty( Criteria criteria )
        {
            return GetMappingInfo( criteria.DeclaringType ).PropertyMapping( criteria.Name );
        }

        internal static EntityMetadata GetMappingInfo( string typeName )
        {
            if ( typeName.IsNullOrEmpty( ) )
                return null;

            return MetaDataManager.TypeBag[typeName.Trim( )];
        }

        internal static EntityMetadata GetMappingInfo( Type type )
        {
            return MetaDataManager.PrepareMetadata( type );
        }

        internal static bool ContainsAsField( string fieldName , EntityMetadata mapping )
        {
            foreach ( PropertyMetadata columnInfo in mapping.ColumnInfoBag )
            {
                if ( fieldName.Trim( ) == columnInfo.ClassDefinationName )
                    return true;
            }

            return false;
        }

        internal PropertyMetadata GetOneSideRelationshipWith( Type type )
        {
            foreach ( PropertyInfo property in Type.GetProperties( ) )
            {
                if ( property.DeclaringType == Type )
                {
                    if ( property.PropertyType.Name == type.Name || MetaDataManager.IsSublassOf( property.PropertyType , type ) )
                    {
                        if ( MetaDataManager.IsPersistentable( property.PropertyType ) )
                        {
                            PropertyMetadata column = GetAll( ).Where( c => property.Name == c.ClassDefinationName ).FirstOrDefault( );

                            return column;
                        }
                    }
                }
            }

            return GetOneSideRelationshipWithParent( type );
        }

        internal PropertyMetadata GetOneSideRelationshipWithParent( Type type )
        {
            if ( !HasBaseEntity )
                return null;

            return EntityMetadata.GetMappingInfo( BaseEntity ).GetOneSideRelationshipWith( type );
        }

        internal PropertyMetadata GetPropertyMappingIncludeBase( string name )
        {
            PropertyMetadata property = PropertyMapping( name );

            if ( property.IsNull( ) && HasBaseEntity )
            {
                property = EntityMetadata.GetMappingInfo( BaseEntity ).GetPropertyMappingIncludeBase( name );
            }

            if ( HasEmbeddedEntities && property.IsNull( ) )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    property = EntityMetadata.GetMappingInfo( type.Type ).GetPropertyMappingIncludeBase( name );

                    if ( property.IsNotNull( ) )
                    {
                        return property;
                    }
                }
            }

            return property;
        }

        internal PropertyMetadata GetPropertyMappingIncludeBaseByColumnName( string columnName )
        {
            PropertyMetadata property = PropertyMappingByColumnName( columnName );

            if ( property.IsNull( ) && HasBaseEntity )
            {
                property = EntityMetadata.GetMappingInfo( BaseEntity ).GetPropertyMappingIncludeBaseByColumnName( columnName );
            }

            if ( HasEmbeddedEntities && property.IsNull( ) )
            {
                foreach ( EmbeddedEntity type in EmbeddedTypes )
                {
                    property = EntityMetadata.GetMappingInfo( type.Type ).GetPropertyMappingIncludeBaseByColumnName( columnName );
                    if ( property.IsNotNull( ) )
                    {
                        return property;
                    }
                }
            }

            return property;
        }

        internal PropertyMetadata GetManySideRelationshipWithParent( Type type )
        {
            if ( !HasBaseEntity )
                return null;

            return EntityMetadata.GetMappingInfo( BaseEntity ).GetManySideRelationshipWith( type );
        }

        internal PropertyMetadata GetManySideRelationshipWith( Type type )
        {
            PropertyMetadata column = GetAll( ).Where( c => !c.RelationType.IsNull( ) && ( ( c.RelationType.Name == type.Name || MetaDataManager.IsSublassOf( c.RelationType , type ) ) && ( c.IsManyToMany || c.IsOneToMany ) ) )
                                        .FirstOrDefault( );

            if ( column.IsNull( ) && HasBaseEntity )
            {
                column = GetManySideRelationshipWithParent( type );
            }

            return column;
        }

        #endregion

        #region ICloneable Members

        public object Clone( )
        {
            EntityMetadata entityMetadata = this.MemberwiseClone( ) as EntityMetadata;
            entityMetadata.ColumnInfoBag = new List<PropertyMetadata>( );
            entityMetadata.LazyBag = new List<PropertyMetadata>( );
            entityMetadata.ImportedBag = new List<PropertyMetadata>( );

            foreach ( PropertyMetadata propertyMetadata in this )
            {
                if ( propertyMetadata.IsDiscriminator )
                {
                    entityMetadata.ColumnInfoBag.Add( ( PropertyMetadata ) propertyMetadata.Clone( ) );
                    continue;
                }

                PropertyMetadata newColumnInfo = propertyMetadata.Clone( ) as PropertyMetadata;
                newColumnInfo.FieldValue = null;

                if ( newColumnInfo.IsImported )
                {
                    entityMetadata.ImportedBag.Add( newColumnInfo );
                    continue;
                }

                if ( newColumnInfo.IsManyToMany || newColumnInfo.IsOneToMany )
                {
                    entityMetadata.LazyBag.Add( newColumnInfo );
                    continue;
                }

                entityMetadata.ColumnInfoBag.Add( newColumnInfo );
            }

            return entityMetadata;
        }

        #endregion

        internal PropertyMetadata GetDiscriminatorProperty( )
        {
            return GetAll( ).Where( c => c.IsDiscriminator ).FirstOrDefault( );
        }
    }
}
