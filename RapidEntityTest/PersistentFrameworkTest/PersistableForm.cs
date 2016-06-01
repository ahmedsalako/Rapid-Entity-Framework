using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PersistentManager;
using PersistentManager.Mapping;

namespace PersistentFrameworkTest
{
    [Table("Product")]
    public partial class PersistableForm : Form
    {
        public PersistableForm()
        {
            InitializeComponent();
        }

        [Key("Id", AutoKey = true)]
        public virtual long Id { get; set; }

        [Field("ItemName", false, true)]
        public virtual string ItemName { get; set; }

        [Field("Price", false, true)]
        public virtual double Price { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            using (EntityManager manager = new EntityManager())
            {
                ItemName = this.txtProductName.Text;
                Price = double.Parse(this.txtPrice.Text);

                manager.OpenDatabaseSession();
                manager.CreateNewEntity(this);
            }
        }
    }
}
