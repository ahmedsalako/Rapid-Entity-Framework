using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using consist.RapidEntity.Customizations.Descriptors;
using System.Data.Common;
using consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders;
using Microsoft.VisualStudio.Data;

namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    public partial class ImportForm : Form , IModelWizard
    {
        private LinkedListNode<UserControl> Current { get; set; }
        private LinkedList<UserControl> panels;
        public ClassDiagram ClassDiagram1 { get; set; }
        public bool IsValid { get { return true; } }
        public List<TableDescriptor> Tables { get; set; }

        public ImportForm( )
        {
            InitializeComponent( );
        }

        private void ImportForm_Load( object sender , EventArgs e )
        {
            panels = new LinkedList<UserControl>( new UserControl[] { GetUserControl( new SelectConnectionPanel( ) ) , GetUserControl( new SelectTablesPanel( ) ) } );
            cmdPrevious.Enabled = false;
            cmdFinish.Enabled = false;
            AddCurrentPanel( panels.First );
        }

        private void cmdPrevious_Click( object sender , EventArgs e )
        {
            if ( Current.Previous == panels.First )
            {
                cmdPrevious.Enabled = false;
                cmdNext.Enabled = true;
                cmdFinish.Enabled = false;
            }

            AddCurrentPanel( Current.Previous );
        }

        private void cmdNext_Click( object sender , EventArgs e )
        {
            if ( ( ( IModelWizard ) Current.Value ).IsValid )
            {                
                Execute( );

                if ( Current.Next == panels.Last )
                {
                    cmdNext.Enabled = false;
                    cmdPrevious.Enabled = true;
                    cmdFinish.Enabled = true;
                }
                
                AddCurrentPanel( Current.Next );                
            }
        }

        private void cmdFinish_Click( object sender , EventArgs e )
        {
            ((IModelWizard)panels.First.Next.Value).Execute();
            foreach ( var table in ( ( IModelWizard ) panels.First.Next.Value ).Tables )
            {
                ClassDiagram1.DragedTable = table;
                ClassDiagram1.AddArtifact( );
            }

            this.DialogResult = DialogResult.OK;
        }

        private UserControl GetUserControl( IModelWizard wizard )
        {
            wizard.ClassDiagram1 = ClassDiagram1;
            wizard.Tables = Tables;

            return wizard as UserControl;
        }

        private void AddCurrentPanel( LinkedListNode<UserControl> current )
        {
            if ( current.IsNotNull( ) )
            {
                mainContextPanel.Controls.Clear( );
                mainContextPanel.Controls.Add( current.Value );
                Current = current;

                Execute( );
            }
        }

        public void LoadTables( List<TableDescriptor> tables )
        {
            foreach( var wizard in panels.ToList() )
            {
                ( ( IModelWizard ) wizard ).Tables = tables;
            }
        }

        public void Execute( )
        {
            if ( ( ( IModelWizard ) Current.Value ).IsValid )
            {
                ( ( IModelWizard ) Current.Value ).Execute( );
                LoadData();
            }
        }

        public void LoadData( )
        {
            if ( ( ( IModelWizard ) Current.Value ).IsValid )
            {
                ( ( IModelWizard ) Current.Value ).LoadData( );
            }
        }
    }
}
