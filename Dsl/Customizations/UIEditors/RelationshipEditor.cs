using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using consist.RapidEntity.GUIExtension;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace consist.RapidEntity.Customizations.Relationship
{
    class RelationshipEditor : UITypeEditor
    {
        private IWindowsFormsEditorService formsEditorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.Modal;
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
                EntityRelationships modal = new EntityRelationships(baseRelationship);
                modal.ShowDialog();

                return modal.VALUE;
            }

            return base.EditValue(context, provider, value);
        }
    }
}
