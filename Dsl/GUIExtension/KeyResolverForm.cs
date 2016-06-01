using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace consist.RapidEntity.GUIExtension
{
    public partial class KeyResolverForm : BaseGUIExtension
    {
        private ModelClass modelClass;

        public string RelationshipColumn { get; set; }
        public string RelationshipType { get; set; }

        public KeyResolverForm()
        {
            InitializeComponent();
        }

        public KeyResolverForm(ModelClass modelClass) : this()
        {            
            this.modelClass = modelClass;
        }

        private void KeyResolverForm_Load(object sender, EventArgs e)
        {
            foreach (ModelAttribute attribute in modelClass.GetProperties())
            {
                lsPersistentKeys.Items.Add(attribute.ColumnName);
            }
        }

        protected bool ListContains(List<string> list, string value)
        {
            foreach (string item in list)
            {
                if (item.ToLower() == value.ToLower())
                    return true;
            }

            return false;
        }

        protected void GetDataFromGrid(bool hasDelemeter, List<string> list)
        {
            int count = 0;

            foreach (DataGridViewRow gridView in keyResolverGrid.Rows)
            {
                DataGridViewCell cell1 = gridView.Cells[0];
                DataGridViewCell cell2 = gridView.Cells[1];

                if (!cell1.Value.IsNull())
                {
                    string columnName = cell1.Value.ToString();
                    string comboBoxValue = cell2.Value.ToString();

                    if (count <= 0 && !hasDelemeter && !ListContains(list, columnName))
                    {
                        RelationshipColumn = columnName;
                        RelationshipType = comboBoxValue;
                    }
                    else if (!ListContains(list, columnName))
                    {
                        RelationshipColumn += string.Concat(columnName, ",");
                        RelationshipType += string.Concat(comboBoxValue, ",");
                    }

                    ++count;
                }
            }
        }

        private void cmdResolve_Click(object sender, EventArgs e)
        {
            bool hasDelemeter = false;

            if (lsPersistentKeys.SelectedItems.Count > 0)
            {
                int count = 0;

                foreach (string item in lsPersistentKeys.SelectedItems)
                {
                    if (count <= 0)
                    {
                        RelationshipColumn = item;
                        RelationshipType = GetRelationshipType(item);
                    }
                    else
                    {
                        hasDelemeter = true;
                        RelationshipColumn += string.Concat(item, ",");
                        RelationshipType += string.Concat(GetRelationshipType(item), ",");
                    }
                    ++count;
                }
                DialogResult = DialogResult.OK;
            }
            else //if (!RelationshipColumn.IsNullOrEmpty())
            {
                string[] array = RelationshipColumn.IsNull() ? (new List<string>()).ToArray() :
                    RelationshipColumn.Split(',');

                GetDataFromGrid(hasDelemeter, new List<string>(array));
                DialogResult = DialogResult.OK;
            }         
        }

        private string GetRelationshipType(string columnName)
        {
            ModelAttribute attribute = modelClass.GetProperties()
                                        .Where(p => p.ColumnName == columnName).FirstOrDefault();

            if (attribute.IsNull())
                return string.Empty;

            return attribute.Type;
        }

        public static KeyResolverForm GetInstance(ModelClass target)
        {
            KeyResolverForm form = new KeyResolverForm(target);
            DialogResult dialogResult = form.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                return form;
            }

            return null;
        }
    }
}
