using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Windows.Forms;
using EnvDTE80;
using consist.RapidEntity.Customizations.IDEHelpers;
using System.IO;
using System.Data;
using Microsoft.VisualStudio.Modeling;
using System.Diagnostics;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Data.Interop;
using System.Data.SqlClient;
using System.Data.OleDb;
using consist.RapidEntity.Customizations.Descriptors;
using consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders;
using System.Xml.Linq;
using Microsoft.VisualStudio.Data.Services;
using consist.RapidEntity.Customizations.CodeGenerator;
using consist.RapidEntity.CodeGenerator;
using System.Drawing;
using System.Reflection;

namespace consist.RapidEntity
{
    public partial class ClassDiagram
    {
        private const string ToolboxDataFormat = "CF_TOOLBOXITEMCONTAINER_CONTENTS";
        public TableDescriptor DragedTable = new TableDescriptor( );
        static Stream image;

        static ClassDiagram( )
        {
            image = Assembly.GetAssembly( typeof( ClassDiagram ) )
                .GetManifestResourceStream( "consist.RapidEntity.Resources.grid2.JPG" );
        }

        protected override void InitializeResources( StyleSet classStyleSet )
        {
            base.InitializeResources( classStyleSet );

            if ( HasGrid )
            {
                AddBackgroundImage( );
            }
        }

        public void RemoveBackgroundImage( )
        {
            ShapeField shapeField = shapeFields.Where( s => s.Name == "background" ).FirstOrDefault( );

            if ( !shapeField.IsNull( ) )
            {
                HasGrid = false;
                shapeFields.Remove( shapeField );
                base.InitializeInstanceResources( );
                base.FocusedDiagramView.Refresh( );
                base.FocusedDiagramView.Focus( );
            }
        }

        public bool ContainsBackground( )
        {
            ShapeField shapeField = shapeFields.Where( s => s.Name == "background" ).FirstOrDefault( );
            return ( !shapeField.IsNull( ) );
        }

        public void AutoLayoutShapes( )
        {
            this.AutoLayoutShapeElements( this.NavigationRoot.NestedChildShapes );
            //LayoutEventArgs args = new LayoutEventArgs(
            //this.ActiveDiagramView.LayoutEngine.Layout(this,
        }

        public void AddBackgroundImage( )
        {
            using ( Transaction transaction = this.Store.TransactionManager.BeginTransaction( "Show Grid" ) )
            {
                Image background = Image.FromStream( image );
                this.AddBackgroundImage( background );
                transaction.Commit( );
            }
        }

        public void AddBackgroundImage( Image backgroundImage )
        {
            if ( ContainsBackground( ) )
            {
                RemoveBackgroundImage( );
                return;
            }

            HasGrid = true;
            ImageField backgroundField = new ImageField( "background" , backgroundImage );

            backgroundField.DefaultFocusable = false;
            backgroundField.DefaultSelectable = false;
            backgroundField.DefaultVisibility = true;
            backgroundField.DefaultUnscaled = false;

            shapeFields.Add( backgroundField );

            backgroundField.AnchoringBehavior.SetTopAnchor( AnchoringBehavior.Edge.Top , 0.01 );
            backgroundField.AnchoringBehavior.SetLeftAnchor( AnchoringBehavior.Edge.Left , 0.01 );
            backgroundField.AnchoringBehavior.SetRightAnchor( AnchoringBehavior.Edge.Right , 0.01 );
            backgroundField.AnchoringBehavior.SetBottomAnchor( AnchoringBehavior.Edge.Bottom , 0.01 );

            base.InitializeInstanceResources( );

            if ( FocusedDiagramView.IsNotNull( ) )
            {
                base.FocusedDiagramView.Refresh( );
                base.FocusedDiagramView.Focus( );
            }
        }

        public override StyleSet StyleSet
        {
            get
            {
                return base.StyleSet;
            }
        }

        protected override void OnAssociated( DiagramAssociationEventArgs e )
        {
            base.OnAssociated( e );

            if ( e.DiagramView == null || e.DiagramView.DiagramClientView == null )
            {
                return;
            }

            Control ctrl = e.DiagramView.DiagramClientView;
            ctrl.AllowDrop = true;
            ctrl.DragOver += new DragEventHandler( ctrl_DragOver );
            ctrl.DragDrop += new DragEventHandler( ctrl_DragDrop );
        }

        void ctrl_DragDrop( object sender , DragEventArgs e )
        {
            //Is Drag Drop from Database Server Explorer
            if ( e.Data.GetDataPresent( ServerExplorer.DataSourceFormat ) )
            {
                using ( ServerExplorer explorer = new ServerExplorer( e.Data.GetData( ServerExplorer.DataSourceFormat ) as Stream ) )
                {
                    if ( explorer.ContainsOnlyTables )
                    {
                        e.Effect = DragDropEffects.Move;
                    }

                    string connectionString = explorer.GetConnectionName( );
                    string encryptedConnectionString;
                    Guid providerGuid = Guid.NewGuid( );

                    DbConnection connection = ( DbConnection ) MetaBaseProvider.GetDataConnection( connectionString , out providerGuid , out encryptedConnectionString ).ConnectionSupport.ProviderObject;
                    DataTable schemaInfo = new DataTable( );

                    using ( Transaction updateDiagramTransaction = Store.TransactionManager.BeginTransaction( "Update Connection string" ) )
                    {
                        this.EncryptedConnection = encryptedConnectionString;
                        this.ConnectionString = connectionString;
                        this.ProviderGuid = providerGuid;
                        updateDiagramTransaction.Commit( );
                    }

                    MetaBaseProvider metaData = MetaBaseProvider.GetMetabaseProvider( connection );

                    foreach ( string tableName in GetSelectedNodeNames( ) )
                    {
                        if ( GetCurrentElements( ).Where( m => m.TableName == tableName.Trim( ) ).Count( ) > 0 )
                            continue;

                        DragedTable = metaData.BuildTable( tableName , connection );

                        AddArtifact( );
                    }

                    connection.Close( );
                }
            }
            else if ( e.Data.GetDataPresent( SolutionExplorerHelper.DataSourceFormat ) )
            {
                string codePath = e.Data.GetData( typeof( string ) ) as string;
                SolutionExplorerHelper.SetActiveIDE( this );
                string fullFilePath = SolutionExplorerHelper.GetSolutionProjectsBySolutionItemPath( Path.GetDirectoryName( codePath ) );

                if ( fullFilePath.IsNullOrEmpty( ) )
                    return;

                string directoryPath = Path.GetDirectoryName( fullFilePath );
                string projectName = SolutionExplorerHelper.GetProjectNameByFullName( fullFilePath );

                ProjectItem projectItem = SolutionExplorerHelper.GetProjectItemByFullPath( SolutionExplorerHelper
                    .GetSolutionProject( projectName ) , codePath );

                CodeParser.ExamineProjectItem( projectItem , this );

            }
            else if ( e.Data.GetDataPresent( ToolboxDataFormat ) )
            {
                if ( this.ModelElement is ModelRoot )
                {
                    ModelRoot model = this.ModelElement as ModelRoot;
                }
            }
        }

        void ctrl_DragOver( object sender , DragEventArgs e )
        {
            if ( e.Data.GetDataPresent( ServerExplorer.DataSourceFormat ) )
            {
                using ( ServerExplorer explorer = new ServerExplorer( e.Data.GetData( ServerExplorer.DataSourceFormat ) as Stream ) )
                {
                    if ( explorer.ContainsOnlyTables )
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
            else if ( e.Data.GetDataPresent( SolutionExplorerHelper.DataSourceFormat ) )
            {
                string codePath = e.Data.GetData( typeof( string ) ) as string;

                if ( codePath.Contains( ".cs" ) || codePath.Contains( ".vb" ) )
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        public void AddArtifact( )
        {
            using ( Transaction addTransaction = this.Store.TransactionManager.BeginTransaction( "Add classes" ) )
            {
                Store store = this.Store;
                ModelRoot root = GetModelRoot( );

                if ( null == root )
                    return;
                
                string name = string.Empty;
                string[] prefixes = this.RemovableTablePrefixes.Split( ',' );

                if ( this.NameSingularization )
                {
                    name = CodeDomHelper.ReformatClassName( CodeDomHelper.RemovePrefix( DragedTable.TableName , prefixes ) );
                }
                else
                {
                    name = GrammarHelper.PascalCase( CodeDomHelper.RemovePrefix( DragedTable.TableName , prefixes ) );
                }

                ModelClass model = GetElementByTableName( name );
                if ( model.IsNull( ) )
                {
                    model = new ModelClass( store );
                    model.Name = name;
                }
                else
                {
                    model.Name = name;
                }

                model.TableName = DragedTable.TableName;

                AddModelProperties( store , model );
                AddRelationFromReferenceSide( model );

                model.TableDescriptor = DragedTable;
                model.ModelRoot = root;

                AddRelationFromOwningSide( model );

                addTransaction.Commit( );
            }
        }

        private void AddModelProperties( Store store , ModelClass model )
        {
            int count = 0;
            foreach ( ColumnDescriptor column in DragedTable.Columns )
            {
                if ( column.IsPrimaryKey )
                {
                    PersistentKey key = new PersistentKey( store );
                    key.AllowNull = column.IsNullable;
                    key.ColumnName = column.ColunmName;
                    key.Name = column.ColunmName;
                    key.IsAutoKey = column.IsAutoIncrement;
                    key.Type = column.LanguageDataType;
                    key.Precision = column.Precision;
                    key.Scale = column.Scale;

                    model.PersistentKeys.Add( key );
                }
                else if ( !column.IsForeignKey )
                {
                    Field field = new Field( store );
                    field.AllowNull = column.IsNullable;
                    field.ColumnName = column.ColunmName;
                    field.Name = column.ColunmName;
                    field.Type = column.LanguageDataType;
                    field.Precision = column.Precision;
                    field.Scale = column.Scale;

                    model.Fields.Add( ( Field ) field );
                }
                count = count + 1;
            }
        }

        private void AddRelationFromReferenceSide( ModelClass model )
        {
            foreach ( ColumnDescriptor foreignKey in DragedTable.ForeignKeys )
            {
                ModelClass modelClass = GetElementByTableName( foreignKey.ForeignKeyOwnerName );

                if ( null != modelClass )
                {
                    if ( modelClass.TableName == model.TableName )
                    {//Self Join
                        AddSelfJoin( model , foreignKey , modelClass );
                    }
                    else
                    {
                        AddOneToManyJoin( modelClass , foreignKey , model );
                    }
                }
            }
        }

        private void AddOneToManyJoin( ModelClass model , ColumnDescriptor foreignKey , ModelClass modelClass )
        {
            OneToMany oneToMany = new OneToMany( model , modelClass );
            oneToMany.OwnerEntity = GetElementByTableName( foreignKey.ForeignKeyOwnerName ).Name;
            oneToMany.RelationColumn = foreignKey.ForeignKeyColumnName;
            oneToMany.ReferenceColumn = foreignKey.ForeignKeyColumnName;
            oneToMany.ReferenceEntity = modelClass.Name;
            oneToMany.ReferencedKey = foreignKey.ForeignKeyOwnerColumn;
            oneToMany.Type = foreignKey.LanguageDataType;
        }

        private void AddSelfJoin( ModelClass model , ColumnDescriptor foreignKey , ModelClass modelClass )
        {
            OneToOne oneToOne = new OneToOne( model , modelClass );
            oneToOne.RelationColumn = foreignKey.ForeignKeyColumnName;
            oneToOne.ReferenceEntity = GetElementByTableName( foreignKey.ForeignKeyOwnerName ).Name;
            oneToOne.ReferenceColumn = foreignKey.ForeignKeyColumnName;
            oneToOne.ReferencedKey = foreignKey.ForeignKeyOwnerColumn;
            oneToOne.Type = foreignKey.LanguageDataType;
        }

        private void AddRelationFromOwningSide( ModelClass model )
        {
            using ( Transaction addReferencesTransaction = this.Store.TransactionManager.BeginTransaction( "Add References" ) )
            {
                List<ModelClass> references = GetReferences( DragedTable.TableName ).ToList<ModelClass>( );

                if ( references.Count( ) > 0 )
                {
                    foreach ( ModelClass modelClass in references )
                    {
                        if ( modelClass.Name != model.Name )
                        {
                            ColumnDescriptor column = modelClass.TableDescriptor.ForeignKeys.Where( f => f.ForeignKeyOwnerName == DragedTable.TableName ).FirstOrDefault( );

                            AddOneToManyJoin( model , column , modelClass );
                        }
                    }
                }

                addReferencesTransaction.Commit( );
            }
        }

        public ModelRoot GetModelRoot( )
        {
            foreach ( ModelElement element in this.Store.ElementDirectory.AllElements )
                if ( element is ModelRoot )
                    return ( ModelRoot ) element;

            return null;
        }

        public IEnumerable<ModelClass> GetReferences( string name )
        {
            return from modelClass in GetCurrentElements( )
                   where modelClass.TableDescriptor.ForeignKeys.Where( c => c.ForeignKeyOwnerName == name ).Count( ) > 0
                   select modelClass;
        }

        public ModelClass GetElementByTableName( string name )
        {
            return GetCurrentElements( ).Where( c => c.TableName == name ).FirstOrDefault( );
        }

        public IEnumerable<ModelClass> GetCurrentElements( )
        {
            foreach ( ModelElement element in this.Store.ElementDirectory.AllElements )
                if ( element is ModelClass )
                    yield return ( ModelClass ) element;
        }

        public IEnumerable<string> GetSelectedNodeNames( )
        {
            EnvDTE80.DTE2 activeIDE = this.GetService( typeof( EnvDTE.DTE ) ) as EnvDTE80.DTE2;
            EnvDTE.Window serverExplore = ( EnvDTE.Window ) activeIDE.Windows.Item( EnvDTE.Constants.vsWindowKindServerExplorer );

            UIHierarchy root = ( UIHierarchy ) serverExplore.Object;

            foreach ( UIHierarchyItem item in ( Array ) root.SelectedItems )
                yield return item.Name;
        }

        public void ShowConnectionDialog()
        {
            string connection = ConnectionString;
            IVsDataConnectionDialogFactory dataDialog = GetService(typeof(IVsDataConnectionDialogFactory)) as IVsDataConnectionDialogFactory;
            IVsDataConnectionDialog connectionDialog = dataDialog.CreateConnectionDialog();
            connectionDialog.AddAllSources();

            if (connectionDialog.ShowDialog())
            {                    
                using (Transaction updateDiagramTransaction = Store.TransactionManager.BeginTransaction("Update Connection string"))
                {
                    connection = connectionDialog.DisplayConnectionString;
                    ProviderGuid = connectionDialog.SelectedProvider;
                    ConnectionString = connectionDialog.DisplayConnectionString;
                    EncryptedConnection = connectionDialog.EncryptedConnectionString;

                    updateDiagramTransaction.Commit();
                }
            }
        }
    }
}
