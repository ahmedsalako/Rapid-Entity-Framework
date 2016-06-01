using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;

namespace PersistentManager
{
    public interface ICriteriaCloneable : ICloneable
    {
        Criteria CloneScalable( );
    }
}
