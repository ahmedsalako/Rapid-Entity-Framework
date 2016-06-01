using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace PersistentManager.Descriptors
{
    public interface IParameter<T>
    {
        IParameter<T> Add(object name, Condition condition, object value);
        IEnumerable<T> OrderBy( params object[] names );
        IParameter<T> Add(string name, object value);
        IEnumerable<T> Search();
    }
}
