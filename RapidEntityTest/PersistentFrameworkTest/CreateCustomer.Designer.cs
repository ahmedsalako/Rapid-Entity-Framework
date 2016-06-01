namespace PersistentFrameworkTest
{
    partial class CreateCustomer
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
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdOrder = new System.Windows.Forms.Button();
            this.customersList = new System.Windows.Forms.ListBox();
            this.txtFName = new System.Windows.Forms.TextBox();
            this.txtLName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ordersList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPostCode = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(42, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "First Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(44, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Last Name";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdOrder);
            this.groupBox1.Controls.Add(this.customersList);
            this.groupBox1.Location = new System.Drawing.Point(376, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 159);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Existing Customers";
            // 
            // cmdOrder
            // 
            this.cmdOrder.Location = new System.Drawing.Point(6, 136);
            this.cmdOrder.Name = "cmdOrder";
            this.cmdOrder.Size = new System.Drawing.Size(58, 23);
            this.cmdOrder.TabIndex = 1;
            this.cmdOrder.Text = "Order";
            this.cmdOrder.UseVisualStyleBackColor = true;
            this.cmdOrder.Click += new System.EventHandler(this.cmdOrder_Click);
            // 
            // customersList
            // 
            this.customersList.FormattingEnabled = true;
            this.customersList.ItemHeight = 16;
            this.customersList.Location = new System.Drawing.Point(6, 32);
            this.customersList.Name = "customersList";
            this.customersList.Size = new System.Drawing.Size(335, 100);
            this.customersList.TabIndex = 0;
            this.customersList.SelectedIndexChanged += new System.EventHandler(this.customersList_SelectedIndexChanged);
            // 
            // txtFName
            // 
            this.txtFName.Location = new System.Drawing.Point(156, 11);
            this.txtFName.Name = "txtFName";
            this.txtFName.Size = new System.Drawing.Size(194, 22);
            this.txtFName.TabIndex = 3;
            // 
            // txtLName
            // 
            this.txtLName.Location = new System.Drawing.Point(156, 54);
            this.txtLName.Name = "txtLName";
            this.txtLName.Size = new System.Drawing.Size(194, 22);
            this.txtLName.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ordersList);
            this.groupBox2.Location = new System.Drawing.Point(376, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(356, 133);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Orders";
            // 
            // ordersList
            // 
            this.ordersList.FormattingEnabled = true;
            this.ordersList.ItemHeight = 16;
            this.ordersList.Location = new System.Drawing.Point(6, 21);
            this.ordersList.Name = "ordersList";
            this.ordersList.Size = new System.Drawing.Size(335, 100);
            this.ordersList.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(66, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Country";
            // 
            // txtCountry
            // 
            this.txtCountry.Location = new System.Drawing.Point(156, 87);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(194, 22);
            this.txtCountry.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(53, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Address";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(156, 127);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(194, 62);
            this.txtAddress.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(36, 217);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Post Code";
            // 
            // txtPostCode
            // 
            this.txtPostCode.Location = new System.Drawing.Point(156, 211);
            this.txtPostCode.Name = "txtPostCode";
            this.txtPostCode.Size = new System.Drawing.Size(194, 22);
            this.txtPostCode.TabIndex = 13;
            // 
            // cmdSave
            // 
            this.cmdSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSave.Location = new System.Drawing.Point(156, 260);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(92, 38);
            this.cmdSave.TabIndex = 14;
            this.cmdSave.Text = "SAVE";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdClear
            // 
            this.cmdClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdClear.Location = new System.Drawing.Point(258, 260);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(92, 38);
            this.cmdClear.TabIndex = 15;
            this.cmdClear.Text = "CLEAR";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // CreateCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 318);
            this.Controls.Add(this.cmdClear);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.txtPostCode);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCountry);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtLName);
            this.Controls.Add(this.txtFName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CreateCustomer";
            this.Text = "CreateCustomer";
            this.Load += new System.EventHandler(this.CreateCustomer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFName;
        private System.Windows.Forms.TextBox txtLName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox ordersList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCountry;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPostCode;
        private System.Windows.Forms.ListBox customersList;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.Button cmdOrder;

    }
}