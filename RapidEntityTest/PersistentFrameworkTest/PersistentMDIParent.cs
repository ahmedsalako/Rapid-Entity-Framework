using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PersistentFrameworkTest
{
    public partial class PersistentMDIParent : Form
    {
        public PersistentMDIParent()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void productToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateProduct productForm = new CreateProduct();            
            productForm.MdiParent = this;
            productForm.Show();
        }

        private void customerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateCustomer customerForm = new CreateCustomer();
            customerForm.MdiParent = this;
            customerForm.Show();
        }

        private void PersistentMDIParent_Load(object sender, EventArgs e)
        {

        }
    }
}
