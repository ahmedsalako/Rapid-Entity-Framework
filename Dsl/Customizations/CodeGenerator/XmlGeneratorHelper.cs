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
using consist.RapidEntity.CodeGenerator;
using System.Xml;

namespace consist.RapidEntity.Customizations.CodeGenerator
{
    public class XmlGeneratorHelper
    {
        public static void Generate(string baseFolder, string defaultNamespace, ModelRoot modelRoot)
        {
            XmlEntityMapping mapping = new XmlEntityMapping();

            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();

                foreach (ModelType type in modelRoot.Types)
                {
                    if (type is ModelClass)
                    {
                        ModelClass classType = (ModelClass)type;
                        string className = ReformatClassName(classType.Name);

                        String source = FormatName(className) + "." + provider.FileExtension;

                        if (!string.IsNullOrEmpty(className))
                        {
                            using (IndentedTextWriter textWriter = new IndentedTextWriter(new StreamWriter(Path.Combine(baseFolder, source), false), "    "))
                            {
                                provider.GenerateCodeFromCompileUnit(BuildCompileUnit( classType , defaultNamespace , ref mapping ), textWriter , new CodeGeneratorOptions() );
                                textWriter.Close();
                            }

                            XmlDocument xmlDocument = mapping.Serialize();
                            xmlDocument.Save(Path.Combine(baseFolder, FormatName(className) + "-rapid.xml"));
                        }
                    }
                }
            }
            catch (Exception x)
            {

            }
        }

        public static string ReformatClassName(string className)
        {
            return GrammarHelper.PascalCase(GrammarHelper.MakeSingle(className));
        }

        public static string RemovePrefix(string word, string[] prefixes)
        {
            foreach (string prefix in prefixes)
            {
                if (word.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    return word.Remove(0, prefix.Trim().Length);
                }
            }

            return word;
        }

        public static CodeCompileUnit BuildCompileUnit( ModelClass model , string projectNamespace , ref XmlEntityMapping mapping )
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace generatorNamespace = BuildNamespace(projectNamespace, compileUnit);

            CodeTypeDeclaration classType = new CodeTypeDeclaration(ReformatClassName(model.Name));
            classType.IsPartial = true;

            mapping.Assemblyname = projectNamespace;
            mapping.ClassName = classType.Name;

            classType.TypeAttributes = (model.IsAbstract == InheritanceModifier.Abstract) ? System.Reflection.TypeAttributes.Abstract | System.Reflection.TypeAttributes.Public : classType.TypeAttributes;
            generatorNamespace.Types.Add(classType);

            AddEntityLevelAttributes( model , classType , ref mapping );
            mapping.Fields = CreateProperties( model ,  classType ).ToArray();
            mapping.Keys = CreateKey(model, classType).ToArray();
            
            CreateRelationship( model , classType , mapping );

            return compileUnit;
        }

        private static void CreateRelationship( ModelClass model , CodeTypeDeclaration classType , XmlEntityMapping mapping )
        {
            List<OneToOneXml> oneToOne = new List<OneToOneXml>();
            List<OneToManyXml> oneToMany = new List<OneToManyXml>();
            List<ManyToOneXml> manyToOne = new List<ManyToOneXml>();

            foreach ( BaseRelationship relationship in model.GetRelationships() )
            {
                if (relationship is OneToOne && relationship.IsSource(model))
                {
                    string referenceType = ReformatClassName(relationship.ReferenceEntity);
                    string property = CreateProperty(classType, relationship, RelationshipTypeEnum.OneToOne, referenceType);

                    oneToOne.Add(new OneToOneXml { RelationColumn = relationship.ReferenceColumn, JoinColumn = relationship.ReferencedKey, RelationClass = referenceType , PropertyName = property });
                }
                else if (relationship is OneToOne && relationship.IsTarget(model))
                {
                    string referenceType = ReformatClassName(relationship.OwnerEntity);
                    string property = CreateProperty(classType, relationship, RelationshipTypeEnum.OneToOne, referenceType);

                    oneToOne.Add(new OneToOneXml { RelationColumn = relationship.ReferenceColumn, JoinColumn = relationship.ReferencedKey, RelationClass = referenceType, IsImported = true , PropertyName = property });
                }
                else if (relationship is OneToMany && relationship.IsSource(model))
                {
                    string referenceType = ReformatClassName(relationship.ReferenceEntity);
                    string property = CreateProperty(classType, relationship, RelationshipTypeEnum.OneToMany, referenceType);

                    oneToMany.Add(new OneToManyXml { RelationColumn = relationship.ReferenceColumn, JoinColumn = relationship.ReferencedKey, RelationClass = referenceType , PropertyName = property });
                }
                else if (relationship is OneToMany && relationship.IsTarget(model))
                {
                    string referenceType = ReformatClassName(relationship.OwnerEntity);
                    string property = CreateProperty(classType, relationship, RelationshipTypeEnum.ManyToOne, referenceType);

                    manyToOne.Add(new ManyToOneXml { RelationColumn = relationship.ReferenceColumn, JoinColumn = relationship.ReferencedKey, RelationClass = referenceType , PropertyName = property });
                }                
            }

            mapping.ManyToOnes = manyToOne.ToArray();
            mapping.OneToManys = oneToMany.ToArray();
            mapping.OneToOnes = oneToOne.ToArray();
        }

        //Add Entity Level Expression
        private static void AddEntityLevelAttributes( ModelClass model , CodeTypeDeclaration classType , ref XmlEntityMapping mapping )
        {
            if (model.Superclass.IsNull() || model.Superclass.IsAbstract == InheritanceModifier.Abstract)
            {
                if (model.IsAbstract != InheritanceModifier.Abstract)
                {
                    mapping.Name = model.TableName;
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
                classType.BaseTypes.Add(model.Superclass.Name);

                mapping.DiscriminatorValue = new DiscriminatorValueXml
                {
                    Name = "Discriminator" , Value = model.Name
                };
            }
        }

        private static CodeNamespace BuildNamespace( string projectNamespace , CodeCompileUnit compileUnit )
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

        public static void CreateFields(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.Fields.Count > 0)
            {
                foreach (Field field in model.Fields)
                {
                    CreateField(classType, field, RelationshipTypeEnum.NOTSET);
                }
            }
        }

        private static void CreateField(CodeTypeDeclaration classType, BaseRelationship relationship, RelationshipTypeEnum relationshipType, string type, string fieldName)
        {
            CodeMemberField memberField = new CodeMemberField();

            memberField.Name = CamelCasing(fieldName);
            memberField.Type = new CodeTypeReference(type);
            memberField.Attributes = MemberAttributes.Private;

            classType.Members.Add(memberField);
        }

        private static void CreateField(CodeTypeDeclaration classType, ModelAttribute field, RelationshipTypeEnum relationshipType)
        {
            CodeMemberField memberField = new CodeMemberField();
            string fieldName = ReformatNaming(field, classType);

            memberField.Name = CamelCasing(fieldName);
            memberField.Type = new CodeTypeReference(field.Type);
            memberField.Attributes = MemberAttributes.Private;

            classType.Members.Add(memberField);
        }

        public static string CamelCasing(string name)
        {
            return GrammarHelper.CamelCase(name.ToLower());
        }

        public static string PascalCasing(string name)
        {
            return GrammarHelper.PascalCase(name);
        }

        public static IEnumerable<FieldXml> CreateProperties( ModelClass model , CodeTypeDeclaration classType )
        {
            if (model.Fields.Count > 0)
            {
                foreach ( Field field in model.Fields )
                {
                    FieldXml fieldXml = new FieldXml();
                    CreateProperty(classType, field, RelationshipTypeEnum.NOTSET , ref fieldXml );

                    yield return fieldXml;
                }
            }
        }

        private static string CreateProperty(CodeTypeDeclaration classType, BaseRelationship relationship, RelationshipTypeEnum relationshipType, string type)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            string fieldName = ReformatNaming(relationshipType, type, classType.Name);

            property.Name = ResolveNameConflicts(PascalCasing(fieldName), classType.Name);
            property.Attributes = MemberAttributes.Public;
            property.HasGet = true;
            property.HasSet = true;

            type = GetRelationshipType( relationshipType , type );
            property.Type = new CodeTypeReference( type );
            CreateField( classType , relationship , relationshipType , type , fieldName );

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(CamelCasing(fieldName))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CamelCasing(fieldName)), new CodePropertySetValueReferenceExpression()));

            classType.Members.Add( property );

            return property.Name;
        }

        private static void CreateProperty( CodeTypeDeclaration classType , ModelAttribute field , RelationshipTypeEnum relationshipType , ref FieldXml fieldXml )
        {
            CodeMemberProperty property = new CodeMemberProperty();
            string fieldName = ReformatNaming(field, classType);            

            property.Name = ResolveNameConflicts(PascalCasing(fieldName), classType.Name);
            property.Attributes = MemberAttributes.Public;
            property.HasGet = true;
            property.HasSet = true;

            fieldXml.Name = MakeValidName(field.Name);
            fieldXml.PropertyName = property.Name;

            property.Type = new CodeTypeReference(field.Type);
            CreateField(classType, field, RelationshipTypeEnum.NOTSET);

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(CamelCasing(fieldName))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CamelCasing(fieldName)), new CodePropertySetValueReferenceExpression()));

            classType.Members.Add(property);
        }

        private static string ResolveNameConflicts(string name1, string name2)
        {
            if (name2.IsNull())
                return name1;

            if (name1.ToLower() == name2.ToLower())
            {
                return string.Concat(name1, "_");
            }

            return name1;
        }

        private static string ReformatNaming(ModelAttribute field, CodeTypeDeclaration classType)
        {
            return ReformatNaming(field, classType.Name);
        }

        private static string ReformatNaming(RelationshipTypeEnum relationshipType, string classTypeName, string propertyName)
        {
            if (!(relationshipType == RelationshipTypeEnum.OneToOne) && !(relationshipType == RelationshipTypeEnum.ManyToOne))
            {
                return GrammarHelper.MakePlural(classTypeName);
            }
            else
            {
                return ResolveNameConflicts(GrammarHelper.MakeSingle(classTypeName), propertyName);
            }
        }

        private static string ReformatNaming(ModelAttribute field, string classTypeName)
        {
            return ResolveNameConflicts(field.Name, classTypeName);
        }

        private static string GetRelationshipType(RelationshipTypeEnum relationshipType, string name)
        {
            return (relationshipType == RelationshipTypeEnum.OneToMany || relationshipType == RelationshipTypeEnum.ManyToMany) ? string.Format("IList<{0}>", name) : name;
        }

        public static IEnumerable<KeyXml> CreateKey(ModelClass model, CodeTypeDeclaration classType)
        {
            if (model.PersistentKeys.Count > 0)
            {
                foreach (PersistentKey field in model.PersistentKeys)
                {
                    CreateField( classType , field , RelationshipTypeEnum.NOTSET );
                    KeyXml keyXml = new KeyXml();
                    keyXml.Name = MakeValidName(field.Name);
                    keyXml.AutoKey = field.IsAutoKey;

                    CreateKey( classType , field , keyXml );

                    yield return keyXml;
                }
            }
        }

        private static void CreateKey(CodeTypeDeclaration classType, PersistentKey field, KeyXml keyXml )
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.Name = PascalCasing(field.Name);
            property.Type = new CodeTypeReference(field.Type);
            property.Attributes = MemberAttributes.Public;
            property.HasGet = true;
            property.HasSet = true;
            keyXml.PropertyName = property.Name;

            property.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(CamelCasing(field.Name))));
            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), CamelCasing(field.Name)), new CodePropertySetValueReferenceExpression()));

            classType.Members.Add(property);
        }
    }
}
