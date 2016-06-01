using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PersistentManager;
using System.Collections;
using Persistent.Entities;

namespace PersistentFrameworkTest.Utility
{
    public static class FormControlHelper
    {
        public static void ClearAllTextBoxes(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is TextBox)
                    ((TextBox)control).Text = string.Empty;
            }
        }

        public static void ClearListBox(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is ListBox)
                    ((ListBox)control).SelectedIndex = -1;
            }
        }

        public static void BindCustomers(ListBox customersList, CreateCustomer form)
        {
            IList<Customer> customers;

            using (EntityManager manager = new EntityManager(ConfigurationFactory.GetInstance()))
            {
                manager.OpenDatabaseSession();
                customers = (from customer in manager.AsQueryable<Customer>()
                           select customer).ToList();

                //customers = manager.GetAll(typeof(Customer));
            }

            customersList.SelectedIndexChanged -= new EventHandler( form.customersList_SelectedIndexChanged );
            customersList.DataSource = customers;
            customersList.ValueMember = "Id";
            customersList.DisplayMember = "FullName";
            customersList.SelectedIndex = -1;
            customersList.SelectedIndexChanged += new EventHandler( form.customersList_SelectedIndexChanged );
        }

        public static void BindProducts<T>(ListBox productList, T form, EventHandler handler) where T : Form
        {
            IList customers = new ArrayList();

            using (EntityManager manager = new EntityManager(ConfigurationFactory.GetInstance()))
            {
                manager.OpenDatabaseSession();
                customers = manager.GetAll(typeof(Product));
            }

            productList.SelectedIndexChanged -= handler;
            productList.DataSource = customers;
            productList.ValueMember = "Id";
            productList.DisplayMember = "ItemName";
            productList.SelectedIndex = -1;
            productList.SelectedIndexChanged += handler;
        }
    }
}
