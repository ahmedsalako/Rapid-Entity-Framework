using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using consist.RapidEntity;

namespace consist.RapidEntity.Rules
{
    [RuleOn(typeof(Relationship), FireTime = TimeToFire.TopLevelCommit)]
    internal sealed class OnDeleteRelationshipRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {            
            base.ElementDeleting(e);

            if (e.ModelElement is Relationship)
            {
                Relationship relationship = e.ModelElement as Relationship;
                Store store = e.ModelElement.Store;
                ModelClass modelClass = relationship.ModelClass;

                if (!modelClass.IsNull())
                {
                    using (Transaction deleteTransaction = store.TransactionManager.BeginTransaction("Remove Relationship"))
                    {
                        modelClass.RemoveRelationshipBindings(relationship.Name, relationship.GetRelationship(), Bindings.Both);
                        modelClass.RemoveRelationshipBindings(relationship.Name, relationship.GetInverseRelationship(), Bindings.Both);

                        deleteTransaction.Commit();
                    }
                }
            }
        }
    }
}
