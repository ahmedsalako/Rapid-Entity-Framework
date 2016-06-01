using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using consist.RapidEntity.GUIExtension;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Data.Services;

namespace consist.RapidEntity.Customizations.EntityProperties
{
    class EntityPropertyEditor : UITypeEditor
    {
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


            ModelAttribute attribute = (context.Instance as ModelAttribute);

            if (attribute != null)
            {
                PersistentPropertSheet modal = new PersistentPropertSheet(attribute);
                modal.ShowDialog();

                return string.Empty;
            }

            return base.EditValue(context, provider, value);
        }
    }
}
