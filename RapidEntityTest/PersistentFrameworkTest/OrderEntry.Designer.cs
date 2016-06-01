namespace PersistentFrameworkTest
{
    partial class OrderEntry
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
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.productList = new System.Windows.Forms.ListBox();
            this.selectedProduct = new System.Windows.Forms.ListBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Name :";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(184, 13);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(263, 26);
            this.txtCustomerName.TabIndex = 1;
            // 
            // productList
            // 
            this.productList.FormattingEnabled = true;
            this.productList.ItemHeight = 20;
            this.productList.Location = new System.Drawing.Point(12, 74);
            this.productList.Name = "productList";
            this.productList.Size = new System.Drawing.Size(199, 124);
            this.productList.TabIndex = 2;
            this.productList.SelectedIndexChanged += new System.EventHandler(this.productList_SelectedIndexChanged);
            // 
            // selectedProduct
            // 
            this.selectedProduct.FormattingEnabled = true;
            this.selectedProduct.ItemHeight = 20;
            this.selectedProduct.Location = new System.Drawing.Point(265, 74);
            this.selectedProduct.Name = "selectedProduct";
            this.selectedProduct.Size = new System.Drawing.Size(215, 124);
            this.selectedProduct.TabIndex = 3;
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(297, 204);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(98, 26);
            this.cmdSave.TabIndex = 4;
            this.cmdSave.Text = "Save Order";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdRemove
            // 
            this.cmdRemove.Location = new System.Drawing.Point(401, 204);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(79, 26);
            this.cmdRemove.TabIndex = 6;
            this.cmdRemove.Text = "Remove";
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // OrderEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 253);
            this.Controls.Add(this.cmdRemove);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.selectedProduct);
            this.Controls.Add(this.productList);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "OrderEntry";
            this.Text = "OrderEntry";
            this.Load += new System.EventHandler(this.OrderEntry_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.ListBox productList;
        private System.Windows.Forms.ListBox selectedProduct;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdRemove;
    }
}