namespace consist.RapidEntity.GUIExtension
{
    partial class KeyResolverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmdResolve = new System.Windows.Forms.Button();
            this.lsPersistentKeys = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.keyResolverGrid = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.keyResolverGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Visible Columns";
            // 
            // cmdResolve
            // 
            this.cmdResolve.Location = new System.Drawing.Point(199, 288);
            this.cmdResolve.Name = "cmdResolve";
            this.cmdResolve.Size = new System.Drawing.Size(188, 33);
            this.cmdResolve.TabIndex = 2;
            this.cmdResolve.Text = "Resolve Persistent Keys";
            this.cmdResolve.UseVisualStyleBackColor = true;
            this.cmdResolve.Click += new System.EventHandler(this.cmdResolve_Click);
            // 
            // lsPersistentKeys
            // 
            this.lsPersistentKeys.FormattingEnabled = true;
            this.lsPersistentKeys.ItemHeight = 18;
            this.lsPersistentKeys.Location = new System.Drawing.Point(168, 35);
            this.lsPersistentKeys.Name = "lsPersistentKeys";
            this.lsPersistentKeys.Size = new System.Drawing.Size(355, 76);
            this.lsPersistentKeys.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(346, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Choose or add column(s) to bound this relationship";
            // 
            // keyResolverGrid
            // 
            this.keyResolverGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.keyResolverGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnType});
            this.keyResolverGrid.Location = new System.Drawing.Point(12, 131);
            this.keyResolverGrid.Name = "keyResolverGrid";
            this.keyResolverGrid.Size = new System.Drawing.Size(514, 150);
            this.keyResolverGrid.TabIndex = 7;
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Column Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.Width = 200;
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Column Type";
            this.ColumnType.Items.AddRange(new object[] {
            "Int32",
            "Double",
            "String",
            "long",
            "DateTime",
            "float",
            "Boolean",
            "Guid",
            "Char",
            "Byte",
            "Int64",
            "Int16"});
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnType.Width = 200;
            // 
            // KeyResolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 337);
            this.Controls.Add(this.keyResolverGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lsPersistentKeys);
            this.Controls.Add(this.cmdResolve);
            this.Controls.Add(this.label1);
            this.Name = "KeyResolverForm";
            this.Text = "Key Resolver";
            this.Load += new System.EventHandler(this.KeyResolverForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.keyResolverGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdResolve;
        private System.Windows.Forms.ListBox lsPersistentKeys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView keyResolverGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnType;
    }
}