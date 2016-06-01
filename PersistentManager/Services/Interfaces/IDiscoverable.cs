using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentManager
{
    public interface IDiscoverable<T>
    {
        T Create( );
    }
}
