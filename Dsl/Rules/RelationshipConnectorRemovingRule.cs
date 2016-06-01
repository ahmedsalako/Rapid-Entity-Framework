using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using consist.RapidEntity;
using consist.RapidEntity.GUIExtension;
using System.Windows.Forms;
using consist.RapidEntity.Customizations.IDEHelpers;

namespace consist.RapidEntity.Rules
{
    [RuleOn(typeof(BaseRelationship), FireTime = TimeToFire.TopLevelCommit)]
    public class RelationshipConnectorRemovingRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            base.ElementDeleting(e);

            if (e.ModelElement is BaseRelationship)
            {
                BaseRelationship baseRelationship = e.ModelElement as BaseRelationship;
                Store store = e.ModelElement.Store;

                using (Transaction deleteTransaction = store.TransactionManager.BeginTransaction("Remove Relationship"))
                {
                    ModelClass modelClass = baseRelationship.Source;
                    ModelClass relatedClass = store.GetModelClassByName(baseRelationship.Target.Name);

                    if (!relatedClass.IsNull())
                    {
                        using (Transaction deleteRelated = relatedClass.Store.TransactionManager.BeginTransaction("Removing Related"))
                        {
                            deleteRelated.Commit();
                        }
                    }

                    deleteTransaction.Commit();
                }
            }
        }
    }

    [RuleOn(typeof(BaseRelationship), FireTime = TimeToFire.TopLevelCommit)]
    internal class RelationshipAddedRule : AddRule
    {
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            if (e.ModelElement.StoreTransactionIsSerializing())
                return;

            if(e.ModelElement is BaseRelationship)
            {
                BaseRelationship relationship = (BaseRelationship) e.ModelElement;                

                ModelClass owner = relationship.Source;
                ModelClass target = relationship.Target;

                if ( relationship.RelationshipAlreadyExistIn( owner , target ) )
                {
                    return;
                }

                relationship.OwnerEntity = owner.Name;
                relationship.ReferenceEntity = target.Name;

                if (owner.Name == target.Name && !(relationship is OneToOne))
                    return;

                EnsureRelationship( owner , target , relationship );
            }
        }

        public void EnsureRelationship( ModelClass keyOwner , ModelClass keyReferencer , BaseRelationship relationship )
        {
            string foreignKey = string.Empty;
            string primaryKey = string.Empty;
            string type = string.Empty;

            if ( keyOwner.HasPersistentKey( ) )
            {
                foreignKey = keyReferencer.SetNewColumnName( keyOwner.PersistentKeys[0].ColumnName );
                type = keyOwner.PersistentKeys[0].Type;
            }
            else
            {
                CustomErrorProvider.AddErrorToErrorList( keyOwner , keyOwner.Store , string.Format( " {0} requires a primary key before relationship can be bound!" ) );
                return;
            }

            relationship.OwnerEntity = keyOwner.Name;
            relationship.ReferencedKey = keyOwner.PersistentKeys[0].ColumnName;
            relationship.ReferenceColumn = foreignKey;
            relationship.ReferenceEntity = keyReferencer.Name;
            relationship.Type = type;
        }
    }
}
