using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Exceptions.EntityManagerException
{
    public class EntityLoadException : PersistentException
    {
        private const string MESSAGE = "Could not load entity";

        public EntityLoadException()
            : base(MESSAGE)
        {
        }

        public EntityLoadException(string message)
            :base(string.Format("{0}: {1}",MESSAGE, message))
        {
        
        }

        public EntityLoadException(string message, Exception innerException)
            : base(string.Format("{0} : {1}", MESSAGE, message), innerException)
        {

        }
    }
}
