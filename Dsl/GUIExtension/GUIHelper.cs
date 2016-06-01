using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace consist.RapidEntity.GUIExtension
{
    public static class GUIHelper
    {
        internal static void BindModelAttributes(ModelClass model, ListControl lsSource)
        {
            IList<string> items = new List<string>();
            items.Add(string.Empty);

            foreach (ModelAttribute attribute in model.Fields)
            {
                items.Add(attribute.Name);
            }

            foreach (PersistentKey key in model.PersistentKeys)
            {
                items.Add(key.Name);
            }

            lsSource.DataSource = items;
        }
    }
}
