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
using System.Threading;
using PersistentManager.Query.Keywords;
using PersistentManager.Query;

namespace PersistentFrameworkTest
{
    public partial class CreateCustomer : Form, IWin32Window
    {
        public CreateCustomer()
        {
            InitializeComponent();
            BindCustomers();
        }

        private void CreateCustomer_Load(object sender, EventArgs e)
        {

        }

        protected void BindCustomers()
        {
            FormControlHelper.BindCustomers(customersList, this);
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (cmdSave.Text.Trim().ToUpper() == "SAVE")
                SaveCustomer();
            else
                UpdateCustomer();

            BindCustomers();
        }

        private void UpdateCustomer( )
        {
            using ( EntityManager manager = new EntityManager( ) )
            {
                manager.OpenDatabaseSession( );

                Customer customer = customersList.SelectedItem as Customer;
                customer.FirstName = txtFName.Text;
                customer.LastName = txtLName.Text;

                if ( null == customer.Address )
                {
                    Address address = new Address( )
                    {
                        Country = txtCountry.Text ,
                        Customer = customer ,
                        PostCode = txtPostCode.Text ,
                        FullAddress = txtAddress.Text
                    };

                    customer.Address = address;
                }
                else
                {
                    customer.Address.Country = txtCountry.Text;
                    customer.Address.FullAddress = txtAddress.Text;
                    customer.Address.PostCode = txtPostCode.Text;
                }

                manager.SaveEntity( customer );
            }
        }

        private void SaveCustomer()
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                Customer customer = new Customer();
                customer.FirstName = txtFName.Text;
                customer.LastName = txtLName.Text;

                Address address = new Address()
                {
                    Country = txtCountry.Text,
                    Customer = customer,
                    PostCode = txtPostCode.Text,
                    FullAddress = txtAddress.Text
                };

                customer.Address = address;
                manager.CreateNewEntity<Customer>(ref customer);
            }
        }

        public void customersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != customersList.SelectedItem)
            {
                using(EntityManager manager = new EntityManager())
                {
                       manager.OpenDatabaseSession();                    

                       FormControlHelper.ClearAllTextBoxes(this);

                       Customer customer = customersList.SelectedItem as Customer;
                       customer = (Customer) manager.LoadEntity(typeof(Customer), customer.Id);
                       txtFName.Text = customer.FirstName;
                       txtLName.Text = customer.LastName;

                       if (null != customer.Address)
                       {
                           txtCountry.Text = customer.Address.Country;
                           txtAddress.Text = customer.Address.FullAddress;
                           txtPostCode.Text = customer.Address.PostCode;
                       }

                       BindOrders(customer);

                       cmdSave.Text = "Update";
                       return;
                }
            }

            cmdSave.Text = "Save";
        }

        private void BindOrders(Customer customer)
        {
            if (customer.CustomerOrders.Count > 0)
            {
                IList orders = new ArrayList();

                foreach (CustomerOrders order in customer.CustomerOrders)
                {
                    orders.Add(order);
                    //int count = order.Products.Count;
                    //Product prd = order.Products[0] as Product;
                    //IList<OrderedProduct> corders = prd.CustomerOrders;
                }

                ordersList.DataSource = orders;
            }
            else
            {
                ordersList.DataSource = null;
            }

            ordersList.ValueMember = "Id";
            ordersList.DisplayMember = "OrderName";
            ordersList.SelectedIndex = -1;
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            FormControlHelper.ClearAllTextBoxes(this);
            FormControlHelper.ClearListBox(this);
            cmdSave.Text = "Save";
        }

        private void cmdOrder_Click(object sender, EventArgs e)
        {
            if (null != customersList.SelectedItem)
            {
                OrderEntry entry = new OrderEntry((Customer)customersList.SelectedItem);
                entry.ShowDialog(this);
            }
        }

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get { return this.Handle; }
        }

        #endregion
    }
}
