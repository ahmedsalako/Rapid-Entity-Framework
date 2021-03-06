﻿
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using VSShellInterop = global::Microsoft.VisualStudio.Shell.Interop;
using VSShell = global::Microsoft.VisualStudio.Shell;
using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using DslDesign = global::Microsoft.VisualStudio.Modeling.Design;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using VSTextTemplatingHost = global::Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
	
namespace consist.RapidEntity.DslPackage
{
	/// <summary>
	/// This class implements the VS package that integrates this DSL into Visual Studio.
	/// </summary>
	[VSShell::DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
	[VSShell::PackageRegistration(RegisterUsing = VSShell::RegistrationMethod.Assembly, UseManagedResourcesOnly = true)]
	[VSShell::ProvideToolWindow(typeof(RapidEntityExplorerToolWindow), MultiInstances = false, Style = VSShell::VsDockStyle.Tabbed, Orientation = VSShell::ToolWindowOrientation.Right, Window = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}")]
	[VSShell::ProvideToolWindowVisibility(typeof(RapidEntityExplorerToolWindow), Constants.RapidEntityEditorFactoryId)]
	[VSShell::ProvideStaticToolboxGroup("@Class DiagramsToolboxTab;consist.RapidEntity.DslPackage.Dsl.dll", "consist.RapidEntity.DslPackage.Class DiagramsToolboxTab")]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@ModelClassToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.ModelClassToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"ModelClassF1Keyword", 
					"@ModelClassToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@BidirectionalAssociationToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.BidirectionalAssociationToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"ConnectBidirectionalAssociationF1Keyword", 
					"@BidirectionalAssociationToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@AggregationToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.AggregationToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"AggregationF1Keyword", 
					"@AggregationToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@GeneralizationToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.GeneralizationToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"GeneralizationF1Keyword", 
					"@GeneralizationToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@CommentToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.CommentToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"CommentF1Keyword", 
					"@CommentToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideStaticToolboxItem("consist.RapidEntity.DslPackage.Class DiagramsToolboxTab",
					"@CommentsReferenceTypesToolboxItem;consist.RapidEntity.DslPackage.Dsl.dll", 
					"consist.RapidEntity.DslPackage.CommentsReferenceTypesToolboxItem", 
					"CF_TOOLBOXITEMCONTAINER,CF_TOOLBOXITEMCONTAINER_HASH,CF_TOOLBOXITEMCONTAINER_CONTENTS", 
					"CommentsReferenceTypesF1Keyword", 
					"@CommentsReferenceTypesToolboxBitmap;consist.RapidEntity.DslPackage.Dsl.dll", 
					0xff00ff)]
	[VSShell::ProvideEditorFactory(typeof(RapidEntityEditorFactory), 103, TrustLevel = VSShellInterop::__VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
	[VSShell::ProvideEditorExtension(typeof(RapidEntityEditorFactory), "." + Constants.DesignerFileExtension, 50)]
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.CSharpProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::ProvideRelatedFile("." + Constants.DesignerFileExtension, Constants.DefaultDiagramExtension,
		ProjectSystem = DslShell::ProvideRelatedFileAttribute.VisualBasicProjectGuid,
		FileOptions = DslShell::RelatedFileType.FileName)]
	[DslShell::RegisterAsDslToolsEditor]
	[global::System.Runtime.InteropServices.ComVisible(true)]
	[DslShell::ProvideBindingPath]
	[DslShell::ProvideXmlEditorChooserBlockSxSWithXmlEditor(@"RapidEntity", typeof(RapidEntityEditorFactory))]
	internal abstract partial class RapidEntityPackageBase : DslShell::ModelingPackage
	{
		protected global::consist.RapidEntity.RapidEntityToolboxHelper toolboxHelper;	
		
		/// <summary>
		/// Initialization method called by the package base class when this package is loaded.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			// Register the editor factory used to create the DSL editor.
			this.RegisterEditorFactory(new RapidEntityEditorFactory(this));
			
			// Initialize the toolbox helper
			toolboxHelper = new global::consist.RapidEntity.RapidEntityToolboxHelper(this);

			// Create the command set that handles menu commands provided by this package.
			RapidEntityCommandSet commandSet = new RapidEntityCommandSet(this);
			commandSet.Initialize();
			
			// Register the model explorer tool window for this DSL.
			this.AddToolWindow(typeof(RapidEntityExplorerToolWindow));

			// Initialize Extension Registars
			// this is a partial method call
			this.InitializeExtensions();

			// Add dynamic toolbox items
			this.SetupDynamicToolbox();
		}

		/// <summary>
		/// Partial method to initialize ExtensionRegistrars (if any) in the DslPackage
		/// </summary>
		partial void InitializeExtensions();
		
		/// <summary>
		/// Returns any dynamic tool items for the designer
		/// </summary>
		/// <remarks>The default implementation is to return the list of items from the generated toolbox helper.</remarks>
		protected override global::System.Collections.Generic.IList<DslDesign::ModelingToolboxItem> CreateToolboxItems()
		{
			try
			{
				Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
				return toolboxHelper.CreateToolboxItems();
			}
			catch(global::System.Exception e)
			{
				global::System.Diagnostics.Debug.Fail("Exception thrown during toolbox item creation.  This may result in Package Load Failure:\r\n\r\n" + e);
				throw;
			}
		}
		
		
		/// <summary>
		/// Given a toolbox item "unique ID" and a data format identifier, returns the content of
		/// the data format. 
		/// </summary>
		/// <param name="itemId">The unique ToolboxItem to retrieve data for</param>
		/// <param name="format">The desired format of the resulting data</param>
		protected override object GetToolboxItemData(string itemId, DataFormats.Format format)
		{
			Debug.Assert(toolboxHelper != null, "Toolbox helper is not initialized");
		
			// Retrieve the specified ToolboxItem from the DSL
			return toolboxHelper.GetToolboxItemData(itemId, format);
		}
	}

}

//
// Package attributes which may need to change are placed on the partial class below, rather than in the main include file.
//
namespace consist.RapidEntity.DslPackage
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	/// <remarks>
	/// A package load key is required to allow this package to load when the Visual Studio SDK is not installed.
	/// Package load keys may be obtained from http://msdn.microsoft.com/vstudio/extend.
	/// Consult the Visual Studio SDK documentation for more information.
	/// [VSShell::ProvideLoadKey("Standard", Constants.ProductVersion, Constants.ProductName, Constants.CompanyName, 1)]
	/// </remarks>	
	///[VSShell::ProvideLoadKey("Standard", Constants.ProductVersion, Constants.ProductName, Constants.CompanyName,105)]
	[VSShell::ProvideMenuResource("1000.ctmenu", 1)]
	[VSShell::ProvideToolboxItems(1)]
	[VSTextTemplatingHost::ProvideDirectiveProcessor(typeof(global::consist.RapidEntity.RapidEntityDirectiveProcessor), global::consist.RapidEntity.RapidEntityDirectiveProcessor.RapidEntityDirectiveProcessorName, "A directive processor that provides access to RapidEntity files")]
	[global::System.Runtime.InteropServices.Guid(Constants.RapidEntityPackageId)]
	internal sealed partial class RapidEntityPackage : RapidEntityPackageBase
	{
	}
}