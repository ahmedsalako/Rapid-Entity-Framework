﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true )]
    public abstract class InterceptorAttribute : Attribute
    {

    }
}
