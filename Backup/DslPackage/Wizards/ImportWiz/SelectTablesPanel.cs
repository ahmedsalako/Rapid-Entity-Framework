using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using consist.RapidEntity.Customizations.Descriptors;

namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    public partial class SelectTablesPanel : UserControl , IModelWizard
    {
        public List<TableDescriptor> Tables { get; set; }
        public ClassDiagram ClassDiagram1 { get; set; }

        public bool IsValid
        {
            get { return true; }
        }

        public SelectTablesPanel( )
        {
            InitializeComponent( );
        }

        private void SelectTablesPanel_Load( object sender , EventArgs e )
        {
            LoadTreeNodes( );
        }

        private void LoadTreeNodes( )
        {
            TablesTreeView.Nodes[0].Nodes.Clear();
            TablesTreeView.Nodes[0].Nodes.AddRange( LoadTable( ).ToArray( ) );
        }

        private IEnumerable<TreeNode> LoadTable()
        {
            foreach ( TableDescriptor table in Tables )
            {
                TreeNode node = new TreeNode( table.TableName , 3 , 3 , LoadProperties( table.Columns ).ToArray() );
                node.Checked = true;
                yield return node;
            }
        }

        private IEnumerable<TreeNode> LoadProperties( List<ColumnDescriptor> columns )
        {
            foreach ( ColumnDescriptor column in columns )
            {
                int imageIndex = column.IsPrimaryKey ? 2 : 1;
                TreeNode node = new TreeNode( column.ColunmName , imageIndex , imageIndex );
                node.Checked = true;
                yield return node;
            }
        }

        public void Execute( )
        {
            foreach ( TreeNode node in TablesTreeView.Nodes[0].Nodes )
            {
                TableDescriptor table = Tables.FirstOrDefault( t => t.TableName == node.Text );
                if ( node.Checked )
                {
                    foreach ( TreeNode child in node.Nodes )
                    {
                        if ( !child.Checked )
                        {
                            ColumnDescriptor column = table.Columns.FirstOrDefault( c => c.ColunmName == child.Text );
                            table.Columns.Remove( column );
                        }
                    }
                }
                else
                {
                    Tables.Remove( table );
                }
            }
        }

        public void LoadData( )
        {
            LoadTreeNodes( );
        }
    }
}
