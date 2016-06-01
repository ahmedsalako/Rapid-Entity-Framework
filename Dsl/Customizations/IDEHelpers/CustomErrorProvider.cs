using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using EnvDTE80;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity.Customizations.IDEHelpers
{
    public static class CustomErrorProvider
    {
        static Guid providerIdentifier = new Guid("C4B3FC14-0D8E-4a61-9E2C-C57F5FE60BBB");

        [CLSCompliant(true)]
        public static ErrorListProvider GetErrorListProvider()
        {
            EnvDTE80.DTE2 activeIDE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;

            Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider = new Microsoft.VisualStudio
                 .Shell.ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)activeIDE);

            return new ErrorListProvider(serviceProvider);
        }

        public static void ClearErrorProvider()
        {
            ErrorListProvider errorProvider = GetErrorListProvider();
            int listCount = errorProvider.Tasks.Count;

            for (int i = 0; i < listCount; i++)
                errorProvider.Tasks.RemoveAt(i);            

            errorProvider.Tasks.Clear();
            errorProvider.Show();
        }

        public static ErrorTask CreateTask(string document, string errorDetails)
        {
            ErrorTask errorTask = new ErrorTask();
            errorTask.ErrorCategory = TaskErrorCategory.Error;
            errorTask.Document = document;
            errorTask.Line = 0;
            errorTask.Text = errorDetails;

            return errorTask;
        }

        public static void AddErrorToErrorList(ModelClass model, Store store, string errorDetails)
        {
              try
              {
                 store.TransactionManager.CurrentTransaction.Rollback();

                 ErrorTask errorTask = CreateTask(model.Name, errorDetails);

                 ErrorListProvider errorProvider = GetErrorListProvider();
                 errorProvider.ProviderGuid = providerIdentifier;
                 errorProvider.ProviderName = "Rapid Error List";
                 int listCount = errorProvider.Tasks.Count;

                 for (int i = 0; i < listCount; i++)
                     errorProvider.Tasks.RemoveAt(i);

                 errorProvider.Refresh();

                 errorProvider.Tasks.Add(errorTask);
                 errorProvider.ForceShowErrors();
                 errorProvider.Show();
            }
            catch(Exception x)
            {

            }
        }

        public static bool CheckNewPropertyExist(ModelAttribute field)
        {
            ModelClass model = field.GetModelClass();

            if (model.IsNull())
                return true;

            if (model.AddedPropertyExist(field))
            {
                //CustomErrorProvider.AddErrorToErrorList(model, field.Store,
                    //string.Format("Property {0} already exist!", field.Name));

                return true;
            }

            return false;
        }

        public static bool CheckUpdatedPropertyExist(ModelAttribute field)
        {
            ModelClass model = field.GetModelClass();

            if (model.UpdatedPropertyExist(field))
            {
                //CustomErrorProvider.AddErrorToErrorList(model, field.Store,
                  //  string.Format("Property {0} already exist!", field.Name));

                return true;
            }

            return false;
        }
    }
}
