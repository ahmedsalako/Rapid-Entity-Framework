using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.Common;
using System.Data;

namespace PersistentManager.Initializers.Interfaces
{
    public interface ILazyLoader
    {
        IList Orphans { get; set; }
        IDictionary<Guid , List<string>> LoadTrack { get; set;}
        object[] OwnerKey { get; set; }
        object GetAllType( );
        object GetType( );
        void PersistChildObject( object child );
        void SaveAllChanges( );
        bool HasChanges { get; }
        bool ChildExist( object child );
        int Count { get; }
        int CountDataStore { get; set; }
        bool RemoveChild( object child );
        object GetIndex( int index );
        void RemoveAt( int index );
        IDataReader GetDataReader( );
        IEnumerable<T> GetEntityReader<T>( );
        void RemoveAllChildren( );
        void Add( object child );
        void ClearOrphans( );
    }
}
