using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

namespace PersistentManager.Query
{
    public interface IQueryResult : IEnumerable
    {
        DataRow[] Rows { get;}
        object this[int index, int column] { get; }
        StringBuilder QueryString { get; }
        IEnumerable<T> ConvertToProjection<T>( T value );
    }
}
