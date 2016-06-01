using System;
using System.Collections.Generic;
using System.Text;

namespace PersistentManager.Exceptions.EntityManagerException
{
    public class PersistCommitException : PersistentException
    {
        private const string MESSAGE = "Could not commit entity changes";

        public PersistCommitException()
            : base(MESSAGE)
        {
        }

        public PersistCommitException(string message, Type entityType)
            : base(string.Format("{0} {1}: {2}", MESSAGE, entityType.Name, message))
        {

        }

        public PersistCommitException(string message, Type entityType, Exception innerException)
            : base(string.Format("{0} {1}: {2}", MESSAGE, entityType.Name, message), innerException)
        {

        }
    }
}
