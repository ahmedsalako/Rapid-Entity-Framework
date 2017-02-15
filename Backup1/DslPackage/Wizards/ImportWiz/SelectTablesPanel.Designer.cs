namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    partial class SelectTablesPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.components = new System.ComponentModel.Container( );
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode( "Tables" , 0 , 0 );
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( SelectTablesPanel ) );
            this.TablesTreeView = new System.Windows.Forms.TreeView( );
            this.dataBaseIcons = new System.Windows.Forms.ImageList( this.components );
            this.SuspendLayout( );
            // 
            // TablesTreeView
            // 
            this.TablesTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TablesTreeView.CheckBoxes = true;
            this.TablesTreeView.ImageIndex = 0;
            this.TablesTreeView.ImageList = this.dataBaseIcons;
            this.TablesTreeView.Location = new System.Drawing.Point( 23 , 24 );
            this.TablesTreeView.Name = "TablesTreeView";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Tables";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "Tables";
            this.TablesTreeView.Nodes.AddRange( new System.Windows.Forms.TreeNode[] {
            treeNode1} );
            this.TablesTreeView.SelectedImageIndex = 0;
            this.TablesTreeView.Size = new System.Drawing.Size( 380 , 186 );
            this.TablesTreeView.TabIndex = 0;
            // 
            // dataBaseIcons
            // 
            this.dataBaseIcons.ImageStream = ( ( System.Windows.Forms.ImageListStreamer ) ( resources.GetObject( "dataBaseIcons.ImageStream" ) ) );
            this.dataBaseIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.dataBaseIcons.Images.SetKeyName( 0 , "DatabaseIcon.bmp" );
            this.dataBaseIcons.Images.SetKeyName( 1 , "FieldIcon.bmp" );
            this.dataBaseIcons.Images.SetKeyName( 2 , "PrimaryKeyIcon.bmp" );
            this.dataBaseIcons.Images.SetKeyName( 3 , "TableIcon.bmp" );
            // 
            // SelectTablesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F , 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.TablesTreeView );
            this.Name = "SelectTablesPanel";
            this.Size = new System.Drawing.Size( 418 , 227 );
            this.Load += new System.EventHandler( this.SelectTablesPanel_Load );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.TreeView TablesTreeView;
        private System.Windows.Forms.ImageList dataBaseIcons;
    }
}
