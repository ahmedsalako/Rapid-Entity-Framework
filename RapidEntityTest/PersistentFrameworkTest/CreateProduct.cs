using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PersistentManager;
using Persistent.Entities;
using System.Collections;
using PersistentFrameworkTest.Utility;

namespace PersistentFrameworkTest
{
    public partial class CreateProduct : Form
    {
        bool isChecked = false;

        public CreateProduct()
        {
            InitializeComponent();

            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();
                BindList( manager );
            }
        }

        private void cmdCreate_Click( object sender , EventArgs e )
        {
            using ( EntityManager manager = new EntityManager( ConfigurationFactory.GetInstance( ) ) )
            {
                manager.OpenDatabaseSession( );

                if ( cmdSave.Text == "SAVE" )
                {
                    if ( rdbPerishable.Checked )
                        CreateNew( new PerishableProduct( ) , manager );
                    else
                        CreateNew( new NonPerishableProduct( ) , manager );
                }
                else
                {
                    Update( manager );
                }
            }

            Reset( );
        }

        private void Reset( )
        {
            FormControlHelper.ClearAllTextBoxes( this );
            FormControlHelper.ClearListBox( this );
            cmdSave.Text = "SAVE";
        }

        private void CreateNew(Product product, EntityManager manager)
        {
            product.ItemName = txtItemName.Text;
            product.Price = double.Parse(txtPrice.Text);          

            manager.CreateNewEntity<Product>(ref product);

            FormControlHelper.ClearAllTextBoxes(this);
            rdbPerishable.Checked = false;
            rdbNonPerishable.Checked = false;
        }

        private void Update( EntityManager manager )
        {
            Product product = ( Product )productList.SelectedItem;
            product.ItemName = txtItemName.Text;
            product.Price = double.Parse( txtPrice.Text );

            manager.SaveEntity( product , product.Id );

            productList.SelectedIndex = -1;
            FormControlHelper.ClearAllTextBoxes( this );
        }

        private void BindList(EntityManager manager)
        {
            productList.SelectedIndexChanged -= new EventHandler(productList_SelectedIndexChanged);
            IList products = manager.GetAll(typeof(Product));
            if (null != products && products.Count > 0)
            {                
                productList.ValueMember = "Id";
                productList.DisplayMember = "ItemName";
                productList.Items.Insert(0, "Select Product");
                productList.DataSource = products;

                productList.SelectedIndex = -1;
            }
            productList.SelectedIndexChanged += new EventHandler(productList_SelectedIndexChanged);
        }

        private void BindList(EntityManager manager, Type type)
        {
            FormControlHelper.ClearAllTextBoxes(this);

            productList.SelectedIndexChanged -= new EventHandler(productList_SelectedIndexChanged);
            IList products = manager.GetAll( type );

            if (null != products && products.Count > 0)
            {
                productList.DataSource = products;
                productList.ValueMember = "Id";
                productList.DisplayMember = "ItemName";
            }
            else
            {
                productList.DataSource = new ArrayList();                
            }

            productList.SelectedIndex = -1;
            productList.SelectedIndexChanged += new EventHandler(productList_SelectedIndexChanged);
        }

        private void productList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != productList.SelectedItem)
            {
                using (EntityManager manager = new EntityManager(ConfigurationFactory.GetInstance()))
                {
                    manager.OpenDatabaseSession();

                    Product product = (Product)productList.SelectedItem;
                    txtItemName.Text = product.ItemName;
                    txtPrice.Text = product.Price.ToString();
                    isChecked = (productList.SelectedIndex > 0);
                    cmdSave.Text = "UPDATE";
                }

                return;
            }

            cmdSave.Text = "SAVE";
            productList.SelectedIndex = -1;
        }

        private void productTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = productTypeList.SelectedItem as string;

            using(EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                if (item == "Perishable")
                {
                    BindList(manager, typeof(PerishableProduct));
                }
                else if (item == "NonPerishable")
                {
                    BindList(manager, typeof(NonPerishableProduct));
                }
            }

            productTypeList.SelectedIndex = -1;
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
