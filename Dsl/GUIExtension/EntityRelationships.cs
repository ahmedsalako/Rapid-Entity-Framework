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
    public partial class EntityRelationships : BaseGUIExtension
    {
        BaseRelationship relationship;
        ModelClass source;
        ModelClass target;

        public EntityRelationships()
        {
            InitializeComponent();
        }

        public EntityRelationships(BaseRelationship relationship):this()
        {
            this.relationship = relationship;

            source = relationship.Source;
            target = relationship.Target;

            BindList(source);
            BindList(target);
        }

        public string VALUE
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                int count = 0;

                foreach (object value in lsTarget.Items)
                {
                    if (0 == count)
                        builder.AppendFormat("{0}", value.ToString());
                    else
                        builder.AppendFormat(",{0}", value.ToString());

                }
                return builder.ToString(); 
            }
        }

        private void BindList(ModelClass model)
        {
            foreach (ModelAttribute attribute in model.Fields)
            {
                lsSource.Items.Add(attribute.Name);
            }

            foreach (PersistentKey key in model.PersistentKeys)
            {
                lsSource.Items.Add(key.Name);
            }
        }
    }
}
