﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainModel RapidEntityDomainModel
	/// Description for consist.RapidEntity.RapidEntity
	/// </summary>
	[DslDesign::DisplayNameResource("consist.RapidEntity.RapidEntityDomainModel.DisplayName", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
	[DslDesign::DescriptionResource("consist.RapidEntity.RapidEntityDomainModel.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
	[global::System.CLSCompliant(true)]
	[DslModeling::DependsOnDomainModel(typeof(global::Microsoft.VisualStudio.Modeling.CoreDomainModel))]
	[DslModeling::DependsOnDomainModel(typeof(global::Microsoft.VisualStudio.Modeling.Diagrams.CoreDesignSurfaceDomainModel))]
	[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]
	[DslModeling::DomainObjectId("cf321742-a8ae-462e-af13-e524570634a6")]
	public partial class RapidEntityDomainModel : DslModeling::DomainModel
	{
		#region Constructor, domain model Id
	
		/// <summary>
		/// RapidEntityDomainModel domain model Id.
		/// </summary>
		public static readonly global::System.Guid DomainModelId = new global::System.Guid(0xcf321742, 0xa8ae, 0x462e, 0xaf, 0x13, 0xe5, 0x24, 0x57, 0x06, 0x34, 0xa6);
	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="store">Store containing the domain model.</param>
		public RapidEntityDomainModel(DslModeling::Store store)
			: base(store, DomainModelId)
		{
			// Call the partial method to allow any required serialization setup to be done.
			this.InitializeSerialization(store);		
		}
		
	
		///<Summary>
		/// Defines a partial method that will be called from the constructor to
		/// allow any necessary serialization setup to be done.
		///</Summary>
		///<remarks>
		/// For a DSL created with the DSL Designer wizard, an implementation of this 
		/// method will be generated in the GeneratedCode\SerializationHelper.cs class.
		///</remarks>
		partial void InitializeSerialization(DslModeling::Store store);
	
	
		#endregion
		#region Domain model reflection
			
		/// <summary>
		/// Gets the list of generated domain model types (classes, rules, relationships).
		/// </summary>
		/// <returns>List of types.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		protected sealed override global::System.Type[] GetGeneratedDomainModelTypes()
		{
			return new global::System.Type[]
			{
				typeof(NamedElement),
				typeof(ModelRoot),
				typeof(ModelClass),
				typeof(Field),
				typeof(Comment),
				typeof(ModelType),
				typeof(ClassModelElement),
				typeof(PersistentKey),
				typeof(ModelAttribute),
				typeof(BaseRelationship),
				typeof(ClassHasFields),
				typeof(ModelRootHasComments),
				typeof(Generalization),
				typeof(OneToOne),
				typeof(OneToMany),
				typeof(ModelRootHasTypes),
				typeof(CommentReferencesSubjects),
				typeof(ManyToMany),
				typeof(ClassHasPersistentKeys),
				typeof(ClassDiagram),
				typeof(RelationshipConnector),
				typeof(BidirectionalConnector),
				typeof(AggregationConnector),
				typeof(GeneralizationConnector),
				typeof(CommentConnector),
				typeof(ManyToManyConnector),
				typeof(CommentBoxShape),
				typeof(ClassShape),
				typeof(global::consist.RapidEntity.FixUpDiagram),
				typeof(global::consist.RapidEntity.DecoratorPropertyChanged),
				typeof(global::consist.RapidEntity.ConnectorRolePlayerChanged),
				typeof(global::consist.RapidEntity.CompartmentItemAddRule),
				typeof(global::consist.RapidEntity.CompartmentItemDeleteRule),
				typeof(global::consist.RapidEntity.CompartmentItemRolePlayerChangeRule),
				typeof(global::consist.RapidEntity.CompartmentItemRolePlayerPositionChangeRule),
				typeof(global::consist.RapidEntity.CompartmentItemChangeRule),
			};
		}
		/// <summary>
		/// Gets the list of generated domain properties.
		/// </summary>
		/// <returns>List of property data.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		protected sealed override DomainMemberInfo[] GetGeneratedDomainProperties()
		{
			return new DomainMemberInfo[]
			{
				new DomainMemberInfo(typeof(NamedElement), "Name", NamedElement.NameDomainPropertyId, typeof(NamedElement.NamePropertyHandler)),
				new DomainMemberInfo(typeof(ModelClass), "Kind", ModelClass.KindDomainPropertyId, typeof(ModelClass.KindPropertyHandler)),
				new DomainMemberInfo(typeof(ModelClass), "IsAbstract", ModelClass.IsAbstractDomainPropertyId, typeof(ModelClass.IsAbstractPropertyHandler)),
				new DomainMemberInfo(typeof(ModelClass), "TableName", ModelClass.TableNameDomainPropertyId, typeof(ModelClass.TableNamePropertyHandler)),
				new DomainMemberInfo(typeof(ModelClass), "ParentName", ModelClass.ParentNameDomainPropertyId, typeof(ModelClass.ParentNamePropertyHandler)),
				new DomainMemberInfo(typeof(Comment), "Text", Comment.TextDomainPropertyId, typeof(Comment.TextPropertyHandler)),
				new DomainMemberInfo(typeof(PersistentKey), "IsAutoKey", PersistentKey.IsAutoKeyDomainPropertyId, typeof(PersistentKey.IsAutoKeyPropertyHandler)),
				new DomainMemberInfo(typeof(ModelAttribute), "Type", ModelAttribute.TypeDomainPropertyId, typeof(ModelAttribute.TypePropertyHandler)),
				new DomainMemberInfo(typeof(ModelAttribute), "ColumnName", ModelAttribute.ColumnNameDomainPropertyId, typeof(ModelAttribute.ColumnNamePropertyHandler)),
				new DomainMemberInfo(typeof(ModelAttribute), "AllowNull", ModelAttribute.AllowNullDomainPropertyId, typeof(ModelAttribute.AllowNullPropertyHandler)),
				new DomainMemberInfo(typeof(ModelAttribute), "Precision", ModelAttribute.PrecisionDomainPropertyId, typeof(ModelAttribute.PrecisionPropertyHandler)),
				new DomainMemberInfo(typeof(ModelAttribute), "Scale", ModelAttribute.ScaleDomainPropertyId, typeof(ModelAttribute.ScalePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "SourceMultiplicity", BaseRelationship.SourceMultiplicityDomainPropertyId, typeof(BaseRelationship.SourceMultiplicityPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "SourceRoleName", BaseRelationship.SourceRoleNameDomainPropertyId, typeof(BaseRelationship.SourceRoleNamePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "TargetMultiplicity", BaseRelationship.TargetMultiplicityDomainPropertyId, typeof(BaseRelationship.TargetMultiplicityPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "TargetRoleName", BaseRelationship.TargetRoleNameDomainPropertyId, typeof(BaseRelationship.TargetRoleNamePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "ReferenceColumn", BaseRelationship.ReferenceColumnDomainPropertyId, typeof(BaseRelationship.ReferenceColumnPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "ReferenceEntity", BaseRelationship.ReferenceEntityDomainPropertyId, typeof(BaseRelationship.ReferenceEntityPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "OwnerEntity", BaseRelationship.OwnerEntityDomainPropertyId, typeof(BaseRelationship.OwnerEntityPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "DefaultRoleName", BaseRelationship.DefaultRoleNameDomainPropertyId, typeof(BaseRelationship.DefaultRoleNamePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "ManyRoleName", BaseRelationship.ManyRoleNameDomainPropertyId, typeof(BaseRelationship.ManyRoleNamePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "OneRoleName", BaseRelationship.OneRoleNameDomainPropertyId, typeof(BaseRelationship.OneRoleNamePropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "ReferencedKey", BaseRelationship.ReferencedKeyDomainPropertyId, typeof(BaseRelationship.ReferencedKeyPropertyHandler)),
				new DomainMemberInfo(typeof(BaseRelationship), "Type", BaseRelationship.TypeDomainPropertyId, typeof(BaseRelationship.TypePropertyHandler)),
				new DomainMemberInfo(typeof(Generalization), "Discriminator", Generalization.DiscriminatorDomainPropertyId, typeof(Generalization.DiscriminatorPropertyHandler)),
				new DomainMemberInfo(typeof(OneToOne), "RelationColumn", OneToOne.RelationColumnDomainPropertyId, typeof(OneToOne.RelationColumnPropertyHandler)),
				new DomainMemberInfo(typeof(OneToOne), "Cascade", OneToOne.CascadeDomainPropertyId, typeof(OneToOne.CascadePropertyHandler)),
				new DomainMemberInfo(typeof(OneToOne), "IsImported", OneToOne.IsImportedDomainPropertyId, typeof(OneToOne.IsImportedPropertyHandler)),
				new DomainMemberInfo(typeof(OneToMany), "RelationColumn", OneToMany.RelationColumnDomainPropertyId, typeof(OneToMany.RelationColumnPropertyHandler)),
				new DomainMemberInfo(typeof(OneToMany), "Cascade", OneToMany.CascadeDomainPropertyId, typeof(OneToMany.CascadePropertyHandler)),
				new DomainMemberInfo(typeof(ManyToMany), "JoinTable", ManyToMany.JoinTableDomainPropertyId, typeof(ManyToMany.JoinTablePropertyHandler)),
				new DomainMemberInfo(typeof(ManyToMany), "JoinColumn", ManyToMany.JoinColumnDomainPropertyId, typeof(ManyToMany.JoinColumnPropertyHandler)),
				new DomainMemberInfo(typeof(ManyToMany), "OwnerColumn", ManyToMany.OwnerColumnDomainPropertyId, typeof(ManyToMany.OwnerColumnPropertyHandler)),
				new DomainMemberInfo(typeof(ManyToMany), "Cascade", ManyToMany.CascadeDomainPropertyId, typeof(ManyToMany.CascadePropertyHandler)),
				new DomainMemberInfo(typeof(ManyToMany), "OwnerKey", ManyToMany.OwnerKeyDomainPropertyId, typeof(ManyToMany.OwnerKeyPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "EncryptedConnection", ClassDiagram.EncryptedConnectionDomainPropertyId, typeof(ClassDiagram.EncryptedConnectionPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "ConnectionString", ClassDiagram.ConnectionStringDomainPropertyId, typeof(ClassDiagram.ConnectionStringPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "NameSingularization", ClassDiagram.NameSingularizationDomainPropertyId, typeof(ClassDiagram.NameSingularizationPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "ProviderGuid", ClassDiagram.ProviderGuidDomainPropertyId, typeof(ClassDiagram.ProviderGuidPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "RemovableTablePrefixes", ClassDiagram.RemovableTablePrefixesDomainPropertyId, typeof(ClassDiagram.RemovableTablePrefixesPropertyHandler)),
				new DomainMemberInfo(typeof(ClassDiagram), "HasGrid", ClassDiagram.HasGridDomainPropertyId, typeof(ClassDiagram.HasGridPropertyHandler)),
			};
		}
		/// <summary>
		/// Gets the list of generated domain roles.
		/// </summary>
		/// <returns>List of role data.</returns>
		protected sealed override DomainRolePlayerInfo[] GetGeneratedDomainRoles()
		{
			return new DomainRolePlayerInfo[]
			{
				new DomainRolePlayerInfo(typeof(BaseRelationship), "Source", BaseRelationship.SourceDomainRoleId),
				new DomainRolePlayerInfo(typeof(BaseRelationship), "Target", BaseRelationship.TargetDomainRoleId),
				new DomainRolePlayerInfo(typeof(ClassHasFields), "ModelClass", ClassHasFields.ModelClassDomainRoleId),
				new DomainRolePlayerInfo(typeof(ClassHasFields), "Attribute", ClassHasFields.AttributeDomainRoleId),
				new DomainRolePlayerInfo(typeof(ModelRootHasComments), "ModelRoot", ModelRootHasComments.ModelRootDomainRoleId),
				new DomainRolePlayerInfo(typeof(ModelRootHasComments), "Comment", ModelRootHasComments.CommentDomainRoleId),
				new DomainRolePlayerInfo(typeof(Generalization), "Superclass", Generalization.SuperclassDomainRoleId),
				new DomainRolePlayerInfo(typeof(Generalization), "Subclass", Generalization.SubclassDomainRoleId),
				new DomainRolePlayerInfo(typeof(OneToOne), "BidirectionalSource", OneToOne.BidirectionalSourceDomainRoleId),
				new DomainRolePlayerInfo(typeof(OneToOne), "BidirectionalTarget", OneToOne.BidirectionalTargetDomainRoleId),
				new DomainRolePlayerInfo(typeof(OneToMany), "AggregationSource", OneToMany.AggregationSourceDomainRoleId),
				new DomainRolePlayerInfo(typeof(OneToMany), "AggregationTarget", OneToMany.AggregationTargetDomainRoleId),
				new DomainRolePlayerInfo(typeof(ModelRootHasTypes), "ModelRoot", ModelRootHasTypes.ModelRootDomainRoleId),
				new DomainRolePlayerInfo(typeof(ModelRootHasTypes), "Type", ModelRootHasTypes.TypeDomainRoleId),
				new DomainRolePlayerInfo(typeof(CommentReferencesSubjects), "Comment", CommentReferencesSubjects.CommentDomainRoleId),
				new DomainRolePlayerInfo(typeof(CommentReferencesSubjects), "Subject", CommentReferencesSubjects.SubjectDomainRoleId),
				new DomainRolePlayerInfo(typeof(ManyToMany), "SourceModelClass", ManyToMany.SourceModelClassDomainRoleId),
				new DomainRolePlayerInfo(typeof(ManyToMany), "TargetModelClass", ManyToMany.TargetModelClassDomainRoleId),
				new DomainRolePlayerInfo(typeof(ClassHasPersistentKeys), "ModelClass", ClassHasPersistentKeys.ModelClassDomainRoleId),
				new DomainRolePlayerInfo(typeof(ClassHasPersistentKeys), "PersistentKey", ClassHasPersistentKeys.PersistentKeyDomainRoleId),
			};
		}
		#endregion
		#region Factory methods
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementMap;
	
		/// <summary>
		/// Creates an element of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementType">Element type which belongs to this domain model.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Generated code.")]	
		public sealed override DslModeling::ModelElement CreateElement(DslModeling::Partition partition, global::System.Type elementType, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementType == null) throw new global::System.ArgumentNullException("elementType");
	
			if (createElementMap == null)
			{
				createElementMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(18);
				createElementMap.Add(typeof(ModelRoot), 0);
				createElementMap.Add(typeof(ModelClass), 1);
				createElementMap.Add(typeof(Field), 2);
				createElementMap.Add(typeof(Comment), 3);
				createElementMap.Add(typeof(PersistentKey), 4);
				createElementMap.Add(typeof(ModelAttribute), 5);
				createElementMap.Add(typeof(ClassDiagram), 6);
				createElementMap.Add(typeof(BidirectionalConnector), 7);
				createElementMap.Add(typeof(AggregationConnector), 8);
				createElementMap.Add(typeof(GeneralizationConnector), 9);
				createElementMap.Add(typeof(CommentConnector), 10);
				createElementMap.Add(typeof(ManyToManyConnector), 11);
				createElementMap.Add(typeof(CommentBoxShape), 12);
				createElementMap.Add(typeof(ClassShape), 13);
			}
			int index;
			if (!createElementMap.TryGetValue(elementType, out index))
			{
				// construct exception error message		
				string exceptionError = string.Format(
								global::System.Globalization.CultureInfo.CurrentCulture,
								global::consist.RapidEntity.RapidEntityDomainModel.SingletonResourceManager.GetString("UnrecognizedElementType"),
								elementType.Name);
				throw new global::System.ArgumentException(exceptionError, "elementType");
			}
			switch (index)
			{
				case 0: return new ModelRoot(partition, propertyAssignments);
				case 1: return new ModelClass(partition, propertyAssignments);
				case 2: return new Field(partition, propertyAssignments);
				case 3: return new Comment(partition, propertyAssignments);
				case 4: return new PersistentKey(partition, propertyAssignments);
				case 5: return new ModelAttribute(partition, propertyAssignments);
				case 6: return new ClassDiagram(partition, propertyAssignments);
				case 7: return new BidirectionalConnector(partition, propertyAssignments);
				case 8: return new AggregationConnector(partition, propertyAssignments);
				case 9: return new GeneralizationConnector(partition, propertyAssignments);
				case 10: return new CommentConnector(partition, propertyAssignments);
				case 11: return new ManyToManyConnector(partition, propertyAssignments);
				case 12: return new CommentBoxShape(partition, propertyAssignments);
				case 13: return new ClassShape(partition, propertyAssignments);
				default: return null;
			}
		}
	
		private static global::System.Collections.Generic.Dictionary<global::System.Type, int> createElementLinkMap;
	
		/// <summary>
		/// Creates an element link of specified type.
		/// </summary>
		/// <param name="partition">Partition where element is to be created.</param>
		/// <param name="elementLinkType">Element link type which belongs to this domain model.</param>
		/// <param name="roleAssignments">List of relationship role assignments for the new link.</param>
		/// <param name="propertyAssignments">New element property assignments.</param>
		/// <returns>Created element link.</returns>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public sealed override DslModeling::ElementLink CreateElementLink(DslModeling::Partition partition, global::System.Type elementLinkType, DslModeling::RoleAssignment[] roleAssignments, DslModeling::PropertyAssignment[] propertyAssignments)
		{
			if (elementLinkType == null) throw new global::System.ArgumentNullException("elementLinkType");
			if (roleAssignments == null) throw new global::System.ArgumentNullException("roleAssignments");
	
			if (createElementLinkMap == null)
			{
				createElementLinkMap = new global::System.Collections.Generic.Dictionary<global::System.Type, int>(10);
				createElementLinkMap.Add(typeof(ClassHasFields), 0);
				createElementLinkMap.Add(typeof(ModelRootHasComments), 1);
				createElementLinkMap.Add(typeof(Generalization), 2);
				createElementLinkMap.Add(typeof(OneToOne), 3);
				createElementLinkMap.Add(typeof(OneToMany), 4);
				createElementLinkMap.Add(typeof(ModelRootHasTypes), 5);
				createElementLinkMap.Add(typeof(CommentReferencesSubjects), 6);
				createElementLinkMap.Add(typeof(ManyToMany), 7);
				createElementLinkMap.Add(typeof(ClassHasPersistentKeys), 8);
			}
			int index;
			if (!createElementLinkMap.TryGetValue(elementLinkType, out index))
			{
				// construct exception error message
				string exceptionError = string.Format(
								global::System.Globalization.CultureInfo.CurrentCulture,
								global::consist.RapidEntity.RapidEntityDomainModel.SingletonResourceManager.GetString("UnrecognizedElementLinkType"),
								elementLinkType.Name);
				throw new global::System.ArgumentException(exceptionError, "elementLinkType");
			
			}
			switch (index)
			{
				case 0: return new ClassHasFields(partition, roleAssignments, propertyAssignments);
				case 1: return new ModelRootHasComments(partition, roleAssignments, propertyAssignments);
				case 2: return new Generalization(partition, roleAssignments, propertyAssignments);
				case 3: return new OneToOne(partition, roleAssignments, propertyAssignments);
				case 4: return new OneToMany(partition, roleAssignments, propertyAssignments);
				case 5: return new ModelRootHasTypes(partition, roleAssignments, propertyAssignments);
				case 6: return new CommentReferencesSubjects(partition, roleAssignments, propertyAssignments);
				case 7: return new ManyToMany(partition, roleAssignments, propertyAssignments);
				case 8: return new ClassHasPersistentKeys(partition, roleAssignments, propertyAssignments);
				default: return null;
			}
		}
		#endregion
		#region Resource manager
		
		private static global::System.Resources.ResourceManager resourceManager;
		
		/// <summary>
		/// The base name of this model's resources.
		/// </summary>
		public const string ResourceBaseName = "consist.RapidEntity.GeneratedCode.DomainModelResx";
		
		/// <summary>
		/// Gets the DomainModel's ResourceManager. If the ResourceManager does not already exist, then it is created.
		/// </summary>
		public override global::System.Resources.ResourceManager ResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				return RapidEntityDomainModel.SingletonResourceManager;
			}
		}
	
		/// <summary>
		/// Gets the Singleton ResourceManager for this domain model.
		/// </summary>
		public static global::System.Resources.ResourceManager SingletonResourceManager
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				if (RapidEntityDomainModel.resourceManager == null)
				{
					RapidEntityDomainModel.resourceManager = new global::System.Resources.ResourceManager(ResourceBaseName, typeof(RapidEntityDomainModel).Assembly);
				}
				return RapidEntityDomainModel.resourceManager;
			}
		}
		#endregion
		#region Copy/Remove closures
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter copyClosure;
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter removeClosure;
		/// <summary>
		/// Returns an IElementVisitorFilter that corresponds to the ClosureType.
		/// </summary>
		/// <param name="type">closure type</param>
		/// <param name="rootElements">collection of root elements</param>
		/// <returns>IElementVisitorFilter or null</returns>
		public override DslModeling::IElementVisitorFilter GetClosureFilter(DslModeling::ClosureType type, global::System.Collections.Generic.ICollection<DslModeling::ModelElement> rootElements)
		{
			switch (type)
			{
				case DslModeling::ClosureType.CopyClosure:
					return RapidEntityDomainModel.CopyClosure;
				case DslModeling::ClosureType.DeleteClosure:
					return RapidEntityDomainModel.DeleteClosure;
			}
			return base.GetClosureFilter(type, rootElements);
		}
		/// <summary>
		/// CopyClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter CopyClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (RapidEntityDomainModel.copyClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter copyFilter = new DslModeling::ChainingElementVisitorFilter();
					copyFilter.AddFilter(new RapidEntityCopyClosure());
					copyFilter.AddFilter(new DslModeling::CoreCopyClosure());
					copyFilter.AddFilter(new DslDiagrams::CoreDesignSurfaceCopyClosure());
					
					RapidEntityDomainModel.copyClosure = copyFilter;
				}
				return RapidEntityDomainModel.copyClosure;
			}
		}
		/// <summary>
		/// DeleteClosure cache
		/// </summary>
		private static DslModeling::IElementVisitorFilter DeleteClosure
		{
			get
			{
				// Incorporate all of the closures from the models we extend
				if (RapidEntityDomainModel.removeClosure == null)
				{
					DslModeling::ChainingElementVisitorFilter removeFilter = new DslModeling::ChainingElementVisitorFilter();
					removeFilter.AddFilter(new RapidEntityDeleteClosure());
					removeFilter.AddFilter(new DslModeling::CoreDeleteClosure());
					removeFilter.AddFilter(new DslDiagrams::CoreDesignSurfaceDeleteClosure());
		
					RapidEntityDomainModel.removeClosure = removeFilter;
				}
				return RapidEntityDomainModel.removeClosure;
			}
		}
		#endregion
		#region Diagram rule helpers
		/// <summary>
		/// Enables rules in this domain model related to diagram fixup for the given store.
		/// If diagram data will be loaded into the store, this method should be called first to ensure
		/// that the diagram behaves properly.
		/// </summary>
		public static void EnableDiagramRules(DslModeling::Store store)
		{
			if(store == null) throw new global::System.ArgumentNullException("store");
			
			DslModeling::RuleManager ruleManager = store.RuleManager;
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.FixUpDiagram));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.DecoratorPropertyChanged));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.ConnectorRolePlayerChanged));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.CompartmentItemAddRule));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.CompartmentItemDeleteRule));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.CompartmentItemRolePlayerChangeRule));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.CompartmentItemRolePlayerPositionChangeRule));
			ruleManager.EnableRule(typeof(global::consist.RapidEntity.CompartmentItemChangeRule));
		}
		
		/// <summary>
		/// Disables rules in this domain model related to diagram fixup for the given store.
		/// </summary>
		public static void DisableDiagramRules(DslModeling::Store store)
		{
			if(store == null) throw new global::System.ArgumentNullException("store");
			
			DslModeling::RuleManager ruleManager = store.RuleManager;
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.FixUpDiagram));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.DecoratorPropertyChanged));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.ConnectorRolePlayerChanged));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.CompartmentItemAddRule));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.CompartmentItemDeleteRule));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.CompartmentItemRolePlayerChangeRule));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.CompartmentItemRolePlayerPositionChangeRule));
			ruleManager.DisableRule(typeof(global::consist.RapidEntity.CompartmentItemChangeRule));
		}
		#endregion
	}
		
	#region Copy/Remove closure classes
	/// <summary>
	/// Remove closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class RapidEntityDeleteClosure : RapidEntityDeleteClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RapidEntityDeleteClosure() : base()
		{
		}
	}
	
	/// <summary>
	/// Base class for remove closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class RapidEntityDeleteClosureBase : DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Specialized.HybridDictionary domainRoles;
		/// <summary>
		/// Constructor
		/// </summary>
		public RapidEntityDeleteClosureBase()
		{
			#region Initialize DomainData Table
			DomainRoles.Add(global::consist.RapidEntity.ClassHasFields.AttributeDomainRoleId, true);
			DomainRoles.Add(global::consist.RapidEntity.ModelRootHasComments.CommentDomainRoleId, true);
			DomainRoles.Add(global::consist.RapidEntity.ModelRootHasTypes.TypeDomainRoleId, true);
			DomainRoles.Add(global::consist.RapidEntity.ClassHasPersistentKeys.PersistentKeyDomainRoleId, true);
			#endregion
		}
		/// <summary>
		/// Called to ask the filter if a particular relationship from a source element should be included in the traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="sourceRoleInfo">DomainRoleInfo of the role that the source element is playing in the relationship</param>
		/// <param name="domainRelationshipInfo">DomainRelationshipInfo for the ElementLink in question</param>
		/// <param name="targetRelationship">Relationship in question</param>
		/// <returns>Yes if the relationship should be traversed</returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRelationship(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::DomainRoleInfo sourceRoleInfo, DslModeling::DomainRelationshipInfo domainRelationshipInfo, DslModeling::ElementLink targetRelationship)
		{
			return DslModeling::VisitorFilterResult.Yes;
		}
		/// <summary>
		/// Called to ask the filter if a particular role player should be Visited during traversal
		/// </summary>
		/// <param name="walker">ElementWalker that is traversing the model</param>
		/// <param name="sourceElement">Model Element playing the source role</param>
		/// <param name="elementLink">Element Link that forms the relationship to the role player in question</param>
		/// <param name="targetDomainRole">DomainRoleInfo of the target role</param>
		/// <param name="targetRolePlayer">Model Element that plays the target role in the relationship</param>
		/// <returns></returns>
		public virtual DslModeling::VisitorFilterResult ShouldVisitRolePlayer(DslModeling::ElementWalker walker, DslModeling::ModelElement sourceElement, DslModeling::ElementLink elementLink, DslModeling::DomainRoleInfo targetDomainRole, DslModeling::ModelElement targetRolePlayer)
		{
			if (targetDomainRole == null) throw new global::System.ArgumentNullException("targetDomainRole");
			return this.DomainRoles.Contains(targetDomainRole.Id) ? DslModeling::VisitorFilterResult.Yes : DslModeling::VisitorFilterResult.DoNotCare;
		}
		/// <summary>
		/// DomainRoles
		/// </summary>
		private global::System.Collections.Specialized.HybridDictionary DomainRoles
		{
			get
			{
				if (this.domainRoles == null)
				{
					this.domainRoles = new global::System.Collections.Specialized.HybridDictionary();
				}
				return this.domainRoles;
			}
		}
	
	}
	/// <summary>
	/// Copy closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class RapidEntityCopyClosure : RapidEntityCopyClosureBase, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RapidEntityCopyClosure() : base()
		{
		}
	}
	/// <summary>
	/// Base class for copy closure visitor filter
	/// </summary>
	[global::System.CLSCompliant(true)]
	public partial class RapidEntityCopyClosureBase : DslModeling::CopyClosureFilter, DslModeling::IElementVisitorFilter
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RapidEntityCopyClosureBase():base()
		{
		}
	}
	#endregion
		
}
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainEnumeration: AccessModifier
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum AccessModifier
	{
		/// <summary>
		/// Public
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/Public.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Public = 0,
		/// <summary>
		/// Assembly
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/Assembly.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Assembly = 1,
		/// <summary>
		/// Private
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/Private.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Private = 2,
		/// <summary>
		/// Family
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/Family.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Family = 3,
		/// <summary>
		/// FamilyOrAssembly
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/FamilyOrAssembly.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		FamilyOrAssembly = 4,
		/// <summary>
		/// FamilyAndAssembly
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.AccessModifier/FamilyAndAssembly.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		FamilyAndAssembly = 5,
	}
}
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainEnumeration: TypeAccessModifier
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum TypeAccessModifier
	{
		/// <summary>
		/// Public
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.TypeAccessModifier/Public.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Public = 0,
		/// <summary>
		/// Private
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.TypeAccessModifier/Private.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Private = 1,
	}
}
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainEnumeration: InheritanceModifier
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum InheritanceModifier
	{
		/// <summary>
		/// None
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.InheritanceModifier/None.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		None = 0,
		/// <summary>
		/// Abstract
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.InheritanceModifier/Abstract.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Abstract = 1,
		/// <summary>
		/// Sealed
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.InheritanceModifier/Sealed.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Sealed = 2,
	}
}
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainEnumeration: Multiplicity
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum Multiplicity
	{
		/// <summary>
		/// ZeroMany
		/// 0..*
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.Multiplicity/ZeroMany.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		ZeroMany = 0,
		/// <summary>
		/// One
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.Multiplicity/One.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		One = 1,
		/// <summary>
		/// ZeroOne
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.Multiplicity/ZeroOne.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		ZeroOne = 2,
		/// <summary>
		/// OneMany
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.Multiplicity/OneMany.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		OneMany = 3,
	}
}
namespace consist.RapidEntity
{
	/// <summary>
	/// DomainEnumeration: OperationConcurrency
	/// </summary>
	[global::System.CLSCompliant(true)]
	public enum OperationConcurrency
	{
		/// <summary>
		/// Sequential
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.OperationConcurrency/Sequential.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Sequential = 0,
		/// <summary>
		/// Guarded
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.OperationConcurrency/Guarded.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Guarded = 1,
		/// <summary>
		/// Concurrent
		/// </summary>
		[DslDesign::DescriptionResource("consist.RapidEntity.OperationConcurrency/Concurrent.Description", typeof(global::consist.RapidEntity.RapidEntityDomainModel), "consist.RapidEntity.GeneratedCode.DomainModelResx")]
		Concurrent = 2,
	}
}

