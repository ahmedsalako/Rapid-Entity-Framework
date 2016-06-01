using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Shell;
using consist.RapidEntity.Customizations.IDEHelpers;

namespace consist.RapidEntity
{
    [RuleOn(typeof(ModelAttribute), FireTime = TimeToFire.TopLevelCommit)]
    public class ModelPropertyChangeRule : ChangeRule
    {        
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
         {
            base.ElementPropertyChanged(e);

            ModelAttribute modelAttribute = e.ModelElement as ModelAttribute;

            if (e.ModelElement.StoreTransactionIsSerializing())
                return;

            if (CustomErrorProvider.CheckUpdatedPropertyExist(modelAttribute))
                return;

            using (Transaction transaction = modelAttribute.Store.TransactionManager.BeginTransaction("Attribute Changed"))
            {
                if (modelAttribute is PersistentKey || modelAttribute is Field)
                {                    
                    if ( modelAttribute.ColumnName.ContainsAny(typeof(Field).Name,typeof(PersistentKey).Name) )
                    {
                        modelAttribute.ColumnName = modelAttribute.Name;
                    }
                }

                transaction.Commit();
            }
        }
    }

    [RuleOn(typeof(ModelAttribute), FireTime=TimeToFire.TopLevelCommit)]
    public class ModelPropertyAddRule : AddRule
    {
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            base.ElementAdded(e);

            if (e.ModelElement.StoreTransactionIsSerializing())
                return;

            ModelAttribute modelAttribute = e.ModelElement as ModelAttribute;

            if (e.ModelElement.StoreTransactionIsSerializing())
                return;

            if (modelAttribute.GetModelClass().IsNull())
                return;

            if (CustomErrorProvider.CheckNewPropertyExist(modelAttribute))
            {
                if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsNull())
                {
                    e.ModelElement.Store.TransactionManager.CurrentTransaction.Rollback();
                }

                return;
            }

            using (Transaction transaction = modelAttribute.Store.TransactionManager.BeginTransaction("Attribute Changed"))
            {
                if (modelAttribute is PersistentKey || modelAttribute is Field)
                {
                    modelAttribute.Type = modelAttribute.Type.IsNullOrEmpty( ) ? typeof( string ).FullName : modelAttribute.Type ;
                    modelAttribute.AllowNull = true;

                    if ( modelAttribute.ColumnName.Trim().IsNullOrEmpty())
                    {
                        modelAttribute.ColumnName = modelAttribute.Name;
                    }

                    if ( modelAttribute is PersistentKey )
                    {
                        modelAttribute.AllowNull = false;
                    }
                }

                transaction.Commit();
            }            
        }
    }
}
