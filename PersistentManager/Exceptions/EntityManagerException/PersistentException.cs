using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager
{
    public class PersistentException : Exception
    {
        internal PersistentException() : base() { }
        internal PersistentException(string message) : base(message) { }
        internal PersistentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
