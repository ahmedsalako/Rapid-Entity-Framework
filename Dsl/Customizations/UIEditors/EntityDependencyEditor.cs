using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace consist.RapidEntity
{
    public class EntityDependencyEditor : UITypeEditor
    {
        private IWindowsFormsEditorService formsEditorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            return base.GetEditStyle(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context == null) || (provider == null) || (context.Instance == null))
            {
                return base.EditValue(context, provider, value);
            }


            RelationshipConnector connector = (context.Instance as RelationshipConnector);
            
            if (connector != null)
            {
                BaseRelationship baseRelationship = (connector.ModelElement as BaseRelationship);

                if (baseRelationship != null)
                {
                    ModelRoot root = baseRelationship.Source.ModelRoot;
                    //EntitiesGraph graph = modelClass.Graph;

                    if (root != null)
                    {
                        ListBox listBox = new ListBox();
                        listBox.Sorted = true;
                        listBox.Click += new EventHandler(listBox_Click);
                        listBox.BorderStyle = BorderStyle.None;

                        foreach (ModelType childType in root.Types)
                        {
                           listBox.Items.Add(childType.Name);
                        }

                        listBox.SelectedItem = value;

                        this.formsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                        this.formsEditorService.DropDownControl(listBox);

                        return listBox.SelectedItem;
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }

        private void listBox_Click(object sender, EventArgs e)
        {
            if (this.formsEditorService != null)
            {
                this.formsEditorService.CloseDropDown();
            }
        }
    }
}
