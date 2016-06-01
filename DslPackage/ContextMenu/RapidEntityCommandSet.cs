using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Modeling.Shell;
using System.Collections;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using consist.RapidEntity.CodeGenerator;
using consist.RapidEntity.Customizations.IDEHelpers;
using System.Xml;
using System.Xml.Linq;
using consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Drawing;
using System.Resources;
using System.Reflection;
using consist.RapidEntity.Customizations.Descriptors;
using consist.RapidEntity.DslPackage.Wizards.ImportWiz;
using consist.RapidEntity.Customizations.CodeGenerator;

namespace consist.RapidEntity.DslPackage
{
    internal partial class RapidEntityCommandSet : RapidEntityCommandSetBase
    {
        public const string providerFile = "rapidprovider.xml";
        public Guid commandGuid = new Guid( "{8D7B9CB3-3591-47f9-B104-B7EB173E0F03}" );
        public const int cmdGenerateEntitiesID = 0x0023;
        public const int cmdExportToDatabaseID = 0x0024;
        public const int cmdGridViewID = 0x0300;
        public const int cmdViewCodeID = 0x0301;
        public const int cmdZoomOutID = 0x0028;
        public const int cmdZoomInID = 0x0027;
        public const int cmdZoomToFitID = 0x0031;
        public const int cmdAutoLayoutID = 0x0032;
        public const int cmdExportToFileID = 0x0033;
        public const int cmdImportFromDataStoreID = 0x0034;
        public const int cmdGenerateEntitiesWithXmlMappingID = 0x0021;

        protected override IList<MenuCommand> GetMenuCommands( )
        {
            IList<MenuCommand> commands = base.GetMenuCommands( );

            DynamicStatusMenuCommand cmdGenerateEntities =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnPopUpMenuClick ) ,
                         new CommandID( commandGuid , cmdGenerateEntitiesID ) );

            DynamicStatusMenuCommand cmdGenerateEntitiesWithXmlMapping =
                    new DynamicStatusMenuCommand(
                         new EventHandler(OnPopUpMenuDisplayAction),
                         new EventHandler(OnGenerateEntitiesWithXmlMappingMenuClick),
                         new CommandID(commandGuid, cmdGenerateEntitiesWithXmlMappingID));

            DynamicStatusMenuCommand cmdExportToDataStore =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnDataStoreMenuClick ) ,
                         new CommandID( commandGuid , cmdExportToDatabaseID ) );

            DynamicStatusMenuCommand cmdViewCode =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnViewCodeMenuClick ) ,
                         new CommandID( commandGuid , cmdViewCodeID ) );

            DynamicStatusMenuCommand cmdGridView =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnGridViewMenuClick ) ,
                         new CommandID( commandGuid , cmdGridViewID ) );

            DynamicStatusMenuCommand cmdZoomIn =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnZoomMenuClick ) ,
                         new CommandID( commandGuid , cmdZoomInID ) );

            DynamicStatusMenuCommand cmdZoomOut =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnZoomMenuClick ) ,
                         new CommandID( commandGuid , cmdZoomOutID ) );

            DynamicStatusMenuCommand cmdZoomToFit =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnZoomMenuClick ) ,
                         new CommandID( commandGuid , cmdZoomToFitID ) );

            DynamicStatusMenuCommand cmdAutoLayout =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnAutoLayoutClick ) ,
                         new CommandID( commandGuid , cmdAutoLayoutID ) );

            DynamicStatusMenuCommand cmdExportToFile =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnDataStoreMenuClick ) ,
                         new CommandID( commandGuid , cmdExportToFileID ) );

            DynamicStatusMenuCommand cmdImportFromDatabase =
                    new DynamicStatusMenuCommand(
                         new EventHandler( OnPopUpMenuDisplayAction ) ,
                         new EventHandler( OnMetadataImportMenuClick ) ,
                         new CommandID( commandGuid , cmdImportFromDataStoreID ) );

            commands.Add( cmdGenerateEntities );
            commands.Add(cmdGenerateEntitiesWithXmlMapping);
            commands.Add( cmdExportToDataStore );
            commands.Add( cmdViewCode );
            commands.Add( cmdGridView );
            commands.Add( cmdZoomIn );
            commands.Add( cmdZoomOut );
            commands.Add( cmdZoomToFit );
            commands.Add( cmdAutoLayout );
            commands.Add( cmdExportToFile );
            commands.Add( cmdImportFromDatabase );

            return commands;
        }

        internal void OnPopUpMenuDisplayAction( object sender , EventArgs e )
        {
            MenuCommand command = sender as MenuCommand;
            int commandId = command.CommandID.ID;

            foreach ( object selection in CurrentSelection )
            {
                if ( selection is ClassDiagram && commandId != cmdViewCodeID )
                {
                    ClassDiagram classDiagram = ( ClassDiagram ) selection;

                    if ( classDiagram.Store.DomainModels.Count > 0 )
                    {
                        command.Enabled = true;
                        return;
                    }
                }
                else if ( selection is ClassShape )
                {
                    if ( commandId == cmdExportToDatabaseID || commandId == cmdViewCodeID )
                    {
                        command.Enabled = true;
                        return;
                    }
                }
            }

            command.Enabled = false;
        }

        //Coded on my way to Cardiff central from London Paddignton
        //To be refactored later.

        internal void OnZoomMenuClick( object sender , EventArgs e )
        {
            int commandId = ( ( DynamicStatusMenuCommand ) sender ).CommandID.ID;
            if ( commandId == cmdZoomInID )
            {
                this.CurrentRapidEntityDocView.CurrentDesigner.ZoomIn( );
                return;
                //this.CurrentRapidEntityDocView.CurrentDesigner.ZoomAtViewCenter(150);
            }
            else if ( commandId == cmdZoomOutID )
            {
                this.CurrentRapidEntityDocView.CurrentDesigner.ZoomOut( );
            }
            else if ( commandId == cmdZoomToFitID )
            {
                this.CurrentRapidEntityDocView.CurrentDesigner.ZoomToFit( );
            }
        }

        internal void OnGridViewMenuClick( object sender , EventArgs e )
        {
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;
            classDiagram.AddBackgroundImage( );
        }

        internal void OnAutoLayoutClick( object sender , EventArgs e )
        {
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;
            using ( Transaction transaction = classDiagram.Store.TransactionManager.BeginTransaction( "Auto Layout" ) )
            {
                classDiagram.AutoLayoutShapes( );
                transaction.Commit( );
            }
        }

        internal void OnViewCodeMenuClick( object sender , EventArgs e )
        {
            MenuCommand command = sender as MenuCommand;

            string docData = this.CurrentRapidEntityDocData.FileName;
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;
            SolutionExplorerHelper.SetActiveIDE( classDiagram );
            string fullFilePath = SolutionExplorerHelper.GetSolutionProjectsBySolutionItemPath( Path.GetDirectoryName( this.CurrentDocData.FileName ) );

            if ( string.IsNullOrEmpty( fullFilePath ) )
                return;

            string directoryPath = Path.GetDirectoryName( fullFilePath );
            directoryPath = Path.Combine( directoryPath , "GeneratedCode" );

            foreach ( ModelClass model in GetSelectedModels( ) )
            {
                string className = model.Name + ".cs";
                string codePath = Path.Combine( directoryPath , className );

                string projectName = SolutionExplorerHelper.GetProjectNameByFullName( fullFilePath );
                SolutionExplorerHelper.ViewCode( projectName , codePath );
            }
        }

        internal void OnMetadataImportMenuClick( object sender , EventArgs e )
        {
            MenuCommand command = sender as MenuCommand;
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;

            if ( command.CommandID.ID == cmdImportFromDataStoreID )
            {
                ImportForm fm = new ImportForm( );
                fm.ClassDiagram1 = classDiagram;                

                fm.ShowDialog( );

                OnAutoLayoutClick( sender , e );
            }
        }

        internal void OnDataStoreMenuClick( object sender , EventArgs e )
        {
            MenuCommand command = sender as MenuCommand;
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;

            if ( command.CommandID.ID == cmdExportToDatabaseID )
            {
                //Resolve Diagram changes and export to database
                GenerateTable( classDiagram );
            }
            else if ( command.CommandID.ID == cmdExportToFileID )
            {
                GenerateScript( classDiagram );
            }
        }

        private void GenerateScript( ClassDiagram classDiagram )
        {
            DbConnection databaseConnection = ( DbConnection ) MetaBaseProvider
                .GetDataConnection( ( DataConnectionManager ) classDiagram.GetService( typeof( DataConnectionManager ) ) ,
                classDiagram.EncryptedConnection , classDiagram.ProviderGuid , true ).ConnectionSupport.ProviderObject;

            MetaBaseProvider provider = MetaBaseProvider.GetMetabaseProvider( databaseConnection );

            StringBuilder script = new StringBuilder( );

            foreach ( ModelClass model in GetSelectedModels( ) )
            {
                if ( model.ParentIsAbstractOrNoParent( ) )
                {
                    script.AppendLine( provider.GenerateCreateScript( model ).ToString( ) );
                }
            }

            foreach ( ModelClass model in GetSelectedModels( ) )
            {
                if ( model.ParentIsAbstractOrNoParent( ) )
                {
                    StringBuilder content = provider.GenerateRelationshipScript( model );

                    if ( content.IsNotNull( ) )
                    {
                        script.AppendLine( content.ToString( ) );
                    }
                }
            }

            SolutionExplorerHelper.SetActiveIDE( classDiagram );
            string fullFilePath = SolutionExplorerHelper.GetSolutionProjectsBySolutionItemPath( Path.GetDirectoryName( this.CurrentDocData.FileName ) );

            if ( string.IsNullOrEmpty( fullFilePath ) )
                return;

            string directoryPath = Path.GetDirectoryName( fullFilePath );

            DirectoryInfo directoryInfo = Directory.CreateDirectory( Path.Combine( directoryPath , "GeneratedCode" ) );
            CodeDomHelper.GenerateSQLCode( directoryInfo.FullName , Path.GetFileNameWithoutExtension( this.CurrentDocData.FileName ) , script );

            string projectName = SolutionExplorerHelper.GetProjectNameByFullName( fullFilePath );
            SolutionExplorerHelper.AddFromFileBasedOnFileExtension( projectName , "RapidEntities" , directoryInfo , ".sql" , false );
            SolutionExplorerHelper.RefereshProject( projectName );
        }

        private IEnumerable<ModelClass> GetSelectedModels( )
        {
            foreach ( object selection in CurrentSelection )
            {
                if ( selection is ClassDiagram )
                {
                    ClassDiagram classDiagram = ( ClassDiagram ) selection;

                    if ( classDiagram.Store.DomainModels.Count > 0 )
                    {
                        foreach ( ModelClass modelClass in classDiagram.Store.GetAllModelClass( ) )
                            yield return modelClass;
                    }
                }
                else if ( selection is ClassShape )
                {
                    ClassShape classShape = selection as ClassShape;
                    yield return classShape.ModelElement as ModelClass;
                }
            }
        }

        private void GenerateTable( ClassDiagram classDiagram )
        {
            try
            {
                DbConnection databaseConnection = ( DbConnection ) MetaBaseProvider
                    .GetDataConnection( ( DataConnectionManager ) classDiagram.GetService( typeof( DataConnectionManager ) ) , classDiagram.EncryptedConnection , classDiagram.ProviderGuid , true ).ConnectionSupport.ProviderObject;

                MetaBaseProvider provider = MetaBaseProvider.GetMetabaseProvider( databaseConnection );
                StatusBarHelper.Initialise( classDiagram );
                StatusBarHelper.InitialiseProgressBar( );

                foreach ( ModelClass model in GetSelectedModels( ) )
                {
                    if ( model.ParentIsAbstractOrNoParent( ) )
                    {
                        StatusBarHelper.IncrementProgress( 10 , string.Format( "Attempting {0} creation if not exist" , model.TableName ) );
                        provider.CreateTable( model );
                    }
                }

                foreach ( ModelClass model in GetSelectedModels( ) )
                {
                    if ( model.ParentIsAbstractOrNoParent( ) )
                    {
                        StatusBarHelper.IncrementProgress( 10 , string.Format( "Syncronizing metadata {0} on {1} database." , model.TableName , databaseConnection.Database ) );
                        provider.SyncronizeMetaData( model );
                    }
                }

                foreach ( ModelClass model in GetSelectedModels( ) )
                {
                    if ( model.ParentIsAbstractOrNoParent( ) )
                    {
                        StatusBarHelper.IncrementProgress( 10 , string.Format( "Syncronizing foreignkey informations for {0}." , model.TableName ) );
                        provider.SyncronizeForeignKeys( model );
                    }
                }

                StatusBarHelper.IncrementProgress( 10 , "Releasing syncronization" );

                databaseConnection.Close( );
            }
            catch ( Exception x )
            {
                throw;
            }
            finally
            {
                StatusBarHelper.ClearProgress( );
            }
        }

        internal void OnPopUpMenuClick( object sender , EventArgs e )
        {
            MenuCommand command = sender as MenuCommand;

            string tempCodeLocation = "RapidEntities";

            string docData = this.CurrentRapidEntityDocData.FileName;
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;
            SolutionExplorerHelper.SetActiveIDE( classDiagram );

            string fullFilePath = SolutionExplorerHelper.GetSolutionProjectsBySolutionItemPath( Path.GetDirectoryName( this.CurrentDocData.FileName ) );

            
            if ( string.IsNullOrEmpty( fullFilePath ) )
                return;

            string directoryPath = Path.GetDirectoryName( fullFilePath );

            //StreamReader templateContent = new StreamReader(Path.Combine(directoryPath, templateName));
            DirectoryInfo directoryInfo = Directory.CreateDirectory( Path.Combine( directoryPath , "GeneratedCode" ) );
            CodeDomHelper.Generate( directoryInfo.FullName , SolutionExplorerHelper.GetProjectDefaultNamespace( fullFilePath ) , ( ModelRoot ) this.CurrentDocData.RootElement );

            string projectName = SolutionExplorerHelper.GetProjectNameByFullName( fullFilePath );
            SolutionExplorerHelper.AddFromFileCopy( projectName , tempCodeLocation , directoryInfo );
            SolutionExplorerHelper.RefereshProject( projectName );
        }

        internal void OnGenerateEntitiesWithXmlMappingMenuClick(object sender, EventArgs e)
        {
            MenuCommand command = sender as MenuCommand;

            string tempCodeLocation = "RapidEntities";

            string docData = this.CurrentRapidEntityDocData.FileName;
            ClassDiagram classDiagram = this.CurrentRapidEntityDocView.Diagram as ClassDiagram;
            SolutionExplorerHelper.SetActiveIDE(classDiagram);

            string fullFilePath = SolutionExplorerHelper.GetSolutionProjectsBySolutionItemPath(Path.GetDirectoryName(this.CurrentDocData.FileName));


            if (string.IsNullOrEmpty(fullFilePath))
                return;

            string directoryPath = Path.GetDirectoryName(fullFilePath);

            //StreamReader templateContent = new StreamReader(Path.Combine(directoryPath, templateName));
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(directoryPath, "GeneratedCode"));
            //CodeDomHelper.Generate(directoryInfo.FullName, SolutionExplorerHelper.GetProjectDefaultNamespace(fullFilePath), (ModelRoot)this.CurrentDocData.RootElement);

            XmlGeneratorHelper.Generate(directoryInfo.FullName, SolutionExplorerHelper.GetProjectDefaultNamespace(fullFilePath), (ModelRoot)this.CurrentDocData.RootElement);

            string projectName = SolutionExplorerHelper.GetProjectNameByFullName(fullFilePath);

            SolutionExplorerHelper.AddFromFileCopy(projectName, tempCodeLocation, directoryInfo);
            SolutionExplorerHelper.AddFromFileBasedOnFileExtension(projectName, "RapidEntities", directoryInfo , ".xml" , true );
            SolutionExplorerHelper.RefereshProject(projectName);
        }

        private static void DeleteProjectDirectory( string tempCodeLocation , string directoryPath )
        {
            try
            {
                Directory.Delete( Path.Combine( directoryPath , tempCodeLocation ) , true );
            }
            catch ( Exception x )
            {

            }
        }

    }
}
