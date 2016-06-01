using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using consist.RapidEntity.Util;
using Microsoft.VisualStudio.Modeling;
using System.Collections;

namespace consist.RapidEntity.GUIExtension
{
    public partial class PersistentPropertSheet : BaseGUIExtension
    {
        ModelClass modelClass;
        ModelAttribute attribute;

        public PersistentPropertSheet()
        {
            InitializeComponent();
        }

        public PersistentPropertSheet(ModelClass modelClass)
            : this()
        {
            this.modelClass = modelClass;

            IList<string> types = new List<string>(GlobalUtility.LoadPrimitiveTypes());
            typeCombo.DataSource = types;
        }

        public PersistentPropertSheet( ModelAttribute attribute )
            : this( )
        {
            this.attribute = attribute;

            if ( attribute is PersistentKey )
                modelClass = ( ( PersistentKey ) attribute ).ModelClass;
            else if ( attribute is Field )
                modelClass = ( ( Field ) attribute ).ModelClass;
            //else if ( attribute is Relationship )
                //modelClass = ( ( Relationship ) attribute ).ModelClass;

            IList<string> types = new List<string>( GlobalUtility.LoadPrimitiveTypes( ) );
            typeCombo.DataSource = types;
            LoadAttributeDetails( attribute );
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            using (Transaction transaction =  attribute.Store.TransactionManager.BeginTransaction("Update Property"))
            {
                string selectedItem = attribute.Name;

                    if (null != selectedItem)
                    {
                        string name = selectedItem.ToString();
                        ModelAttribute attr = GetModelAttribute(name, modelClass);

                        attr.ColumnName = txtColumnName.Text.Trim();
                        attr.AllowNull = allowNullChk.Checked;
                        attr.Type = typeCombo.Text;                   

                        if(attr is PersistentKey)
                        {
                            PersistentKey key = ((PersistentKey)attr);
                            key.IsAutoKey = isAutoKeyChk.Checked;
                        }
                    }

                    transaction.Commit();
            }

            this.Close();
        }

        private void LoadAttributeDetails(ModelAttribute attribute)
        {
            string selectedItem = attribute.Name;
            txtProperty.Text = selectedItem;

            if (null != selectedItem || selectedItem != string.Empty)
            {                
                string name = selectedItem.ToString();
                ModelAttribute attr = GetModelAttribute(name, modelClass);

                if (attr.IsNull())
                    return;

                txtColumnName.Text = attr.ColumnName;
                allowNullChk.Checked = attr.AllowNull;
                typeCombo.Text = attr.Type;

                if (attr is Field)
                {
                    isAutoKeyChk.Enabled = false;
                }
                else if(attr is PersistentKey)
                {
                    isAutoKeyChk.Enabled = true;
                    isAutoKeyChk.Checked = ((PersistentKey)attr).IsAutoKey;
                }                    
            }
        }

        private ModelAttribute GetModelAttribute(string name, ModelClass modelClass)
        {
            ModelAttribute attribute = modelClass.Fields.Where(f => f.Name == name).FirstOrDefault();

            if (!attribute.IsNull())
                return attribute;

            return modelClass.PersistentKeys.Where(f => f.Name == name).FirstOrDefault();
        }

        private void typeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
