namespace consist.RapidEntity.GUIExtension
{
    partial class PersistentPropertSheet
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
            this.allowNullChk = new System.Windows.Forms.CheckBox();
            this.txtColumnName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.isAutoKeyChk = new System.Windows.Forms.CheckBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtProperty = new System.Windows.Forms.TextBox();
            this.typeCombo = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Property";
            // 
            // allowNullChk
            // 
            this.allowNullChk.AutoSize = true;
            this.allowNullChk.Location = new System.Drawing.Point(140, 74);
            this.allowNullChk.Name = "allowNullChk";
            this.allowNullChk.Size = new System.Drawing.Size(91, 22);
            this.allowNullChk.TabIndex = 2;
            this.allowNullChk.Text = "Allow Null";
            this.allowNullChk.UseVisualStyleBackColor = true;
            // 
            // txtColumnName
            // 
            this.txtColumnName.Location = new System.Drawing.Point(140, 102);
            this.txtColumnName.Name = "txtColumnName";
            this.txtColumnName.Size = new System.Drawing.Size(210, 24);
            this.txtColumnName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Column Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Property Type";
            // 
            // isAutoKeyChk
            // 
            this.isAutoKeyChk.AutoSize = true;
            this.isAutoKeyChk.Location = new System.Drawing.Point(140, 216);
            this.isAutoKeyChk.Name = "isAutoKeyChk";
            this.isAutoKeyChk.Size = new System.Drawing.Size(101, 22);
            this.isAutoKeyChk.TabIndex = 7;
            this.isAutoKeyChk.Text = "Is Auto Key";
            this.isAutoKeyChk.UseVisualStyleBackColor = true;
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(275, 215);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 38);
            this.cmdSave.TabIndex = 8;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtProperty
            // 
            this.txtProperty.Location = new System.Drawing.Point(140, 26);
            this.txtProperty.Name = "txtProperty";
            this.txtProperty.ReadOnly = true;
            this.txtProperty.Size = new System.Drawing.Size(210, 24);
            this.txtProperty.TabIndex = 9;
            // 
            // typeCombo
            // 
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.ItemHeight = 18;
            this.typeCombo.Location = new System.Drawing.Point(140, 162);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(210, 40);
            this.typeCombo.TabIndex = 10;
            // 
            // PersistentPropertSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 259);
            this.Controls.Add(this.typeCombo);
            this.Controls.Add(this.txtProperty);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.isAutoKeyChk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtColumnName);
            this.Controls.Add(this.allowNullChk);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PersistentPropertSheet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rapid Entity Property";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox allowNullChk;
        private System.Windows.Forms.TextBox txtColumnName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox isAutoKeyChk;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.TextBox txtProperty;
        private System.Windows.Forms.ListBox typeCombo;
    }
}