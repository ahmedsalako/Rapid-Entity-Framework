namespace consist.RapidEntity.DslPackage.Wizards.ImportWiz
{
    partial class ImportForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            this.menuPanePanel = new System.Windows.Forms.Panel( );
            this.panel1 = new System.Windows.Forms.Panel( );
            this.label3 = new System.Windows.Forms.Label( );
            this.label2 = new System.Windows.Forms.Label( );
            this.label1 = new System.Windows.Forms.Label( );
            this.selectTablePanel = new System.Windows.Forms.Panel( );
            this.mainContextPanel = new System.Windows.Forms.Panel( );
            this.cmdPrevious = new System.Windows.Forms.Button( );
            this.cmdNext = new System.Windows.Forms.Button( );
            this.cmdFinish = new System.Windows.Forms.Button( );
            this.menuPanePanel.SuspendLayout( );
            this.SuspendLayout( );
            // 
            // menuPanePanel
            // 
            this.menuPanePanel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.menuPanePanel.Controls.Add( this.panel1 );
            this.menuPanePanel.Controls.Add( this.label3 );
            this.menuPanePanel.Controls.Add( this.label2 );
            this.menuPanePanel.Controls.Add( this.label1 );
            this.menuPanePanel.Controls.Add( this.selectTablePanel );
            this.menuPanePanel.Location = new System.Drawing.Point( 0 , 0 );
            this.menuPanePanel.Name = "menuPanePanel";
            this.menuPanePanel.Size = new System.Drawing.Size( 164 , 284 );
            this.menuPanePanel.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point( 166 , 26 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 427 , 228 );
            this.panel1.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font( "Microsoft Sans Serif" , 8.25F , System.Drawing.FontStyle.Bold , System.Drawing.GraphicsUnit.Point , ( ( byte ) ( 0 ) ) );
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point( 12 , 51 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 125 , 13 );
            this.label3.TabIndex = 2;
            this.label3.Text = "3. Generate Artifacts";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font( "Microsoft Sans Serif" , 8.25F , System.Drawing.FontStyle.Bold , System.Drawing.GraphicsUnit.Point , ( ( byte ) ( 0 ) ) );
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point( 11 , 26 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 100 , 13 );
            this.label2.TabIndex = 1;
            this.label2.Text = "2. Select Tables";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font( "Microsoft Sans Serif" , 8.25F , System.Drawing.FontStyle.Bold , System.Drawing.GraphicsUnit.Point , ( ( byte ) ( 0 ) ) );
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point( 12 , 4 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 123 , 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "1. Connection String";
            // 
            // selectTablePanel
            // 
            this.selectTablePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectTablePanel.Location = new System.Drawing.Point( 166 , 151 );
            this.selectTablePanel.Name = "selectTablePanel";
            this.selectTablePanel.Size = new System.Drawing.Size( 427 , 228 );
            this.selectTablePanel.TabIndex = 8;
            // 
            // mainContextPanel
            // 
            this.mainContextPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainContextPanel.Location = new System.Drawing.Point( 170 , 4 );
            this.mainContextPanel.Name = "mainContextPanel";
            this.mainContextPanel.Size = new System.Drawing.Size( 429 , 246 );
            this.mainContextPanel.TabIndex = 7;
            // 
            // cmdPrevious
            // 
            this.cmdPrevious.Location = new System.Drawing.Point( 170 , 256 );
            this.cmdPrevious.Name = "cmdPrevious";
            this.cmdPrevious.Size = new System.Drawing.Size( 75 , 23 );
            this.cmdPrevious.TabIndex = 8;
            this.cmdPrevious.Text = "Previous";
            this.cmdPrevious.UseVisualStyleBackColor = true;
            this.cmdPrevious.Click += new System.EventHandler( this.cmdPrevious_Click );
            // 
            // cmdNext
            // 
            this.cmdNext.Location = new System.Drawing.Point( 274 , 256 );
            this.cmdNext.Name = "cmdNext";
            this.cmdNext.Size = new System.Drawing.Size( 75 , 23 );
            this.cmdNext.TabIndex = 9;
            this.cmdNext.Text = "Next";
            this.cmdNext.UseVisualStyleBackColor = true;
            this.cmdNext.Click += new System.EventHandler( this.cmdNext_Click );
            // 
            // cmdFinish
            // 
            this.cmdFinish.Location = new System.Drawing.Point( 376 , 256 );
            this.cmdFinish.Name = "cmdFinish";
            this.cmdFinish.Size = new System.Drawing.Size( 75 , 23 );
            this.cmdFinish.TabIndex = 10;
            this.cmdFinish.Text = "Finish";
            this.cmdFinish.UseVisualStyleBackColor = true;
            this.cmdFinish.Click += new System.EventHandler( this.cmdFinish_Click );
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F , 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size( 604 , 285 );
            this.Controls.Add( this.cmdFinish );
            this.Controls.Add( this.cmdNext );
            this.Controls.Add( this.cmdPrevious );
            this.Controls.Add( this.mainContextPanel );
            this.Controls.Add( this.menuPanePanel );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from Database";
            this.Load += new System.EventHandler( this.ImportForm_Load );
            this.menuPanePanel.ResumeLayout( false );
            this.menuPanePanel.PerformLayout( );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Panel menuPanePanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel selectTablePanel;
        private System.Windows.Forms.Panel mainContextPanel;
        private System.Windows.Forms.Button cmdPrevious;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Button cmdFinish;
    }
}