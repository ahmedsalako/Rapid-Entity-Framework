using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using PersistentManager.Query;

namespace PersistentManager
{
    internal static class CriteriaExtension
    {
        internal static Criteria CloneScalable( this Criteria criteria )
        {
            return ( ( ICriteriaCloneable ) criteria ).CloneScalable( ) as Criteria;
        }

        internal static Criteria Clone( this Criteria criteria )
        {
            return ( ( ICriteriaCloneable ) criteria ).Clone( ) as Criteria;
        }

        internal static bool IsComposite( this Criteria criteria )
        {
            return ( criteria is CompositeCriteria );
        }

        internal static bool IsSelectCriteria( this Criteria criteria )
        {
            return ( criteria.QueryPart == QueryPart.SELECT );
        }

        internal static bool IsEmptyCriteria( this CompositeCriteria criteria )
        {
            return ( criteria.Criterions.Count <= 0 );
        }
    }
}
