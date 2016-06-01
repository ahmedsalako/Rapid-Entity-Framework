namespace consist.RapidEntity.GUIExtension
{
    partial class EntityRelationships
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
            this.lsSource = new System.Windows.Forms.ListBox();
            this.lsTarget = new System.Windows.Forms.ListBox();
            this.cmdToTarget = new System.Windows.Forms.Button();
            this.cmdToSource = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lsSource
            // 
            this.lsSource.FormattingEnabled = true;
            this.lsSource.ItemHeight = 18;
            this.lsSource.Location = new System.Drawing.Point(56, 94);
            this.lsSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lsSource.Name = "lsSource";
            this.lsSource.Size = new System.Drawing.Size(236, 184);
            this.lsSource.TabIndex = 0;
            // 
            // lsTarget
            // 
            this.lsTarget.FormattingEnabled = true;
            this.lsTarget.ItemHeight = 18;
            this.lsTarget.Location = new System.Drawing.Point(495, 94);
            this.lsTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lsTarget.Name = "lsTarget";
            this.lsTarget.Size = new System.Drawing.Size(236, 184);
            this.lsTarget.TabIndex = 1;
            // 
            // cmdToTarget
            // 
            this.cmdToTarget.Location = new System.Drawing.Point(336, 134);
            this.cmdToTarget.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdToTarget.Name = "cmdToTarget";
            this.cmdToTarget.Size = new System.Drawing.Size(112, 32);
            this.cmdToTarget.TabIndex = 2;
            this.cmdToTarget.Text = ">>";
            this.cmdToTarget.UseVisualStyleBackColor = true;
            // 
            // cmdToSource
            // 
            this.cmdToSource.Location = new System.Drawing.Point(336, 195);
            this.cmdToSource.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdToSource.Name = "cmdToSource";
            this.cmdToSource.Size = new System.Drawing.Size(112, 32);
            this.cmdToSource.TabIndex = 3;
            this.cmdToSource.Text = "<<";
            this.cmdToSource.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(102, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "Source Properties";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(536, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "Selected Properties";
            // 
            // EntityRelationships
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 300);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdToSource);
            this.Controls.Add(this.cmdToTarget);
            this.Controls.Add(this.lsTarget);
            this.Controls.Add(this.lsSource);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EntityRelationships";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EntityRelationships";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lsSource;
        private System.Windows.Forms.ListBox lsTarget;
        private System.Windows.Forms.Button cmdToTarget;
        private System.Windows.Forms.Button cmdToSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}