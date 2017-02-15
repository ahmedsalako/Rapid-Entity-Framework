namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    partial class SelectConnectionPanel
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
            this.cmdSelectConnection = new System.Windows.Forms.Button();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.txtProviderGuid = new System.Windows.Forms.TextBox();
            this.lblProviderGuid = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbExistingConnections = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmdSelectConnection
            // 
            this.cmdSelectConnection.Location = new System.Drawing.Point(349, 36);
            this.cmdSelectConnection.Name = "cmdSelectConnection";
            this.cmdSelectConnection.Size = new System.Drawing.Size(33, 22);
            this.cmdSelectConnection.TabIndex = 0;
            this.cmdSelectConnection.Text = "...";
            this.cmdSelectConnection.UseVisualStyleBackColor = true;
            this.cmdSelectConnection.Click += new System.EventHandler(this.cmdSelectConnection_Click);
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtConnectionString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConnectionString.Enabled = false;
            this.txtConnectionString.Location = new System.Drawing.Point(24, 89);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(358, 63);
            this.txtConnectionString.TabIndex = 2;
            // 
            // txtProviderGuid
            // 
            this.txtProviderGuid.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtProviderGuid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProviderGuid.Enabled = false;
            this.txtProviderGuid.Location = new System.Drawing.Point(24, 186);
            this.txtProviderGuid.Multiline = true;
            this.txtProviderGuid.Name = "txtProviderGuid";
            this.txtProviderGuid.Size = new System.Drawing.Size(358, 22);
            this.txtProviderGuid.TabIndex = 3;
            // 
            // lblProviderGuid
            // 
            this.lblProviderGuid.AutoSize = true;
            this.lblProviderGuid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProviderGuid.Location = new System.Drawing.Point(24, 167);
            this.lblProviderGuid.Name = "lblProviderGuid";
            this.lblProviderGuid.Size = new System.Drawing.Size(84, 13);
            this.lblProviderGuid.TabIndex = 4;
            this.lblProviderGuid.Text = "Provider Guid";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Connection String";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(402, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select an existing Connection or click the button to create a new one";
            // 
            // cmbExistingConnections
            // 
            this.cmbExistingConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExistingConnections.Items.AddRange(new object[] {
            "Select an existing connection"});
            this.cmbExistingConnections.Location = new System.Drawing.Point(24, 36);
            this.cmbExistingConnections.Name = "cmbExistingConnections";
            this.cmbExistingConnections.Size = new System.Drawing.Size(319, 21);
            this.cmbExistingConnections.TabIndex = 1;
            this.cmbExistingConnections.SelectedIndexChanged += new System.EventHandler(this.cmbExistingConnections_SelectedIndexChanged);
            // 
            // SelectConnectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblProviderGuid);
            this.Controls.Add(this.txtProviderGuid);
            this.Controls.Add(this.txtConnectionString);
            this.Controls.Add(this.cmbExistingConnections);
            this.Controls.Add(this.cmdSelectConnection);
            this.Name = "SelectConnectionPanel";
            this.Size = new System.Drawing.Size(418, 227);
            this.Load += new System.EventHandler(this.SelectConnectionPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdSelectConnection;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.TextBox txtProviderGuid;
        private System.Windows.Forms.Label lblProviderGuid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbExistingConnections;

    }
}
