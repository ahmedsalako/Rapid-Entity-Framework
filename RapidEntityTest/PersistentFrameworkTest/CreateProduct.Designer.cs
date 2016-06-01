namespace PersistentFrameworkTest
{
    partial class CreateProduct
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
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbNonPerishable = new System.Windows.Forms.RadioButton();
            this.rdbPerishable = new System.Windows.Forms.RadioButton();
            this.productList = new System.Windows.Forms.ListBox();
            this.productTypeList = new System.Windows.Forms.ListBox();
            this.cmdClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item Name";
            // 
            // txtItemName
            // 
            this.txtItemName.Location = new System.Drawing.Point(145, 33);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(250, 23);
            this.txtItemName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(31, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Price";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(145, 88);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(100, 23);
            this.txtPrice.TabIndex = 3;
            // 
            // cmdSave
            // 
            this.cmdSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSave.Location = new System.Drawing.Point(133, 208);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(112, 39);
            this.cmdSave.TabIndex = 4;
            this.cmdSave.Text = "SAVE";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdCreate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbNonPerishable);
            this.groupBox1.Controls.Add(this.rdbPerishable);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(133, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 67);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Product Type";
            // 
            // rdbNonPerishable
            // 
            this.rdbNonPerishable.AutoSize = true;
            this.rdbNonPerishable.Location = new System.Drawing.Point(155, 33);
            this.rdbNonPerishable.Name = "rdbNonPerishable";
            this.rdbNonPerishable.Size = new System.Drawing.Size(130, 24);
            this.rdbNonPerishable.TabIndex = 1;
            this.rdbNonPerishable.TabStop = true;
            this.rdbNonPerishable.Text = "NonPerishable";
            this.rdbNonPerishable.UseVisualStyleBackColor = true;
            // 
            // rdbPerishable
            // 
            this.rdbPerishable.AutoSize = true;
            this.rdbPerishable.Location = new System.Drawing.Point(12, 33);
            this.rdbPerishable.Name = "rdbPerishable";
            this.rdbPerishable.Size = new System.Drawing.Size(101, 24);
            this.rdbPerishable.TabIndex = 0;
            this.rdbPerishable.TabStop = true;
            this.rdbPerishable.Text = "Perishable";
            this.rdbPerishable.UseVisualStyleBackColor = true;
            // 
            // productList
            // 
            this.productList.FormattingEnabled = true;
            this.productList.ItemHeight = 17;
            this.productList.Location = new System.Drawing.Point(439, 73);
            this.productList.Name = "productList";
            this.productList.Size = new System.Drawing.Size(184, 174);
            this.productList.TabIndex = 6;
            this.productList.SelectedIndexChanged += new System.EventHandler(this.productList_SelectedIndexChanged);
            // 
            // productTypeList
            // 
            this.productTypeList.AllowDrop = true;
            this.productTypeList.FormattingEnabled = true;
            this.productTypeList.ItemHeight = 17;
            this.productTypeList.Items.AddRange(new object[] {
            "Perishable",
            "NonPerishable"});
            this.productTypeList.Location = new System.Drawing.Point(439, 33);
            this.productTypeList.Name = "productTypeList";
            this.productTypeList.Size = new System.Drawing.Size(184, 38);
            this.productTypeList.TabIndex = 7;
            this.productTypeList.SelectedIndexChanged += new System.EventHandler(this.productTypeList_SelectedIndexChanged);
            // 
            // cmdClear
            // 
            this.cmdClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdClear.Location = new System.Drawing.Point(271, 208);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(112, 39);
            this.cmdClear.TabIndex = 8;
            this.cmdClear.Text = "CLEAR";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // CreateProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 266);
            this.Controls.Add(this.cmdClear);
            this.Controls.Add(this.productTypeList);
            this.Controls.Add(this.productList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CreateProduct";
            this.Text = "Create Product";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbNonPerishable;
        private System.Windows.Forms.RadioButton rdbPerishable;
        private System.Windows.Forms.ListBox productList;
        private System.Windows.Forms.ListBox productTypeList;
        private System.Windows.Forms.Button cmdClear;
    }
}