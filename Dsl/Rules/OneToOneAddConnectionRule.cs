using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using System.Windows.Forms;

namespace consist.RapidEntity
{
    [RuleOn( typeof( ModelClass ) , FireTime = TimeToFire.TopLevelCommit )]
    internal sealed class EntityClassEditedRule : ChangeRule
    {
        public override void ElementPropertyChanged( ElementPropertyChangedEventArgs e )
        {
            base.ElementPropertyChanged( e );

            if ( e.ModelElement.StoreTransactionIsSerializing( ) )
                return;

            ModelClass model = e.ModelElement as ModelClass;

            using ( Transaction transaction = model.Store.TransactionManager.BeginTransaction( "Change Model" ) )
            {
                if ( e.NewValue.ToString( ) == model.Name )
                {
                    if ( model.TableName.IsNullOrEmpty() || model.TableName.ContainsAny( typeof( ModelClass ).Name ) )
                    {
                        model.TableName = model.Name;
                    }

                    foreach ( BaseRelationship relationshipConnector in model.Store.GetAllConnectors( ) )
                    {
                        if ( relationshipConnector.ReferenceEntity == e.OldValue.ToString( ) )
                        {
                            relationshipConnector.ReferenceEntity = e.NewValue.ToString( );
                        }

                        if ( relationshipConnector.OwnerEntity == e.OldValue.ToString( ) )
                        {
                            relationshipConnector.OwnerEntity = e.NewValue.ToString( );
                        }
                    }
                }
                transaction.Commit( );
            }
        }
    }

    [RuleOn( typeof( ModelClass ) , FireTime = TimeToFire.TopLevelCommit )]
    internal sealed class EntityClassRemoveRule : DeletingRule
    {
        public override void ElementDeleting( ElementDeletingEventArgs e )
        {
            base.ElementDeleting( e );
            ModelClass model = e.ModelElement as ModelClass;
        }
    }

    //[RuleOn(typeof(ModelClassHasRelationships), FireTime = TimeToFire.TopLevelCommit)]
    //internal sealed class RoleChangedRule : RolePlayerChangeRule
    //{
    //    public override void RolePlayerChanged(RolePlayerChangedEventArgs e)
    //    {
    //        base.RolePlayerChanged(e);

    //    }
    //}    

    [RuleOn( typeof( ModelClass ) , FireTime = TimeToFire.TopLevelCommit )]
    internal sealed class EntityClassAddRule : AddRule
    {
        public override void ElementAdded( ElementAddedEventArgs e )
        {
            base.ElementAdded( e );

            if ( e.ModelElement.StoreTransactionIsSerializing( ) )
                return;

            ModelClass model = e.ModelElement as ModelClass;

            if ( model.PersistentKeys.Count > 0 )
                return;
        }
    }
}
