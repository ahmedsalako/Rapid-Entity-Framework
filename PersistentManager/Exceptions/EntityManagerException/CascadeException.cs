using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Exceptions.EntityManagerException
{
    public class CascadeException : PersistentException
    {
        public CascadeException()
            : base()
        {
        }

        public CascadeException(string message)
            : base(message)
        {

        }

        public CascadeException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
