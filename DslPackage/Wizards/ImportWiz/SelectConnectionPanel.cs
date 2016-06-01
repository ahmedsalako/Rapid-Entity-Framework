using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using consist.RapidEntity.Customizations.Descriptors;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell;
using System.Data.Common;
using consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    public partial class SelectConnectionPanel : UserControl , IModelWizard
    {
        List<DataExplorerConnection> Connections = new List<DataExplorerConnection>( );
        DataConnection Connection { get; set; }
        List<TableDescriptor> IModelWizard.Tables { get; set; }
        public ClassDiagram ClassDiagram1 { get; set; }

        public bool IsValid
        {
            get
            {
                return !txtProviderGuid.Text.IsNullOrEmpty( ) && !txtConnectionString.Text.IsNullOrEmpty( );
            }
        }

        public SelectConnectionPanel( )
        {
            InitializeComponent( );
        }

        private static IEnumerable<DataExplorerConnection> FindDataConnections()
        {
            DataExplorerConnectionManager manager = ( DataExplorerConnectionManager ) Package.GetGlobalService( typeof( DataExplorerConnectionManager ) );

            foreach ( DataExplorerConnection connection in manager.GetConnections( ) )
            {
                yield return connection;
            }
        }

        public IEnumerable<TableDescriptor> GetAllTables()
        {
            DbConnection databaseConnection = ( DbConnection ) MetaBaseProvider
                .GetDataConnection( ( DataConnectionManager ) ClassDiagram1.GetService( typeof( DataConnectionManager ) ) ,
                txtConnectionString.Text , new Guid( txtProviderGuid.Text ) , true ).ConnectionSupport.ProviderObject;

            MetaBaseProvider provider = MetaBaseProvider.GetMetabaseProvider( databaseConnection );

            using ( Transaction trans = ClassDiagram1.Store.TransactionManager.BeginTransaction( "Update Connection string" ) )
            {
                ClassDiagram1.EncryptedConnection = txtConnectionString.Text;
                ClassDiagram1.ProviderGuid = new Guid( txtProviderGuid.Text );
            }

            return provider.BuildTables( databaseConnection );
        }

        private void SelectConnectionPanel_Load( object sender , EventArgs e )
        {
            Connections = FindDataConnections( ).ToList( );

            foreach( DataExplorerConnection connection in Connections )
            {
                cmbExistingConnections.Items.Add( connection.DisplayName );                
            }
        }

        private void cmbExistingConnections_SelectedIndexChanged( object sender , EventArgs e )
        {
            DataExplorerConnection connection = Connections.FirstOrDefault( c => c.DisplayName == cmbExistingConnections.Text.Trim( ) );

            if ( connection.IsNotNull( ) )
            {
                txtConnectionString.Text = connection.EncryptedConnectionString;
                txtProviderGuid.Text = connection.Provider.ToString( );
            }
            else
            {
                txtConnectionString.Text = string.Empty;
                txtProviderGuid.Text = string.Empty;
            }
        }

        public void Execute( )
        {
            ( ( ImportForm ) this.Parent.Parent ).LoadTables( GetAllTables( ).ToList( ) );
        }

        public void LoadData( )
        {

        }

        private void cmdSelectConnection_Click(object sender, EventArgs e)
        {
            ClassDiagram1.ShowConnectionDialog();
            txtConnectionString.Text = ClassDiagram1.EncryptedConnection;
            txtProviderGuid.Text = ClassDiagram1.ProviderGuid.ToString();
        }
    }
}
