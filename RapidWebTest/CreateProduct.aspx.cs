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
using PersistentManager;
using System.Collections.Generic;
using PersistentManager.Query;
using Persistent.Entities;

namespace RapidWebTest
{
    public partial class CreateProduct : System.Web.UI.Page
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
            using(EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                ddlProducts.DataSource = from product in manager.AsQueryable<Product>() select product;
                ddlProducts.DataTextField = "ItemName";
                ddlProducts.DataValueField = "Id";
                ddlProducts.DataBind();
            }
        }

        protected string GetValue()
        {
            return "Nigeria";
        }

        protected void cmdItem_Click(object sender, EventArgs e)
        {
            using (EntityManager manager = new EntityManager())
            {
                manager.OpenDatabaseSession();

                PerishableProduct product = new PerishableProduct();
                product.ItemName = txtItemName.Text;
                product.Price = double.Parse(txtPrice.Text.Trim());

                manager.CreateNewEntity<PerishableProduct>(ref product);
            }
        }
    }
}
