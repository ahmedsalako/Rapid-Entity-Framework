using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using consist.RapidEntity.Customizations.Descriptors;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity.Customizations.CodeGenerator
{
    public static class CodeParser
    {
        const string Name = "Name";
        const string Entity = "Entity";
        const string Table = "Table";
        const string IsUnique = "IsUnique";
        const string Key = "Key";
        const string AutoKey = "AutoKey";
        const string AllowNullValue = "AllowNullValue";
        const string Field = "Field";
        const string OneToOne_Attribute = "OneToOne";
        const string OneToMany_Attribute = "OneToMany";
        const string ManyToMany_Attribute = "ManyToMany";
        const string ManyToOne_Attribute = "ManyToOne";
        const string RelationColumn = "RelationColumn";
        const string JoinColumn = "JoinColumn";
        const string Type_Name = "Type";

        [CLSCompliant( true )]
        public static bool IsPersistentClass( CodeClass codeClass )
        {
            return !GetClassLevelAttribute( codeClass ).IsNull( );
        }

        public static CodeAttribute GetClassLevelAttribute( CodeClass codeClass )
        {
            foreach ( CodeElement attribute in codeClass.Attributes )
            {
                CodeAttribute codeAttribute = ( CodeAttribute ) attribute;
                string name = codeAttribute.FullName;

                if ( name.Contains( Entity ) || name.Contains( Table ) )
                    return codeAttribute;
            }

            return null;
        }

        public static EntityMapping GetClassName( CodeClass codeClass )
        {
            CodeAttribute codeAttribute = GetClassLevelAttribute( codeClass );
            EntityMapping mapping = new EntityMapping( );
            string name = GetAttributeValue( codeAttribute , Name , 1 );

            mapping.DataStoreName = name ?? codeClass.Name;
            mapping.EntityName = codeClass.Name;

            return mapping;
        }

        public static string GetAttributeValue( CodeAttribute codeAttribute , string name , int index )
        {
            if ( codeAttribute.IsNull( ) )
                return null;

            string[] values = NameValueScanner.SplitSpecial( codeAttribute.Value , ',' , true );
            return NameValueScanner.GetValueByName( name , values , index );
        }

        public static CodeAttribute GetKeyAttribute( CodeProperty codeProperty )
        {
            foreach ( CodeElement attribute in codeProperty.Attributes )
            {
                CodeAttribute codeAttribute = ( CodeAttribute ) attribute;
                string name = codeAttribute.FullName;
                bool? isUnique = false;

                if ( codeAttribute.Name == Field || codeAttribute.Name == Key )
                {
                    if ( codeAttribute.Name == Field )
                    {
                        string value = GetAttributeValue( codeAttribute , IsUnique , 2 );
                        isUnique = ( value.IsNull( ) ) ? false : bool.Parse( value );
                    }

                    if ( name.Contains( Key ) || ( codeAttribute.Name == Field ) && isUnique.Value )
                        return codeAttribute;
                }
            }

            return null;
        }

        public static CodeAttribute GetFieldAttribute( CodeProperty codeProperty )
        {
            foreach ( CodeElement attribute in codeProperty.Attributes )
            {
                CodeAttribute codeAttribute = ( CodeAttribute ) attribute;
                string name = codeAttribute.FullName;

                if ( name.Contains( Field ) || GetAttributeValue( codeAttribute , IsUnique , 2 ).IsNull( ) )
                    return codeAttribute;
            }

            return null;
        }

        public static CodeAttribute GetRelationshipAttribute( CodeProperty codeProperty )
        {
            foreach ( CodeElement attribute in codeProperty.Attributes )
            {
                CodeAttribute codeAttribute = ( CodeAttribute ) attribute;
                string name = codeAttribute.FullName;

                if ( name.Contains( OneToOne_Attribute ) || name.Contains( OneToMany_Attribute )
                    || name.Contains( ManyToOne_Attribute ) || name.Contains( ManyToMany_Attribute ) )
                    return codeAttribute;
            }

            return null;
        }

        public static bool HasKeyAttribute( CodeProperty codeProperty )
        {
            return !GetKeyAttribute( codeProperty ).IsNull( );
        }

        public static bool HasFieldAttribute( CodeProperty codeProperty )
        {
            return !GetFieldAttribute( codeProperty ).IsNull( );
        }

        public static bool HasRelationshipAttribute( CodeProperty codeProperty )
        {
            return !GetRelationshipAttribute( codeProperty ).IsNull( );
        }

        public static void AddAsParent( CodeClass codeClass , ModelClass parent , ClassDiagram classDiagram )
        {
            foreach ( ModelClass child in classDiagram.Store.GetAllModelClass( ).Where( m => m.ParentName == parent.Name ).ToList( ) )
            {
                child.Superclass = parent;
            }
        }

        public static void AddAsChild( CodeClass codeClass , ModelClass child , ClassDiagram classDiagram )
        {
            foreach ( CodeElement parentCode in codeClass.Bases )
            {
                if ( parentCode is CodeClass )
                {
                    ModelClass parent = classDiagram.Store.GetModelClassByName( parentCode.Name );

                    if ( parent.IsNotNull( ) )
                    {
                        child.Superclass = parent;
                    }
                    else
                    {
                        child.ParentName = parentCode.Name;
                    }
                }
            }
        }

        public static CodeElement GetNamespaceDeclaration( CodeElements codeElements )
        {
            foreach ( CodeElement codeElement in codeElements )
            {
                if ( codeElement.Kind == vsCMElement.vsCMElementNamespace )
                {
                    return codeElement;
                }
            }

            throw new InvalidOperationException( "An entity must be defined within a namespace!" );
        }

        public static void ExamineProjectItem( ProjectItem codeItem , ClassDiagram classDiagram )
        {
            FileCodeModel fileCodeModel = codeItem.FileCodeModel;

            foreach ( CodeElement codeElement in GetNamespaceDeclaration( fileCodeModel.CodeElements ).Children )
            {
                if ( codeElement.Kind == vsCMElement.vsCMElementClass )
                {
                    using ( Transaction addTransaction = classDiagram.Store.TransactionManager.BeginTransaction( "Add class" ) )
                    {
                        CodeClass codeClass = ( CodeClass ) codeElement;
                        ModelClass model = new ModelClass( classDiagram.Store );

                        ModelClass current = GetCurrentElements( classDiagram.Store )
                                                .Where( m => m.Name == codeClass.Name ).FirstOrDefault( );

                        if ( codeClass.IsAbstract )
                        {
                            model.IsAbstract = InheritanceModifier.Abstract;
                        }

                        if ( !current.IsNull( ) )
                            return;

                        if ( IsPersistentClass( codeClass ) )
                        {
                            model.Name = codeClass.Name;
                            model.TableName = GetClassName( codeClass ).DataStoreName;
                            ProcessPersistentProperty( codeClass , classDiagram , model );
                        }
                        else
                        {
                            ProcessPersistentProperty( codeClass , classDiagram , model );
                            model.Name = model.TableName = codeClass.Name;
                        }

                        model.ModelRoot = classDiagram.GetModelRoot( );
                        AddAsParent( codeClass , model , classDiagram );
                        AddAsChild( codeClass , model , classDiagram );
                        addTransaction.Commit( );
                    }
                }
            }
        }

        public static void ProcessPersistentProperty( CodeClass codeClass , ClassDiagram classDiagram , ModelClass modelClass )
        {
            foreach ( CodeElement classMemberElement in codeClass.Members )
            {
                if ( classMemberElement.Kind == vsCMElement.vsCMElementProperty )
                {
                    CodeProperty codeProperty = ( CodeProperty ) classMemberElement;
                    CodeFunction codeFunction = codeProperty.Getter;
                    CodeTypeRef codeFunctionType = codeFunction.Type;

                    string propertyType = codeFunctionType.AsFullName;

                    if ( codeFunction.Access != vsCMAccess.vsCMAccessPrivate )
                    {
                        AddFields( codeProperty , codeFunctionType , modelClass , classDiagram );
                    }
                }
            }
        }

        public static void AddFields( CodeProperty codeProperty , CodeTypeRef codeFunctionType , ModelClass modelClass , ClassDiagram classDiagram )
        {
            CodeAttribute codeAttribute = null;

            if ( HasKeyAttribute( codeProperty ) )
            {
                codeAttribute = GetKeyAttribute( codeProperty );
                AddToKey( codeProperty , codeFunctionType , modelClass , codeAttribute );
            }
            else if ( HasFieldAttribute( codeProperty ) )
            {
                codeAttribute = GetFieldAttribute( codeProperty );
                AddToField( codeProperty , codeFunctionType , modelClass , codeAttribute );
            }
            else if ( HasRelationshipAttribute( codeProperty ) )
            {
                codeAttribute = GetRelationshipAttribute( codeProperty );
                string RelationshipType = codeAttribute.Name;
                string[] parameters = NameValueScanner.SplitSpecial( codeAttribute.Value , ',' , true );

                string ReferencedTypeName = NameValueScanner.GetTypeName( NameValueScanner.GetValueByName( Type_Name , parameters , 1 ) );
                string RelationColumnName = NameValueScanner.GetValueByName( RelationColumn , parameters , 2 );
                string JoinColumnName = NameValueScanner.GetValueByName( JoinColumn , parameters , 0 ); // ?? RelationColumnName;

                AddRelationFromOwningSide( modelClass , ReferencedTypeName , RelationshipType , RelationColumnName , JoinColumnName );
                AddRelationFromReferenceSide( modelClass , ReferencedTypeName , RelationColumnName , JoinColumnName , string.Empty );
            }
        }

        private static void AddToField( CodeProperty codeProperty , CodeTypeRef codeFunctionType , ModelClass modelClass , CodeAttribute codeAttribute )
        {
            Field field = new Field( modelClass.Store );
            field.AllowNull = GetAttributeValue( codeAttribute , AllowNullValue , 3 ).IsNull( ) ? true : false;
            field.ColumnName = GetAttributeValue( codeAttribute , Name , 1 ) ?? codeProperty.Name;
            field.Name = codeProperty.Name;
            field.Type = codeFunctionType.AsFullName.OverrideNullOrEmpty( codeFunctionType.AsString.Replace("@byte" , "System.Byte" ) );

            modelClass.Fields.Add( ( Field ) field );
        }

        private static void AddToKey( CodeProperty codeProperty , CodeTypeRef codeFunctionType , ModelClass modelClass , CodeAttribute codeAttribute )
        {
            PersistentKey key = new PersistentKey( modelClass.Store );
            key.AllowNull = false;
            key.ColumnName = GetAttributeValue( codeAttribute , Name , 1 ) ?? codeProperty.Name;
            key.Name = codeProperty.Name;
            key.IsAutoKey = !GetAttributeValue( codeAttribute , AutoKey , 2 ).IsNull( );
            key.Type = codeFunctionType.AsFullName.OverrideNullOrEmpty(codeFunctionType.AsString.Replace("@byte", "System.Byte"));

            modelClass.PersistentKeys.Add( key );
        }

        private static IEnumerable<ModelClass> GetCurrentElements( Store store )
        {
            foreach ( ModelElement element in store.ElementDirectory.AllElements )
                if ( element is ModelClass )
                    yield return ( ModelClass ) element;
        }

        private static void AddOneToManyJoin( ModelClass model , string referencedName , string relationColumn , string joinColumn , string languageDataType )
        {
            ModelClass referencedEntity = GetCurrentElements( model.Store )
                .Where( m => m.Name == referencedName ).FirstOrDefault( );

            if ( referencedEntity.IsNull( ) )
                return;

            OneToMany oneToMany = new OneToMany( model , referencedEntity );
            oneToMany.OwnerEntity = model.Name;
            oneToMany.RelationColumn = relationColumn;
            oneToMany.ReferenceColumn = relationColumn;
            oneToMany.ReferenceEntity = referencedName;
            oneToMany.ReferencedKey = joinColumn ?? model.PersistentKeys[0].ColumnName;
            oneToMany.Type = languageDataType;
        }

        private static void AddSelfJoin( ModelClass model , ModelClass target , string relationColumn , string joinColumn )
        {
            PersistentKey key = ( PersistentKey ) model.GetPropertiesIncludeInheritance( ).Where( p => p is PersistentKey ).FirstOrDefault( );

            OneToOne oneToOne = new OneToOne( model , target );
            oneToOne.RelationColumn = relationColumn;

            oneToOne.ReferenceEntity = target.Name;
            oneToOne.ReferenceColumn = relationColumn;
            oneToOne.OwnerEntity = model.Name;
            oneToOne.ReferencedKey = joinColumn ?? key.ColumnName;
        }

        private static IEnumerable<ModelClass> GetReferences( string name , ModelClass model )
        {
            return from modelClass in GetCurrentElements( model.Store )
                   where modelClass.Name == name
                   select modelClass;
        }

        private static void AddRelationFromOwningSide( ModelClass model , string targetName , string RelationshipType , string relationColumn , string joinColumn )
        {
            using ( Transaction addReferencesTransaction = model.Store.TransactionManager.BeginTransaction( "Add References" ) )
            {
                List<ModelClass> references = GetReferences( targetName , model ).ToList<ModelClass>( );

                if ( references.Count( ) > 0 )
                {
                    foreach ( ModelClass modelClass in references )
                    {
                        if ( modelClass.Name != model.Name && model.GetRelationshipBySource( modelClass.Name ).IsNull( ) )
                        {
                            RelationshipTypeEnum relationshipType = RelationshipTypeEnum.NOTSET.GetByName( RelationshipType );

                            string languageDataType = string.Empty;
                            ModelClass parent = ( RelationshipType == OneToMany_Attribute ) ? model : modelClass;

                            joinColumn = joinColumn ?? parent.PersistentKeys[0].ColumnName;
                            PersistentKey key = parent.GetPersistentKeyIncludeInheritanceByColumnName( joinColumn );
                            languageDataType = ( key.IsNotNull( ) ) ? key.Type : string.Empty;

                            if ( relationshipType == RelationshipTypeEnum.ManyToOne )
                            {
                                AddOneToManyJoin( modelClass , model.Name , relationColumn , joinColumn , languageDataType );
                            }
                            else if( relationshipType == RelationshipTypeEnum.OneToMany )
                            {
                                AddOneToManyJoin( model , modelClass.Name , relationColumn , joinColumn , languageDataType );
                            }
                        }
                    }
                }

                addReferencesTransaction.Commit( );
            }
        }

        private static void AddRelationFromReferenceSide( ModelClass model , string referencedName , string relationColumn , string joinColumn , string languageDataType )
        {
            ModelClass referencedEntity = GetCurrentElements( model.Store )
                        .Where( m => m.Name == referencedName ).FirstOrDefault( );

            if ( referencedEntity.IsNull( ) )
                return;

            BaseRelationship relationship = model.GetRelationship( referencedEntity.Name.Trim( ) );

            if ( relationship.IsNull( ) )
            {
                if ( referencedEntity.TableName == model.TableName )
                {//Self Join
                    AddSelfJoin( model , referencedEntity , relationColumn , joinColumn );
                }
                else if ( relationship is OneToOne )
                {
                    joinColumn = joinColumn ?? model.PersistentKeys[0].ColumnName;
                    AddOneToOneJoin( model , referencedEntity , relationColumn , joinColumn );
                }
                else
                {
                    AddOneToManyJoin( model , referencedName , relationColumn , joinColumn , languageDataType  );
                }
            }
        }

        private static void AddOneToOneJoin( ModelClass model , ModelClass referencedEntity , string relationColumn , string joinColumn )
        {
            using ( Transaction transaction = model.Store.TransactionManager.BeginTransaction( "Add One To One" ) )
            {
                RelationshipTypeEnum relationshipType = RelationshipTypeEnum.NOTSET.GetByName( OneToOne_Attribute );

                string languageDataType = string.Empty;

                OneToOne oneToOne = new OneToOne( model , referencedEntity );
                oneToOne.RelationColumn = relationColumn;
                oneToOne.OwnerEntity = model.Name;
                oneToOne.ReferenceEntity = referencedEntity.Name;
                oneToOne.Type = languageDataType;
                oneToOne.ReferencedKey = joinColumn;

                transaction.Commit( );
            }
        }
    }
}
