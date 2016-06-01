using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;

namespace consist.RapidEntity.Customizations.UIEditors
{
    public class CheckBoxUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.None;
            }
            return base.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            ClassDiagram classDiagram = (e.Context.Instance as ClassDiagram);
            if ((e.Context == null) || (e.Context.Instance == null))
            {
                return;
            }
            ControlPaint.DrawCheckBox(e.Graphics, e.Bounds, classDiagram.NameSingularization ? ButtonState.Checked : ButtonState.Normal);
        }
    }

    public class ComboBoxUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.None;
            }
            return base.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            ClassDiagram classDiagram = (e.Context.Instance as ClassDiagram);
            if ((e.Context == null) || (e.Context.Instance == null))
            {
                return;
            }
            ControlPaint.DrawCheckBox(e.Graphics, e.Bounds, classDiagram.NameSingularization ? ButtonState.Checked : ButtonState.Normal);
        }
    }
}
