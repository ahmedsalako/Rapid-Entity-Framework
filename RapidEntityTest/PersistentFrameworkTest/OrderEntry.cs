using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Persistent.Entities;
using PersistentManager;
using System.Collections;
using PersistentFrameworkTest.Utility;

namespace PersistentFrameworkTest
{
    public partial class OrderEntry : Form
    {
        Customer customer;

        public OrderEntry(Customer customer) : this()
        {
            this.customer = customer;
        }

        public OrderEntry()
        {
            InitializeComponent();            
        }

        private void productList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != productList.SelectedItem)
            {
                Product product = productList.SelectedItem as Product;
                selectedProduct.Items.Add(product);                
            }
        }

        private void AddProduct( )
        {
            using ( EntityManager manager = new EntityManager( ) )
            {
                manager.OpenDatabaseSession( );
                CustomerOrders order = new CustomerOrders( );
                order.Customer = customer;
                order.Orderdate = DateTime.Now;
                order.Products = new ArrayList( );

                foreach ( Product product in selectedProduct.Items )
                {
                    order.Products.Add( product );
                }

                customer.CustomerOrders.Add( order );

                manager.SaveEntity( customer );
            }
        }

        private void OrderEntry_Load(object sender, EventArgs e)
        {
            txtCustomerName.Text = customer.FullName;
            FormControlHelper.BindProducts<OrderEntry>(productList, this, new EventHandler(productList_SelectedIndexChanged));
            selectedProduct.DisplayMember = "ItemName";
            selectedProduct.ValueMember = "Id";
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            if (null != productList.SelectedItem)
            {
                Product product = productList.SelectedItem as Product;                
                selectedProduct.Items.Remove(product);
                object collection = selectedProduct.Items;
                selectedProduct.DataSource = null;
                selectedProduct.DataSource = collection;
                selectedProduct.Refresh();
            }
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            AddProduct();
            this.Close();
        }
    }
}
