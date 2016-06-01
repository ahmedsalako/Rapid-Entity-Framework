using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Data.Services;
using System.ComponentModel;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity.Customizations
{
    class ClassDiagramPropertyEditor : UITypeEditor
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
                ClassDiagram classDiagram = (context.Instance as ClassDiagram);
                if ((context == null) || (provider == null) || (context.Instance == null))
                {
                    return base.EditValue(context, provider, classDiagram.EncryptedConnection);
                }

                if ( classDiagram != null )
                {
                    string connection = classDiagram.ConnectionString;
                    IVsDataConnectionDialogFactory dataDialog = classDiagram.GetService( typeof( IVsDataConnectionDialogFactory ) ) as IVsDataConnectionDialogFactory;
                    IVsDataConnectionDialog connectionDialog = dataDialog.CreateConnectionDialog( );
                    connectionDialog.AddAllSources( );

                    if ( connectionDialog.ShowDialog( ) )
                    {
                        //connectionDialog.SelectedProvider                        
                        using ( Transaction updateDiagramTransaction = classDiagram.Store.TransactionManager.BeginTransaction( "Update Connection string" ) )
                        {
                            connection = connectionDialog.DisplayConnectionString;
                            classDiagram.ProviderGuid = connectionDialog.SelectedProvider;
                            classDiagram.ConnectionString = connectionDialog.DisplayConnectionString;
                            classDiagram.EncryptedConnection = connectionDialog.EncryptedConnectionString;
                            
                            updateDiagramTransaction.Commit( );
                        }
                    }

                    return connection;
                }

                return base.EditValue(context, provider, value);
            }
        }   
}
