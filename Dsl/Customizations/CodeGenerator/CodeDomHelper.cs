using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using consist.RapidEntity;
using consist.RapidEntity.Customizations.CodeGenerator;
using System.Globalization;

namespace consist.RapidEntity.CodeGenerator
{
    public static class CodeDomHelper
    {
        public static void Generate(string baseFolder, string defaultNamespace, ModelRoot modelRoot)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();

                foreach (ModelType type in modelRoot.Types)
                {
                    if (type is ModelClass)
                    {
                        ModelClass classType = (ModelClass)type;
                        string className = ReformatClassName( classType.Name );

                        String source = FormatName(className) + "." + provider.FileExtension;

                        if (!string.IsNullOrEmpty(className))
                        {
                            using (IndentedTextWriter textWriter = new IndentedTextWriter(new StreamWriter(Path.Combine(baseFolder, source), false), "    "))
                            {
                                provider.GenerateCodeFromCompileUnit(BuildCompileUnit(classType, defaultNamespace), textWriter, new CodeGeneratorOptions());
                                textWriter.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {

            }
        }

        public static void GenerateSQLCode(string baseFolder, string filename, StringBuilder script)
        {
            using (IndentedTextWriter textWriter = new IndentedTextWriter(new StreamWriter(Path.Combine(baseFolder, filename + ".sql"), false), "    "))
            {
                textWriter.WriteLine(script.ToString());
                textWriter.Close();
            }
        }

        public static string ReformatClassName( string className )
        {
            return GrammarHelper.PascalCase( GrammarHelper.MakeSingle( className ) );
        }

        public static string RemovePrefix( string word , string[] prefixes )
        {
            foreach ( string prefix in prefixes )
            {
                if ( word.StartsWith( prefix , StringComparison.CurrentCultureIgnoreCase ) )
                {
                    return word.Remove( 0 , prefix.Trim( ).Length );
                }
            }

            return word;
        }

        public static CodeCompileUnit BuildCompileUnit(ModelClass model, string projectNamespace)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace generatorNamespace = BuildNamespace(projectNamespace, compileUnit);

            CodeTypeDeclaration classType = new CodeTypeDeclaration( ReformatClassName(model.Name) );
            classType.IsPartial = true;

            classType.TypeAttributes = (model.IsAbstract == InheritanceModifier.Abstract) ? System.Reflection.TypeAttributes.Abstract | System.Reflection.TypeAttributes.Public : classType.TypeAttributes;
            generatorNamespace.Types.Add(classType);

            AddEntityLevelAttributes(model, classType);
            CreateProperties(model, classType);
            CreateKey(model, classType);
            CreateRelationship(model, classType);
            
            return compileUnit;
        }

        private static void CreateRelationship( ModelClass model , CodeTypeDeclaration classType )
        {
            foreach ( BaseRelationship relationship in model.GetRelationships() )
            {
                RelationshipTypeEnum relationshipType = RelationshipTypeEnum.NOTSET;
                CodeAttributeArgument argument = null;
                string referenceType = string.Empty;

                CodeAttributeArgument argument2 = CreateAttributeArgument( string.Format( "RelationColumn = \"{0}\"" , relationship.ReferenceColumn ) );
                CodeAttributeArgument argument3 = CreateAttributeArgument( string.Format( "JoinColumn = \"{0}\"" , relationship.ReferencedKey ) );
                CodeAttributeArgument argument4 = CreateAttributeArgument( string.Format( "IsImported = true" ) );

                CodeAttributeDeclaration attribute = null;

                if ( relationship is OneToOne && relationship.IsSource( model ) )
                {
                    referenceType = ReformatClassName( relationship.ReferenceEntity );
                    argument = CreateAttributeArgument(string.Format("typeof({0})", referenceType ));
                    attribute = CreateAttributeDeclaration( "OneToOne" , argument , argument2 , argument3 );
                    relationshipType = RelationshipTypeEnum.OneToOne;
                }
                else if ( relationship is OneToOne && relationship.IsTarget( model ) )
                {
                    referenceType = ReformatClassName( relationship.OwnerEntity );
                    argument = CreateAttributeArgument( string.Format( "typeof({0})" , referenceType ) );
                    attribute = CreateAttributeDeclaration( "OneToOne" , argument , argument2 , argument4 );
                    relationshipType = RelationshipTypeEnum.OneToOne;
                }
                else if ( relationship is OneToMany && relationship.IsSource( model ) )
                {
                    referenceType = ReformatClassName( relationship.ReferenceEntity );
                    argument = CreateAttributeArgument( string.Format( "typeof({0})" , referenceType ) );
                    attribute = CreateAttributeDeclaration( "OneToMany" , argument , argument2 , argument3 );
                    relationshipType = RelationshipTypeEnum.OneToMany;
                }
                else if ( relationship is OneToMany && relationship.IsTarget( model ) )
                {
                    referenceType = ReformatClassName( relationship.OwnerEntity );
                    argument = CreateAttributeArgument( string.Format( "typeof({0})" , referenceType ) );
                    attribute = CreateAttributeDeclaration( "ManyToOne" , argument , argument2 , argument3 );
                    relationshipType = RelationshipTypeEnum.ManyToOne;
                }

                CreateProperty( classType , relationship , attribute , relationshipType , referenceType );
            }
        }

        //Add Entity Level Expression
        private static void AddEntityLevelAttributes(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.Superclass.IsNull() || model.Superclass.IsAbstract == InheritanceModifier.Abstract)
            {
                if (model.IsAbstract != InheritanceModifier.Abstract)
                {
                    CodeAttributeArgument attributeArg = CreateAttributeArgument(string.Format("\"{0}\"", MakeValidName(model.TableName)));
                    CodeAttributeDeclaration attribute = CreateAttributeDeclaration("Entity", attributeArg);
                    classType.CustomAttributes.Add(attribute);
                }

                if (model.Superclass.IsNull())
                {
                    return;
                }
                else if (model.Superclass.IsAbstract == InheritanceModifier.Abstract)
                {
                    classType.BaseTypes.Add(model.Superclass.Name);
                }
            }
            else if (model.Superclass.IsNotNull())
            {
                CodeAttributeArgument discriminatorName = CreateAttributeArgument(string.Format("\"{0}\"", "Discriminator")); 
                CodeAttributeArgument disciminatorValue = CreateAttributeArgument(string.Format("\"{0}\"", model.Name));

                CodeAttributeDeclaration attribute = CreateAttributeDeclaration("DiscriminatorValue", discriminatorName, disciminatorValue);
                classType.CustomAttributes.Add(attribute);
                classType.BaseTypes.Add(model.Superclass.Name);
            }            
        }

        private static CodeAttributeDeclaration CreateAttributeDeclaration(string attributeName, 
            params CodeAttributeArgument[] attributeArg)
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration( attributeName , attributeArg );
            return attribute;
        }

        private static CodeAttributeArgument CreateAttributeArgument(string expression)
        {
            return new CodeAttributeArgument(new CodeSnippetExpression(expression));
        }

        private static CodeNamespace BuildNamespace(string projectNamespace, CodeCompileUnit compileUnit)
        {
            CodeNamespace generatorNamespace = new CodeNamespace(projectNamespace);
            compileUnit.Namespaces.Add(generatorNamespace);
            generatorNamespace.Imports.Add(new CodeNamespaceImport("System"));
            generatorNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            generatorNamespace.Imports.Add(new CodeNamespaceImport("PersistentManager.Mapping"));
            generatorNamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            return generatorNamespace;
        }

        private static string FormatName(string name)
        {
            return name;
        }

        private static string MakeValidName(string typeName)
        {
            return typeName;
            //return Regex.Replace(typeName, "[^a-zA-Z]", "");
        }

        public static void CreateFields( ModelClass model , CodeTypeDeclaration classType )
        {
            if ( model.Fields.Count > 0 )
            {
                foreach ( Field field in model.Fields )
                {
                    CreateField( classType , field , RelationshipTypeEnum.NOTSET );
                }
            }
        }

        private static void CreateField( CodeTypeDeclaration classType , BaseRelationship relationship , RelationshipTypeEnum relationshipType , string type , string fieldName )
        {
            CodeMemberField memberField = new CodeMemberField( );

            memberField.Name = CamelCasing( fieldName );
            memberField.Type = new CodeTypeReference(type);
            memberField.Attributes = MemberAttributes.Private;

            classType.Members.Add( memberField );
        }

        private static void CreateField( CodeTypeDeclaration classType , ModelAttribute field, RelationshipTypeEnum relationshipType)
        {
            CodeMemberField memberField = new CodeMemberField( );
            string fieldName = ReformatNaming( field , classType );

            memberField.Name = CamelCasing( fieldName );
            memberField.Type = new CodeTypeReference( field.Type );
            memberField.Attributes = MemberAttributes.Private;

            classType.Members.Add(memberField);
        }

        public static string CamelCasing(string name)
        {
            return GrammarHelper.CamelCase( name.ToLower() );
        }

        public static string PascalCasing(string name)
        {
            return GrammarHelper.PascalCase( name );
        }

        public static void CreateProperties(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.Fields.Count > 0)
            {
                foreach (Field field in model.Fields)
                {
                    CodeAttributeArgument argument = CreateAttributeArgument(string.Format("\"{0}\"", MakeValidName(field.Name)));
                    CodeAttributeDeclaration attribute = CreateAttributeDeclaration("Field", argument);

                    CreateProperty(classType, field, attribute, RelationshipTypeEnum.NOTSET);
                }
            }
        }

        private static void CreateProperty( CodeTypeDeclaration classType , BaseRelationship relationship , CodeAttributeDeclaration attribute , RelationshipTypeEnum relationshipType , string type )
        {
            CodeMemberProperty property = new CodeMemberProperty( );
            string fieldName = ReformatNaming( relationshipType , type , classType.Name );

            property.Name = ResolveNameConflicts( PascalCasing( fieldName ) , classType.Name );
            property.Attributes = MemberAttributes.Public;
            property.CustomAttributes.Add( attribute );
            property.HasGet = true;
            property.HasSet = true;

            type = GetRelationshipType( relationshipType , type );
            property.Type = new CodeTypeReference( type );
            CreateField( classType , relationship , relationshipType , type , fieldName );

            property.GetStatements.Add( new CodeMethodReturnStatement( new CodeSnippetExpression( CamelCasing( fieldName ) ) ) );
            property.SetStatements.Add( new CodeAssignStatement( new CodeFieldReferenceExpression( new CodeThisReferenceExpression( ) , CamelCasing( fieldName ) ) , new CodePropertySetValueReferenceExpression( ) ) );

            classType.Members.Add( property );
        }

        private static void CreateProperty( CodeTypeDeclaration classType , ModelAttribute field , CodeAttributeDeclaration attribute , RelationshipTypeEnum relationshipType )
        {
            CodeMemberProperty property = new CodeMemberProperty( );
            string fieldName = ReformatNaming( field , classType );

            property.Name = ResolveNameConflicts( PascalCasing( fieldName ) , classType.Name );
            property.Attributes = MemberAttributes.Public;
            property.CustomAttributes.Add( attribute );
            property.HasGet = true;
            property.HasSet = true;

            property.Type = new CodeTypeReference( field.Type );
            CreateField( classType , field , RelationshipTypeEnum.NOTSET );

            property.GetStatements.Add( new CodeMethodReturnStatement( new CodeSnippetExpression( CamelCasing( fieldName ) ) ) );
            property.SetStatements.Add( new CodeAssignStatement( new CodeFieldReferenceExpression( new CodeThisReferenceExpression( ) , CamelCasing( fieldName ) ) , new CodePropertySetValueReferenceExpression( ) ) );

            classType.Members.Add( property );
        }

        private static string ResolveNameConflicts( string name1 , string name2 )
        {
            if ( name2.IsNull( ) )
                return name1;

            if ( name1.ToLower( ) == name2.ToLower( ) )
            {
                return string.Concat( name1 , "_" );
            }

            return name1;
        }

        private static string ReformatNaming(ModelAttribute field, CodeTypeDeclaration classType)
        {
            return ReformatNaming(field, classType.Name);
        }

        private static string ReformatNaming( RelationshipTypeEnum relationshipType , string classTypeName , string propertyName )
        {
            if ( !( relationshipType == RelationshipTypeEnum.OneToOne ) && !( relationshipType == RelationshipTypeEnum.ManyToOne ) )
            {
                return GrammarHelper.MakePlural( classTypeName );
            }
            else
            {
                return ResolveNameConflicts( GrammarHelper.MakeSingle( classTypeName ) , propertyName );
            }
        }

        private static string ReformatNaming(ModelAttribute field, string classTypeName)
        {
            return ResolveNameConflicts(field.Name, classTypeName);
        }

        private static string GetRelationshipType( RelationshipTypeEnum relationshipType , string name )
        {
            return ( relationshipType == RelationshipTypeEnum.OneToMany || relationshipType == RelationshipTypeEnum.ManyToMany ) ? string.Format( "IList<{0}>" , name ) : name;
        }

        public static void CreateKey(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.PersistentKeys.Count > 0)
            {
                foreach (PersistentKey field in model.PersistentKeys)
                {
                    CreateField(classType, field, RelationshipTypeEnum.NOTSET);

                    CodeAttributeArgument argument = CreateAttributeArgument(string.Format("\"{0}\"", MakeValidName(field.Name)));
                    CodeAttributeArgument argument2 = CreateAttributeArgument(string.Format("AutoKey={0}", field.IsAutoKey.ToString().ToLower()));

                    CreateKey(classType, field, argument, argument2);
                }
            }
        }

        private static void CreateKey(CodeTypeDeclaration classType, PersistentKey field, CodeAttributeArgument argument, CodeAttributeArgument argument2)
        {
            CodeAttributeDeclaration attribute = CreateAttributeDeclaration("Key", argument, argument2);

            CodeMemberProperty property = new CodeMemberProperty();
            property.Name = PascalCasing(field.Name);
            property.Type = new CodeTypeReference(field.Type);
            property.Attributes = MemberAttributes.Public;
            property.CustomAttributes.Add(attribute);

            property.HasGet = true;
            property.HasSet = true;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(CamelCasing(field.Name))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CamelCasing(field.Name)), new CodePropertySetValueReferenceExpression()));

            classType.Members.Add(property);
        }

        public static void CreateCompositeKeys(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.PersistentKeys.Count > 1)
            {
                StringBuilder compositeNames = new StringBuilder();
                compositeNames.Append("KeyName = \"");

                for (int i = 0; i < model.PersistentKeys.Count; i++)
                {
                    PersistentKey field = model.PersistentKeys[i];
                    //CreateField(classType, field);

                    if (i == 0)
                        compositeNames.AppendFormat("{0}", MakeValidName(field.ColumnName));
                    else
                        compositeNames.AppendFormat(",{0}", MakeValidName(field.ColumnName));
                }

                compositeNames.Append("\"");

                CodeAttributeArgument argument = CreateAttributeArgument(compositeNames.ToString());

                StringBuilder compositeTypes = new StringBuilder();
                compositeTypes.Append("Types = new[]{");

                for (int i = 0; i < model.PersistentKeys.Count; i++)
                {
                    PersistentKey field = model.PersistentKeys[i];

                    if (i == 0)
                        compositeTypes.AppendFormat("typeof({0})", field.Type);
                    else
                        compositeTypes.AppendFormat(", typeof({0})", field.Type);
                }

                compositeTypes.Append("}");

                CodeAttributeArgument argument2 = CreateAttributeArgument(compositeTypes.ToString());

                CodeAttributeDeclaration attribute = CreateAttributeDeclaration("CompositeKey", argument, argument2);
                classType.CustomAttributes.Add(attribute);
            }
        }
    }
}
