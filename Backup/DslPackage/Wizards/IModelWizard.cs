using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using consist.RapidEntity.Customizations.Descriptors;
using consist.RapidEntity.DslPackage.Wizards.ImportWiz;
using System.Windows.Forms;

namespace consist.RapidEntity.DslPackage.Wizards
{
    public interface IModelWizard
    {
        List<TableDescriptor> Tables { get; set; }
        ClassDiagram ClassDiagram1 { get; set; }
        bool IsValid { get; }
        void LoadData( );
        void Execute( );
    }
}
