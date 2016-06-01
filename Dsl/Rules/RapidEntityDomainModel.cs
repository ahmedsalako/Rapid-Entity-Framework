using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using consist.RapidEntity;
using consist.RapidEntity.Rules;

namespace consist.RapidEntity
{
    public partial class RapidEntityDomainModel
    {
        protected override Type[] GetCustomDomainModelTypes()
        {
            return new[] {  typeof(EntityClassAddRule),
                            typeof(EntityClassEditedRule),
                            //typeof(RoleChangedRule),
                            typeof(RelationshipConnectorRemovingRule),
                            typeof(RelationshipAddedRule ),
                            typeof(EntityClassRemoveRule),
                            typeof(ModelPropertyChangeRule),
                            typeof(ModelPropertyAddRule)
                         };
        }
    }
}
