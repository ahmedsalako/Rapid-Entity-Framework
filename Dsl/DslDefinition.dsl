<?xml version="1.0" encoding="utf-8"?>
<Dsl dslVersion="1.0.0.0" Id="cf321742-a8ae-462e-af13-e524570634a6" Description="Description for consist.RapidEntity.RapidEntity" Name="RapidEntity" DisplayName="Rapid Entity Diagrams" Namespace="consist.RapidEntity" ProductName="RapidEntity" CompanyName="consist" PackageGuid="ae24ea31-9883-4087-9e20-23e7b5a79ba3" PackageNamespace="" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="ca28a2a1-63d2-4ee9-acfe-4fa15bfc8599" Description="" Name="NamedElement" DisplayName="Named Element" InheritanceModifier="Abstract" Namespace="consist.RapidEntity">
      <Properties>
        <DomainProperty Id="cc49aa62-6084-449b-927c-598f301e6269" Description="" Name="Name" DisplayName="Name" DefaultValue="" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="03788f77-178f-413d-b59b-e2c145f69dd6" Description="" Name="ModelRoot" DisplayName="Model Root" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="ModelType" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ModelRootHasTypes.Types</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="bbb854cf-7915-43d6-8727-a8e102e4e04c" Description="" Name="ModelClass" DisplayName="Model Class" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="ModelType" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="cef9a7e8-3d55-43cc-8fd7-ea83e3b18ed4" Description="" Name="Kind" DisplayName="Kind" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7fbb3157-c4f6-41d5-a883-5baff596519d" Description="" Name="IsAbstract" DisplayName="Is Abstract" DefaultValue="None">
          <Type>
            <DomainEnumerationMoniker Name="InheritanceModifier" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="c5d528d1-7949-42ec-b473-af769271611a" Description="Description for consist.RapidEntity.ModelClass.Table Name" Name="TableName" DisplayName="Table Name">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e176246c-ecee-4ff4-a737-5c3c6d2f2883" Description="Description for consist.RapidEntity.ModelClass.Parent Name" Name="ParentName" DisplayName="Parent Name" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Field" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasFields.Fields</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="PersistentKey" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>ClassHasPersistentKeys.PersistentKeys</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="c8209cc1-12fb-48f0-b1bc-b3abb54802d1" Description="An attribute of a class." Name="Field" DisplayName="Field" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="ModelAttribute" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="743fe339-dc13-44bb-ace7-060dd9db7c0f" Description="" Name="Comment" DisplayName="Comment" Namespace="consist.RapidEntity">
      <Properties>
        <DomainProperty Id="97c14f94-78fb-4424-a5e5-50d4b29a17e6" Description="" Name="Text" DisplayName="Text" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="8aa4f57a-f10f-45a0-b093-3705ebb61dce" Description="" Name="ModelType" DisplayName="Model Type" InheritanceModifier="Abstract" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="Comment" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>CommentReferencesSubjects.Comments</DomainPath>
            <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot/ModelRootHasComments.Comments</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="a71b24b4-525c-4caf-9f02-267c9247d388" Description="Element with a Description" Name="ClassModelElement" DisplayName="Class Model Element" InheritanceModifier="Abstract" Namespace="consist.RapidEntity">
      <Notes>Abstract base of all elements that have a Description property.</Notes>
      <BaseClass>
        <DomainClassMoniker Name="NamedElement" />
      </BaseClass>
    </DomainClass>
    <DomainClass Id="718418c3-17c9-4830-abab-7e1cf343b4bb" Description="Description for consist.RapidEntity.PersistentKey" Name="PersistentKey" DisplayName="Persistent Key" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="ModelAttribute" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="95762e74-97b4-4aa0-994d-f074d4039c4b" Description="Description for consist.RapidEntity.PersistentKey.Is Auto Key" Name="IsAutoKey" DisplayName="Is Auto Key">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(consist.RapidEntity.Customizations.EntityProperties.EntityPropertyEditor), typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
    <DomainClass Id="9f85f547-85da-445d-9722-b1d72e5723de" Description="Description for consist.RapidEntity.ModelAttribute" Name="ModelAttribute" DisplayName="Model Attribute" Namespace="consist.RapidEntity">
      <BaseClass>
        <DomainClassMoniker Name="ClassModelElement" />
      </BaseClass>
      <Properties>
        <DomainProperty Id="dad27be4-c014-448b-afdb-fa85a18091f7" Description="Description for consist.RapidEntity.ModelAttribute.Type" Name="Type" DisplayName="Type">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(consist.RapidEntity.Customizations.EntityProperties.EntityPropertyEditor), typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2e789c55-e937-45e1-9613-b69c0892322d" Description="Description for consist.RapidEntity.ModelAttribute.Column Name" Name="ColumnName" DisplayName="Column Name">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(consist.RapidEntity.Customizations.EntityProperties.EntityPropertyEditor), typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="42ed9e26-5d92-4b0c-b755-cb3276c5bc56" Description="Description for consist.RapidEntity.ModelAttribute.Allow Null" Name="AllowNull" DisplayName="Allow Null">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="18b41612-70f6-4728-8cfa-eeba7f078e5e" Description="Description for consist.RapidEntity.ModelAttribute.Precision" Name="Precision" DisplayName="Precision">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f8f07f85-be86-40fa-bc49-3f8526e23949" Description="Description for consist.RapidEntity.ModelAttribute.Scale" Name="Scale" DisplayName="Scale">
          <Type>
            <ExternalTypeMoniker Name="/System/Int32" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="c6734bd5-5807-498c-bb73-247a5a9aef54" Description="Associations between Classes." Name="BaseRelationship" DisplayName="Relationship" InheritanceModifier="Abstract" Namespace="consist.RapidEntity" AllowsDuplicates="true">
      <Notes>This is the abstract base relationship of the several kinds of association between Classes.
      It defines the Properties that are attached to each association.</Notes>
      <Properties>
        <DomainProperty Id="d76804d6-b6a5-495c-80c3-5d6aae0eac3a" Description="" Name="SourceMultiplicity" DisplayName="Source Multiplicity" DefaultValue="" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7771bacf-4e24-4956-8fc7-1b934d508b16" Description="" Name="SourceRoleName" DisplayName="Source Role Name" DefaultValue="" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="064bdfd7-040d-4e55-af73-034dedddb1e4" Description="" Name="TargetMultiplicity" DisplayName="Target Multiplicity" DefaultValue="" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <DomainEnumerationMoniker Name="Multiplicity" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f00db165-4aa9-4597-a058-71c7e972e4bb" Description="" Name="TargetRoleName" DisplayName="Target Role Name" DefaultValue="" IsBrowsable="false" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="21549dfc-9b0b-4c53-adee-24a4f6674143" Description="Description for consist.RapidEntity.BaseRelationship.Reference Column" Name="ReferenceColumn" DisplayName="Reference Column">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="d2335f91-1957-4e27-ad47-8296ef650e2c" Description="Description for consist.RapidEntity.BaseRelationship.Reference Entity" Name="ReferenceEntity" DisplayName="Reference Entity">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="f468efe5-65b7-467b-ad0f-e065d85258e2" Description="Description for consist.RapidEntity.BaseRelationship.Owner Entity" Name="OwnerEntity" DisplayName="Owner Entity">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="06d5da4a-ca39-4f8c-9f03-a000db4245be" Description="Description for consist.RapidEntity.BaseRelationship.Default Role Name" Name="DefaultRoleName" DisplayName="Default Role Name" DefaultValue="    " IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="974bcf39-5cb4-45da-b48d-90077f118adb" Description="Description for consist.RapidEntity.BaseRelationship.Many Role Name" Name="ManyRoleName" DisplayName="Many Role Name" DefaultValue="0..*" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="658077c3-b30a-48b6-8b25-5a63cff65a61" Description="Description for consist.RapidEntity.BaseRelationship.One Role Name" Name="OneRoleName" DisplayName="One Role Name" DefaultValue="1" IsBrowsable="false">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="2c64faf7-0b9d-4404-bdd2-6ba7922f4b4a" Description="Description for consist.RapidEntity.BaseRelationship.Referenced Key" Name="ReferencedKey" DisplayName="Referenced Key">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e94b9dc5-940d-43c5-b2ac-bcec38c311e8" Description="Description for consist.RapidEntity.BaseRelationship.Type" Name="Type" DisplayName="Type">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="95b7a751-7267-464f-ab7a-16ebe1be70f7" Description="" Name="Source" DisplayName="Source" PropertyName="Targets" PropertyDisplayName="Targets">
          <Notes>The Targets property on a ModelClass will include all the elements targeted by every kind of Association.</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="c8119040-e4a6-4918-b0ef-4bca12e3a271" Description="" Name="Target" DisplayName="Target" PropertyName="Sources" PropertyDisplayName="Sources">
          <Notes>The Sources property on a ModelClass will include all the elements sourced by every kind of Association.</Notes>
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="ca4a177d-99f5-4932-bcb7-8f7cf3ca05a0" Description="" Name="ClassHasFields" DisplayName="Class Has Fields" Namespace="consist.RapidEntity" IsEmbedding="true">
      <Source>
        <DomainRole Id="1dcdf90c-3a20-420f-b57d-68626057e530" Description="" Name="ModelClass" DisplayName="Model Class" PropertyName="Fields" PropertyDisplayName="Fields">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="7cb320fd-2d6f-40dd-a8d6-d35aed78485f" Description="" Name="Attribute" DisplayName="Attribute" PropertyName="ModelClass" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Model Class">
          <RolePlayer>
            <DomainClassMoniker Name="Field" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="c5135fd4-4169-4ab8-8c02-3b30da4388a4" Description="" Name="ModelRootHasComments" DisplayName="Model Root Has Comments" Namespace="consist.RapidEntity" IsEmbedding="true">
      <Source>
        <DomainRole Id="89e8d218-725f-4e00-9859-c89995ff904a" Description="" Name="ModelRoot" DisplayName="Model Root" PropertyName="Comments" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="af9f4e2d-4c86-4f84-8f5b-d439983cd3aa" Description="" Name="Comment" DisplayName="Comment" PropertyName="ModelRoot" Multiplicity="One" PropagatesDelete="true" PropertyDisplayName="Model Root">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="a79105fb-4b47-4f22-b826-ea93592d1cd4" Description="Inheritance between Classes." Name="Generalization" DisplayName="Generalization" Namespace="consist.RapidEntity">
      <Properties>
        <DomainProperty Id="0b5c6234-9803-4394-8382-217ae016d0dc" Description="" Name="Discriminator" DisplayName="Discriminator" DefaultValue="">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="17248147-e7ab-40c4-8a20-e94797c0a90a" Description="" Name="Superclass" DisplayName="Superclass" PropertyName="Subclasses" PropertyDisplayName="Subclasses">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="1ba399d5-7b63-42b7-aa2d-01440bd80040" Description="" Name="Subclass" DisplayName="Subclass" PropertyName="Superclass" Multiplicity="ZeroOne" PropertyDisplayName="Superclass">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="f52a2a14-3cec-4122-b124-1e58d5d10e0a" Description="" Name="OneToOne" DisplayName="One To One" Namespace="consist.RapidEntity" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="BaseRelationship" />
      </BaseRelationship>
      <Properties>
        <DomainProperty Id="33c11381-0554-4af2-bfd6-b11d8c7e4bf7" Description="Description for consist.RapidEntity.OneToOne.Relation Column" Name="RelationColumn" DisplayName="Relation Column" IsUIReadOnly="true">
          <Attributes>
            <ClrAttribute Name="System.ComponentModel.Editor">
              <Parameters>
                <AttributeParameter Value="typeof(consist.RapidEntity.Customizations.Relationship.RelationshipEditor), typeof(System.Drawing.Design.UITypeEditor)" />
              </Parameters>
            </ClrAttribute>
          </Attributes>
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="7e173564-1e2a-43ce-ad1d-4337c6d24b71" Description="Description for consist.RapidEntity.OneToOne.Cascade" Name="Cascade" DisplayName="Cascade" DefaultValue="Cascade.NOTSET" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="11daeab4-fed9-47f5-aad4-ef72e16f9928" Description="Description for consist.RapidEntity.OneToOne.Is Imported" Name="IsImported" DisplayName="Is Imported">
          <Type>
            <ExternalTypeMoniker Name="/System/Boolean" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="80e9f254-b85b-453b-89f4-01b75fb91ca6" Description="" Name="BidirectionalSource" DisplayName="Bidirectional Source" PropertyName="OneToOneTargets" PropertyDisplayName="One To One Targets">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="ab1a2013-2931-4015-b976-72fd10fc5cd3" Description="" Name="BidirectionalTarget" DisplayName="Bidirectional Target" PropertyName="OneToOneSources" PropertyDisplayName="One To One Sources">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="56c847ae-c860-4f0f-819f-cc775517f716" Description="" Name="OneToMany" DisplayName="One To Many" Namespace="consist.RapidEntity" AllowsDuplicates="true">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="BaseRelationship" />
      </BaseRelationship>
      <Properties>
        <DomainProperty Id="e54e4fee-8913-4231-8364-ab4af2524488" Description="Description for consist.RapidEntity.OneToMany.Relation Column" Name="RelationColumn" DisplayName="Relation Column" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="cafecae1-8748-49aa-83de-453d83a16dfd" Description="Description for consist.RapidEntity.OneToMany.Cascade" Name="Cascade" DisplayName="Cascade" DefaultValue="Cascade.NOTSET" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="db3c9e19-e21a-41a2-969b-666a6db796c5" Description="" Name="AggregationSource" DisplayName="Aggregation Source" PropertyName="OneToManyTargets" PropertyDisplayName="One To Many Targets">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="212356a7-a58d-4856-8c81-f77a653cfc91" Description="" Name="AggregationTarget" DisplayName="Aggregation Target" PropertyName="OneToManySources" PropertyDisplayName="One To Many Sources">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="e757a872-6a18-4297-9cb7-28bbfdddb645" Description="" Name="ModelRootHasTypes" DisplayName="Model Root Has Types" Namespace="consist.RapidEntity" IsEmbedding="true">
      <Source>
        <DomainRole Id="3b35e7dc-c553-4c9e-b00b-9fef50ec0756" Description="" Name="ModelRoot" DisplayName="Model Root" PropertyName="Types" PropertyDisplayName="Types">
          <RolePlayer>
            <DomainClassMoniker Name="ModelRoot" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="616cb0c9-eb37-4bd4-b32a-3a2927a2f03c" Description="" Name="Type" DisplayName="Type" PropertyName="ModelRoot" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="8ec6619c-21e8-4625-89cc-a7a8a14633e1" Description="" Name="CommentReferencesSubjects" DisplayName="Comment References Subjects" Namespace="consist.RapidEntity">
      <Source>
        <DomainRole Id="9061f823-5ec1-4c16-b1e5-889b02c1fc75" Description="" Name="Comment" DisplayName="Comment" PropertyName="Subjects" PropertyDisplayName="Subjects">
          <RolePlayer>
            <DomainClassMoniker Name="Comment" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="99ea99fb-298f-4126-adc2-5cbe5a24e203" Description="" Name="Subject" DisplayName="Subject" PropertyName="Comments" PropertyDisplayName="Comments">
          <RolePlayer>
            <DomainClassMoniker Name="ModelType" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="b7be6d9e-0048-4fd5-8f5b-6b1771b19a17" Description="Description for consist.RapidEntity.ManyToMany" Name="ManyToMany" DisplayName="Many To Many" Namespace="consist.RapidEntity">
      <BaseRelationship>
        <DomainRelationshipMoniker Name="BaseRelationship" />
      </BaseRelationship>
      <Properties>
        <DomainProperty Id="d15e43dc-5590-49c0-ba97-f1f10d14b8d0" Description="Description for consist.RapidEntity.ManyToMany.Join Table" Name="JoinTable" DisplayName="Join Table" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="231c557f-6a87-4dc7-b9ee-db95afed1eb2" Description="Description for consist.RapidEntity.ManyToMany.Join Column" Name="JoinColumn" DisplayName="Join Column" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e1ca1b20-339e-444f-9a1d-394ec9182a36" Description="Description for consist.RapidEntity.ManyToMany.Owner Column" Name="OwnerColumn" DisplayName="Owner Column" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e625d320-c6b7-43b5-a62f-ea8d7537b366" Description="Description for consist.RapidEntity.ManyToMany.Cascade" Name="Cascade" DisplayName="Cascade" DefaultValue="Cascade.NOTSET" IsUIReadOnly="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="fef4c4c6-2e0f-425c-bbf3-70209ccb0db0" Description="Description for consist.RapidEntity.ManyToMany.Owner Key" Name="OwnerKey" DisplayName="Owner Key">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <Source>
        <DomainRole Id="a58ac9a3-218d-44c7-a93f-822a3303b085" Description="Description for consist.RapidEntity.ManyToMany.SourceModelClass" Name="SourceModelClass" DisplayName="Source Model Class" PropertyName="ManyTargetModel" PropertyDisplayName="Many Target Model">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="df0c9af6-166d-41fd-90b0-e59e934be43f" Description="Description for consist.RapidEntity.ManyToMany.TargetModelClass" Name="TargetModelClass" DisplayName="Target Model Class" PropertyName="ManySourceModels" PropertyDisplayName="Many Source Models">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
    <DomainRelationship Id="d8ebf6d7-1637-4657-aaaf-b4a2ffb2c314" Description="Description for consist.RapidEntity.ClassHasPersistentKeys" Name="ClassHasPersistentKeys" DisplayName="Class Has Persistent Keys" Namespace="consist.RapidEntity" IsEmbedding="true">
      <Source>
        <DomainRole Id="92a845d5-a5cf-4d2b-a929-b201373e75e7" Description="Description for consist.RapidEntity.ClassHasPersistentKeys.ModelClass" Name="ModelClass" DisplayName="Model Class" PropertyName="PersistentKeys" PropertyDisplayName="Persistent Keys">
          <RolePlayer>
            <DomainClassMoniker Name="ModelClass" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="317db333-a89a-4f22-8862-de0074b1eb67" Description="Description for consist.RapidEntity.ClassHasPersistentKeys.PersistentKey" Name="PersistentKey" DisplayName="Persistent Key" PropertyName="ModelClass" Multiplicity="One" PropagatesDelete="true" PropagatesCopy="true" PropertyDisplayName="Model Class">
          <RolePlayer>
            <DomainClassMoniker Name="PersistentKey" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
    <DomainEnumeration Name="AccessModifier" Namespace="consist.RapidEntity" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Public" Value="0" />
        <EnumerationLiteral Description="" Name="Assembly" Value="1" />
        <EnumerationLiteral Description="" Name="Private" Value="2" />
        <EnumerationLiteral Description="" Name="Family" Value="3" />
        <EnumerationLiteral Description="" Name="FamilyOrAssembly" Value="4" />
        <EnumerationLiteral Description="" Name="FamilyAndAssembly" Value="5" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="TypeAccessModifier" Namespace="consist.RapidEntity" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Public" Value="0" />
        <EnumerationLiteral Description="" Name="Private" Value="1" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="InheritanceModifier" Namespace="consist.RapidEntity" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="None" Value="0" />
        <EnumerationLiteral Description="" Name="Abstract" Value="1" />
        <EnumerationLiteral Description="" Name="Sealed" Value="2" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="Multiplicity" Namespace="consist.RapidEntity" Description="">
      <Literals>
        <EnumerationLiteral Description="0..*" Name="ZeroMany" Value="0" />
        <EnumerationLiteral Description="" Name="One" Value="1" />
        <EnumerationLiteral Description="" Name="ZeroOne" Value="2" />
        <EnumerationLiteral Description="" Name="OneMany" Value="3" />
      </Literals>
    </DomainEnumeration>
    <DomainEnumeration Name="OperationConcurrency" Namespace="consist.RapidEntity" Description="">
      <Literals>
        <EnumerationLiteral Description="" Name="Sequential" Value="0" />
        <EnumerationLiteral Description="" Name="Guarded" Value="1" />
        <EnumerationLiteral Description="" Name="Concurrent" Value="2" />
      </Literals>
    </DomainEnumeration>
  </Types>
  <Shapes>
    <CompartmentShape Id="27c3b877-e65f-4de1-9861-9f113a6dd034" Description="" Name="ClassShape" DisplayName="Class Shape" Namespace="consist.RapidEntity" FixedTooltipText="Class Shape" FillColor="LightSkyBlue" InitialHeight="0.3" OutlineThickness="0.01" FillGradientMode="ForwardDiagonal" Geometry="RoundedRectangle">
      <ShapeHasDecorators Position="InnerTopCenter" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Name" DisplayName="Name" DefaultText="Name" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopRight" HorizontalOffset="0" VerticalOffset="0">
        <ExpandCollapseDecorator Name="ExpandCollapse" DisplayName="Expand Collapse" />
      </ShapeHasDecorators>
      <ShapeHasDecorators Position="InnerTopLeft" HorizontalOffset="0" VerticalOffset="0">
        <IconDecorator Name="ArtifactIcon" DisplayName="Artifact Icon" DefaultIcon="Resources\artifact_logo.bmp" />
      </ShapeHasDecorators>
      <Compartment TitleFillColor="235, 235, 235" Name="AttributesCompartment" Title="Persistent Fields" />
      <Compartment TitleFillColor="235, 235, 235" Name="KeysCompartment" Title="Keys" />
    </CompartmentShape>
    <GeometryShape Id="ab663580-542c-44a1-be8d-20e4c37eba35" Description="" Name="CommentBoxShape" DisplayName="Comment Box Shape" Namespace="consist.RapidEntity" FixedTooltipText="Comment Box Shape" FillColor="255, 255, 204" OutlineColor="204, 204, 102" InitialHeight="0.3" OutlineThickness="0.01" FillGradientMode="None" Geometry="Rectangle">
      <ShapeHasDecorators Position="Center" HorizontalOffset="0" VerticalOffset="0">
        <TextDecorator Name="Comment" DisplayName="Comment" DefaultText="BusinessRulesShapeNameDecorator" />
      </ShapeHasDecorators>
    </GeometryShape>
  </Shapes>
  <Connectors>
    <Connector Id="eb382bc1-b397-4673-adb8-40f7aaedfcc1" Description="" Name="RelationshipConnector" DisplayName="Relationship Connector" InheritanceModifier="Abstract" Namespace="consist.RapidEntity" FixedTooltipText="Relationship Connector" Color="113, 111, 110" Thickness="0.01">
      <ConnectorHasDecorators Position="TargetBottom" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="TargetMultiplicity" DisplayName="Target Multiplicity" DefaultText="TargetMultiplicity" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="SourceBottom" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="SourceMultiplicity" DisplayName="Source Multiplicity" DefaultText="SourceMultiplicity" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="TargetTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="TargetRoleName" DisplayName="Target Role Name" DefaultText="TargetRoleName" />
      </ConnectorHasDecorators>
      <ConnectorHasDecorators Position="SourceTop" OffsetFromShape="0" OffsetFromLine="0">
        <TextDecorator Name="SourceRoleName" DisplayName="Source Role Name" DefaultText="SourceRoleName" />
      </ConnectorHasDecorators>
    </Connector>
    <Connector Id="7d254598-b383-4814-aa53-d27ea0617e13" Description="" Name="BidirectionalConnector" DisplayName="Bidirectional Connector" Namespace="consist.RapidEntity" FixedTooltipText="" Color="DodgerBlue" DashStyle="Dash" SourceEndStyle="FilledDiamond" TargetEndStyle="FilledArrow" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="RelationshipConnector" />
      </BaseConnector>
    </Connector>
    <Connector Id="19453173-064c-4172-a111-4e7a6666bfe7" Description="" Name="AggregationConnector" DisplayName="Aggregation Connector" Namespace="consist.RapidEntity" FixedTooltipText="Aggregation Connector" Color="DodgerBlue" SourceEndStyle="FilledDiamond" TargetEndStyle="FilledArrow" Thickness="0.01">
      <BaseConnector>
        <ConnectorMoniker Name="RelationshipConnector" />
      </BaseConnector>
    </Connector>
    <Connector Id="e6294267-4e2f-4e6a-bcb6-6e33b3bcf8d5" Description="" Name="GeneralizationConnector" DisplayName="Generalization Connector" Namespace="consist.RapidEntity" FixedTooltipText="Generalization Connector" Color="113, 111, 110" SourceEndStyle="HollowArrow" Thickness="0.01" />
    <Connector Id="c929a787-49e8-45b9-8f4a-1c3467aa08bb" Description="" Name="CommentConnector" DisplayName="Comment Connector" Namespace="consist.RapidEntity" FixedTooltipText="Comment Connector" Color="113, 111, 110" DashStyle="Dot" Thickness="0.01" RoutingStyle="Straight" />
    <Connector Id="a39ace9f-a9b5-4eb2-9861-17a6fe412e3e" Description="Description for consist.RapidEntity.ManyToManyConnector" Name="ManyToManyConnector" DisplayName="Many To Many Connector" Namespace="consist.RapidEntity" FixedTooltipText="Many To Many Connector" Color="DodgerBlue" SourceEndStyle="FilledDiamond" TargetEndStyle="FilledArrow">
      <BaseConnector>
        <ConnectorMoniker Name="RelationshipConnector" />
      </BaseConnector>
    </Connector>
  </Connectors>
  <XmlSerializationBehavior Name="RapidEntitySerializationBehavior" Namespace="consist.RapidEntity">
    <ClassData>
      <XmlClassData TypeName="NamedElement" MonikerAttributeName="" MonikerElementName="namedElementMoniker" ElementName="namedElement" MonikerTypeName="NamedElementMoniker">
        <DomainClassMoniker Name="NamedElement" />
        <ElementData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="NamedElement/Name" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="BaseRelationship" MonikerAttributeName="" SerializeId="true" MonikerElementName="baseRelationshipMoniker" ElementName="baseRelationship" MonikerTypeName="BaseRelationshipMoniker">
        <DomainRelationshipMoniker Name="BaseRelationship" />
        <ElementData>
          <XmlPropertyData XmlName="sourceMultiplicity">
            <DomainPropertyMoniker Name="BaseRelationship/SourceMultiplicity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="sourceRoleName">
            <DomainPropertyMoniker Name="BaseRelationship/SourceRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetMultiplicity">
            <DomainPropertyMoniker Name="BaseRelationship/TargetMultiplicity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="targetRoleName">
            <DomainPropertyMoniker Name="BaseRelationship/TargetRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="referenceColumn">
            <DomainPropertyMoniker Name="BaseRelationship/ReferenceColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="referenceEntity">
            <DomainPropertyMoniker Name="BaseRelationship/ReferenceEntity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="ownerEntity">
            <DomainPropertyMoniker Name="BaseRelationship/OwnerEntity" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="defaultRoleName">
            <DomainPropertyMoniker Name="BaseRelationship/DefaultRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="manyRoleName">
            <DomainPropertyMoniker Name="BaseRelationship/ManyRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="oneRoleName">
            <DomainPropertyMoniker Name="BaseRelationship/OneRoleName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="referencedKey">
            <DomainPropertyMoniker Name="BaseRelationship/ReferencedKey" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="type">
            <DomainPropertyMoniker Name="BaseRelationship/Type" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassHasFields" MonikerAttributeName="" MonikerElementName="classHasFieldsMoniker" ElementName="classHasFields" MonikerTypeName="ClassHasFieldsMoniker">
        <DomainRelationshipMoniker Name="ClassHasFields" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasComments" MonikerAttributeName="" MonikerElementName="modelRootHasCommentsMoniker" ElementName="modelRootHasComments" MonikerTypeName="ModelRootHasCommentsMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasComments" />
      </XmlClassData>
      <XmlClassData TypeName="Generalization" MonikerAttributeName="" MonikerElementName="generalizationMoniker" ElementName="generalization" MonikerTypeName="GeneralizationMoniker">
        <DomainRelationshipMoniker Name="Generalization" />
        <ElementData>
          <XmlPropertyData XmlName="discriminator">
            <DomainPropertyMoniker Name="Generalization/Discriminator" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelRootHasTypes" MonikerAttributeName="" MonikerElementName="modelRootHasTypesMoniker" ElementName="modelRootHasTypes" MonikerTypeName="ModelRootHasTypesMoniker">
        <DomainRelationshipMoniker Name="ModelRootHasTypes" />
      </XmlClassData>
      <XmlClassData TypeName="CommentReferencesSubjects" MonikerAttributeName="" MonikerElementName="commentReferencesSubjectsMoniker" ElementName="commentReferencesSubjects" MonikerTypeName="CommentReferencesSubjectsMoniker">
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </XmlClassData>
      <XmlClassData TypeName="ModelRoot" MonikerAttributeName="" MonikerElementName="modelRootMoniker" ElementName="modelRoot" MonikerTypeName="ModelRootMoniker">
        <DomainClassMoniker Name="ModelRoot" />
        <ElementData>
          <XmlRelationshipData RoleElementName="comments">
            <DomainRelationshipMoniker Name="ModelRootHasComments" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="types">
            <DomainRelationshipMoniker Name="ModelRootHasTypes" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelClass" MonikerAttributeName="" MonikerElementName="modelClassMoniker" ElementName="modelClass" MonikerTypeName="ModelClassMoniker">
        <DomainClassMoniker Name="ModelClass" />
        <ElementData>
          <XmlPropertyData XmlName="kind">
            <DomainPropertyMoniker Name="ModelClass/Kind" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isAbstract">
            <DomainPropertyMoniker Name="ModelClass/IsAbstract" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="fields">
            <DomainRelationshipMoniker Name="ClassHasFields" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="subclasses">
            <DomainRelationshipMoniker Name="Generalization" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="oneToOneTargets">
            <DomainRelationshipMoniker Name="OneToOne" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="oneToManyTargets">
            <DomainRelationshipMoniker Name="OneToMany" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="targets">
            <DomainRelationshipMoniker Name="BaseRelationship" />
          </XmlRelationshipData>
          <XmlRelationshipData UseFullForm="true" RoleElementName="manyTargetModel">
            <DomainRelationshipMoniker Name="ManyToMany" />
          </XmlRelationshipData>
          <XmlRelationshipData RoleElementName="persistentKeys">
            <DomainRelationshipMoniker Name="ClassHasPersistentKeys" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="tableName">
            <DomainPropertyMoniker Name="ModelClass/TableName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="parentName">
            <DomainPropertyMoniker Name="ModelClass/ParentName" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="Field" MonikerAttributeName="" MonikerElementName="fieldMoniker" ElementName="field" MonikerTypeName="FieldMoniker">
        <DomainClassMoniker Name="Field" />
      </XmlClassData>
      <XmlClassData TypeName="Comment" MonikerAttributeName="" SerializeId="true" MonikerElementName="commentMoniker" ElementName="comment" MonikerTypeName="CommentMoniker">
        <DomainClassMoniker Name="Comment" />
        <ElementData>
          <XmlPropertyData XmlName="text">
            <DomainPropertyMoniker Name="Comment/Text" />
          </XmlPropertyData>
          <XmlRelationshipData RoleElementName="subjects">
            <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
          </XmlRelationshipData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelType" MonikerAttributeName="" MonikerElementName="modelTypeMoniker" ElementName="modelType" MonikerTypeName="ModelTypeMoniker">
        <DomainClassMoniker Name="ModelType" />
      </XmlClassData>
      <XmlClassData TypeName="ClassModelElement" MonikerAttributeName="" MonikerElementName="classModelElementMoniker" ElementName="classModelElement" MonikerTypeName="ClassModelElementMoniker">
        <DomainClassMoniker Name="ClassModelElement" />
      </XmlClassData>
      <XmlClassData TypeName="OneToOne" MonikerAttributeName="" SerializeId="true" MonikerElementName="oneToOneMoniker" ElementName="oneToOne" MonikerTypeName="OneToOneMoniker">
        <DomainRelationshipMoniker Name="OneToOne" />
        <ElementData>
          <XmlPropertyData XmlName="relationColumn">
            <DomainPropertyMoniker Name="OneToOne/RelationColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cascade">
            <DomainPropertyMoniker Name="OneToOne/Cascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="isImported">
            <DomainPropertyMoniker Name="OneToOne/IsImported" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="OneToMany" MonikerAttributeName="" SerializeId="true" MonikerElementName="oneToManyMoniker" ElementName="oneToMany" MonikerTypeName="OneToManyMoniker">
        <DomainRelationshipMoniker Name="OneToMany" />
        <ElementData>
          <XmlPropertyData XmlName="relationColumn">
            <DomainPropertyMoniker Name="OneToMany/RelationColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cascade">
            <DomainPropertyMoniker Name="OneToMany/Cascade" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassShape" MonikerAttributeName="" MonikerElementName="classShapeMoniker" ElementName="classShape" MonikerTypeName="ClassShapeMoniker">
        <CompartmentShapeMoniker Name="ClassShape" />
      </XmlClassData>
      <XmlClassData TypeName="CommentBoxShape" MonikerAttributeName="" MonikerElementName="commentBoxShapeMoniker" ElementName="commentBoxShape" MonikerTypeName="CommentBoxShapeMoniker">
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </XmlClassData>
      <XmlClassData TypeName="RelationshipConnector" MonikerAttributeName="" MonikerElementName="relationshipConnectorMoniker" ElementName="relationshipConnector" MonikerTypeName="RelationshipConnectorMoniker">
        <ConnectorMoniker Name="RelationshipConnector" />
      </XmlClassData>
      <XmlClassData TypeName="BidirectionalConnector" MonikerAttributeName="" MonikerElementName="bidirectionalConnectorMoniker" ElementName="bidirectionalConnector" MonikerTypeName="BidirectionalConnectorMoniker">
        <ConnectorMoniker Name="BidirectionalConnector" />
      </XmlClassData>
      <XmlClassData TypeName="AggregationConnector" MonikerAttributeName="" MonikerElementName="aggregationConnectorMoniker" ElementName="aggregationConnector" MonikerTypeName="AggregationConnectorMoniker">
        <ConnectorMoniker Name="AggregationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="GeneralizationConnector" MonikerAttributeName="" MonikerElementName="generalizationConnectorMoniker" ElementName="generalizationConnector" MonikerTypeName="GeneralizationConnectorMoniker">
        <ConnectorMoniker Name="GeneralizationConnector" />
      </XmlClassData>
      <XmlClassData TypeName="CommentConnector" MonikerAttributeName="" MonikerElementName="commentConnectorMoniker" ElementName="commentConnector" MonikerTypeName="CommentConnectorMoniker">
        <ConnectorMoniker Name="CommentConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ClassDiagram" MonikerAttributeName="" MonikerElementName="classDiagramMoniker" ElementName="classDiagram" MonikerTypeName="ClassDiagramMoniker">
        <DiagramMoniker Name="ClassDiagram" />
        <ElementData>
          <XmlPropertyData XmlName="encryptedConnection">
            <DomainPropertyMoniker Name="ClassDiagram/EncryptedConnection" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="connectionString">
            <DomainPropertyMoniker Name="ClassDiagram/ConnectionString" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="nameSingularization">
            <DomainPropertyMoniker Name="ClassDiagram/NameSingularization" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="providerGuid">
            <DomainPropertyMoniker Name="ClassDiagram/ProviderGuid" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="removableTablePrefixes">
            <DomainPropertyMoniker Name="ClassDiagram/RemovableTablePrefixes" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="hasGrid">
            <DomainPropertyMoniker Name="ClassDiagram/HasGrid" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="PersistentKey" MonikerAttributeName="" MonikerElementName="persistentKeyMoniker" ElementName="persistentKey" MonikerTypeName="PersistentKeyMoniker">
        <DomainClassMoniker Name="PersistentKey" />
        <ElementData>
          <XmlPropertyData XmlName="isAutoKey">
            <DomainPropertyMoniker Name="PersistentKey/IsAutoKey" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ManyToManyConnector" MonikerAttributeName="" MonikerElementName="manyToManyConnectorMoniker" ElementName="manyToManyConnector" MonikerTypeName="ManyToManyConnectorMoniker">
        <ConnectorMoniker Name="ManyToManyConnector" />
      </XmlClassData>
      <XmlClassData TypeName="ManyToMany" MonikerAttributeName="" SerializeId="true" MonikerElementName="manyToManyMoniker" ElementName="manyToMany" MonikerTypeName="ManyToManyMoniker">
        <DomainRelationshipMoniker Name="ManyToMany" />
        <ElementData>
          <XmlPropertyData XmlName="joinTable">
            <DomainPropertyMoniker Name="ManyToMany/JoinTable" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="joinColumn">
            <DomainPropertyMoniker Name="ManyToMany/JoinColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="ownerColumn">
            <DomainPropertyMoniker Name="ManyToMany/OwnerColumn" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="cascade">
            <DomainPropertyMoniker Name="ManyToMany/Cascade" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="ownerKey">
            <DomainPropertyMoniker Name="ManyToMany/OwnerKey" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ModelAttribute" MonikerAttributeName="" MonikerElementName="modelAttributeMoniker" ElementName="modelAttribute" MonikerTypeName="ModelAttributeMoniker">
        <DomainClassMoniker Name="ModelAttribute" />
        <ElementData>
          <XmlPropertyData XmlName="type">
            <DomainPropertyMoniker Name="ModelAttribute/Type" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="columnName">
            <DomainPropertyMoniker Name="ModelAttribute/ColumnName" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="allowNull">
            <DomainPropertyMoniker Name="ModelAttribute/AllowNull" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="precision">
            <DomainPropertyMoniker Name="ModelAttribute/Precision" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="scale">
            <DomainPropertyMoniker Name="ModelAttribute/Scale" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="ClassHasPersistentKeys" MonikerAttributeName="" MonikerElementName="classHasPersistentKeysMoniker" ElementName="classHasPersistentKeys" MonikerTypeName="ClassHasPersistentKeysMoniker">
        <DomainRelationshipMoniker Name="ClassHasPersistentKeys" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="RapidEntityExplorer">
    <CustomNodeSettings>
      <ExplorerNodeSettings IconToDisplay="Resources\artifact_logo.bmp" ShowsDomainClass="true">
        <Class>
          <DomainClassMoniker Name="ModelClass" />
        </Class>
      </ExplorerNodeSettings>
    </CustomNodeSettings>
  </ExplorerBehavior>
  <ConnectionBuilders>
    <ConnectionBuilder Name="OneToOneBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="OneToOne" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="OneToManyBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="OneToMany" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="GeneralizationBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="Generalization" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="CommentReferencesSubjectsBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="Comment" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelType" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
    <ConnectionBuilder Name="ManyToManyBuilder">
      <LinkConnectDirective>
        <DomainRelationshipMoniker Name="ManyToMany" />
        <SourceDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </SourceDirectives>
        <TargetDirectives>
          <RolePlayerConnectDirective>
            <AcceptingClass>
              <DomainClassMoniker Name="ModelClass" />
            </AcceptingClass>
          </RolePlayerConnectDirective>
        </TargetDirectives>
      </LinkConnectDirective>
    </ConnectionBuilder>
  </ConnectionBuilders>
  <Diagram Id="d9fd7045-072a-4acf-bb8b-58168a4da914" Description="" Name="ClassDiagram" DisplayName="Class Diagram" Namespace="consist.RapidEntity">
    <Properties>
      <DomainProperty Id="13d97aa9-5962-43e4-b44a-a946a30152df" Description="Description for consist.RapidEntity.ClassDiagram.Encrypted Connection" Name="EncryptedConnection" DisplayName="Encrypted Connection" IsUIReadOnly="true">
        <Type>
          <ExternalTypeMoniker Name="/System/String" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="6cec6e9b-0c98-4501-b0d7-d373930012fd" Description="Description for consist.RapidEntity.ClassDiagram.Connection String" Name="ConnectionString" DisplayName="Connection String">
        <Attributes>
          <ClrAttribute Name="System.ComponentModel.Editor">
            <Parameters>
              <AttributeParameter Value="typeof(consist.RapidEntity.Customizations.ClassDiagramPropertyEditor), typeof(System.Drawing.Design.UITypeEditor)" />
            </Parameters>
          </ClrAttribute>
        </Attributes>
        <Type>
          <ExternalTypeMoniker Name="/System/String" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="b45afc55-614f-46fb-b791-e7a72975dabc" Description="Description for consist.RapidEntity.ClassDiagram.Name Singularization" Name="NameSingularization" DisplayName="Name Singularization">
        <Type>
          <ExternalTypeMoniker Name="/System/Boolean" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="cfd0d4d7-5643-46ad-b515-f11634143188" Description="Description for consist.RapidEntity.ClassDiagram.Provider Guid" Name="ProviderGuid" DisplayName="Provider Guid">
        <Type>
          <ExternalTypeMoniker Name="/System/Guid" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="6da518f4-4c59-436f-9e96-e2d3b80c5469" Description="Description for consist.RapidEntity.ClassDiagram.Removable Table Prefixes" Name="RemovableTablePrefixes" DisplayName="Removable Table Prefixes">
        <Type>
          <ExternalTypeMoniker Name="/System/String" />
        </Type>
      </DomainProperty>
      <DomainProperty Id="dc962417-3b0b-4522-bd6b-37f828dd73e3" Description="Description for consist.RapidEntity.ClassDiagram.Has Grid" Name="HasGrid" DisplayName="Has Grid" IsBrowsable="false">
        <Type>
          <ExternalTypeMoniker Name="/System/Boolean" />
        </Type>
      </DomainProperty>
    </Properties>
    <Class>
      <DomainClassMoniker Name="ModelRoot" />
    </Class>
    <ShapeMaps>
      <CompartmentShapeMap>
        <DomainClassMoniker Name="ModelClass" />
        <ParentElementPath>
          <DomainPath>ModelRootHasTypes.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="ClassShape/Name" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
              <DomainPath />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <CompartmentShapeMoniker Name="ClassShape" />
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/AttributesCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasFields.Fields/!Attribute</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
        <CompartmentMap>
          <CompartmentMoniker Name="ClassShape/KeysCompartment" />
          <ElementsDisplayed>
            <DomainPath>ClassHasPersistentKeys.PersistentKeys/!PersistentKey</DomainPath>
          </ElementsDisplayed>
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="NamedElement/Name" />
            </PropertyPath>
          </PropertyDisplayed>
        </CompartmentMap>
      </CompartmentShapeMap>
      <ShapeMap>
        <DomainClassMoniker Name="Comment" />
        <ParentElementPath>
          <DomainPath>ModelRootHasComments.ModelRoot/!ModelRoot</DomainPath>
        </ParentElementPath>
        <DecoratorMap>
          <TextDecoratorMoniker Name="CommentBoxShape/Comment" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="Comment/Text" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <GeometryShapeMoniker Name="CommentBoxShape" />
      </ShapeMap>
    </ShapeMaps>
    <ConnectorMaps>
      <ConnectorMap>
        <ConnectorMoniker Name="AggregationConnector" />
        <DomainRelationshipMoniker Name="OneToMany" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/DefaultRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/DefaultRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/OneRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/ManyRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="CommentConnector" />
        <DomainRelationshipMoniker Name="CommentReferencesSubjects" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="GeneralizationConnector" />
        <DomainRelationshipMoniker Name="Generalization" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="ManyToManyConnector" />
        <DomainRelationshipMoniker Name="ManyToMany" />
      </ConnectorMap>
      <ConnectorMap>
        <ConnectorMoniker Name="BidirectionalConnector" />
        <DomainRelationshipMoniker Name="OneToOne" />
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/SourceRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/OneRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/TargetRoleName" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/OneRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/SourceMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/DefaultRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
        <DecoratorMap>
          <TextDecoratorMoniker Name="RelationshipConnector/TargetMultiplicity" />
          <PropertyDisplayed>
            <PropertyPath>
              <DomainPropertyMoniker Name="BaseRelationship/DefaultRoleName" />
            </PropertyPath>
          </PropertyDisplayed>
        </DecoratorMap>
      </ConnectorMap>
    </ConnectorMaps>
  </Diagram>
  <Designer FileExtension="rapd" EditorGuid="fef902f3-b77f-40d9-9229-02580c178cb8">
    <RootClass>
      <DomainClassMoniker Name="ModelRoot" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="RapidEntitySerializationBehavior" />
    </XmlSerializationDefinition>
    <ToolboxTab TabText="Class Diagrams">
      <ElementTool Name="ModelClass" ToolboxIcon="Resources\ClassTool.bmp" Caption="Entity" Tooltip="Create a Class" HelpKeyword="ModelClassF1Keyword">
        <DomainClassMoniker Name="ModelClass" />
      </ElementTool>
      <ConnectionTool Name="BidirectionalAssociation" ToolboxIcon="Resources\UnidirectionTool.bmp" Caption="One To One" Tooltip="Create a Bidirectional link" HelpKeyword="ConnectBidirectionalAssociationF1Keyword">
        <ConnectionBuilderMoniker Name="RapidEntity/OneToOneBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="Aggregation" ToolboxIcon="Resources\AggregationTool.bmp" Caption="One to Many" Tooltip="Create an Aggregation link" HelpKeyword="AggregationF1Keyword">
        <ConnectionBuilderMoniker Name="RapidEntity/OneToManyBuilder" />
      </ConnectionTool>
      <ConnectionTool Name="Generalization" ToolboxIcon="resources\generalizationtool.bmp" Caption="Inheritance" Tooltip="Create a Generalization or Implementation link" HelpKeyword="GeneralizationF1Keyword" ReversesDirection="true">
        <ConnectionBuilderMoniker Name="RapidEntity/GeneralizationBuilder" />
      </ConnectionTool>
      <ElementTool Name="Comment" ToolboxIcon="resources\commenttool.bmp" Caption="Comment" Tooltip="Create a Comment" HelpKeyword="CommentF1Keyword">
        <DomainClassMoniker Name="Comment" />
      </ElementTool>
      <ConnectionTool Name="CommentsReferenceTypes" ToolboxIcon="resources\commentlinktool.bmp" Caption="Comment Link" Tooltip="Link a comment to an element" HelpKeyword="CommentsReferenceTypesF1Keyword">
        <ConnectionBuilderMoniker Name="RapidEntity/CommentReferencesSubjectsBuilder" />
      </ConnectionTool>
    </ToolboxTab>
    <Validation UsesMenu="true" UsesOpen="true" UsesSave="true" UsesLoad="false" />
    <DiagramMoniker Name="ClassDiagram" />
  </Designer>
  <Explorer ExplorerGuid="20fc2e25-12d7-41b9-b100-594e01ed6fc7" Title="Rapid Entity Explorer">
    <ExplorerBehaviorMoniker Name="RapidEntity/RapidEntityExplorer" />
  </Explorer>
</Dsl>