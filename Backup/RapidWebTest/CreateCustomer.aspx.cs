using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using PersistentManager;
using Persistent.Entities;
using PersistentManager.Query.Keywords;

namespace RapidWebTest
{
    public partial class CreateCustomer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                var cus = manager.AsQueryable<Customer>().FirstOrDefault();
                var cus2 = cus.Address.Customer;

                ddlCustomer.DataSource = manager.AsQueryable<Customer>().Select( c => c ).ToList();

                ddlCustomer.DataTextField = "FullName";
                ddlCustomer.DataValueField = "Id";
                ddlCustomer.DataBind();
            }
        }

        protected void cmdItem_Click(object sender, EventArgs e)
        {
            using (EntityManager entitymanager = new EntityManager())
            {
                entitymanager.OpenDatabaseSession();                

                Customer customer = new Customer();
                customer.FirstName = txtFirstName.Text;
                customer.LastName = txtLastName.Text;

                Address address = new Address();
                address.Country = txtCountry.Text;
                address.Customer = customer;
                address.FullAddress = txtHouseNo.Text;
                address.PostCode = txtPostCode.Text;
                customer.Address = address;
             
                entitymanager.CreateNewEntity( customer );
            }

            BindData();
        }
    }
}
