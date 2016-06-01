using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Modeling;

namespace consist.RapidEntity
{
    public enum RelationshipTypeEnum : int
    {
        NOTSET,
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany,
    }    
}
