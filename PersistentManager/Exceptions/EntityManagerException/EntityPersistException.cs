using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Exceptions.EntityManagerException
{
    public class EntityPersistException : PersistentException
    {
        private const string MESSAGE = "Could not persist entity";

        public EntityPersistException()
            : base(MESSAGE)
        {
        }

        public EntityPersistException(string message, Exception ex)
            :base(string.Format("{0} : {1}",MESSAGE, ex.Message))
        {
        
        }
    }
}
