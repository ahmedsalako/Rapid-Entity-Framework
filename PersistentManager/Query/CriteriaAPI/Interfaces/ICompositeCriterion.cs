using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager
{
    internal interface ICompositeCriterion
    {
        Guid CompositionId { get; set; }
        IList<Criteria> Criterions { get; set; }
        CompositeType CompositeType { get; set; }
    }
}
