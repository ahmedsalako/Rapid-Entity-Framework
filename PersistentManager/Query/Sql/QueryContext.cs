using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Descriptors;
using System.Data.Common;
using System.Data;

namespace PersistentManager.Query.Sql
{
    internal class QueryContext
    {
        private StringBuilder query = new StringBuilder();
        private object entityInstance;
        private QueryType queryType;
        private object[] keys;

        private EntityMetadata metaStructure;
        private Type entityType;

        public PropertyMetadata Property { get; set; }
        public bool IsUpdated { get; set; }
        public IDataReader DataReader { get; set; }
        public object ScalarResult { get; set; }

        public Type EntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }

        public Type JoinTableType { get; set; }

        internal string EntityTypeName { get; set; }

        public object EntityInstance
        {
            get { return entityInstance; }
            set { entityInstance = value; }
        }

        internal object[] Keys
        {
            get { return keys; }
            set { keys = value; }
        }

        internal string[] Names {get; set;}

        internal object[] Values { get; set; }

        internal EntityMetadata MetaStructure
        {
            get { return metaStructure; }
            set { metaStructure = value; }
        }

        internal QueryContext( QueryType queryType )
        {
            this.queryType = queryType;
        }

        internal QueryContext( QueryType queryType , Type type , object[] keys , string[] names ) : this( queryType )
        {
            MetaStructure = MetaDataManager.PrepareMetadata( type );
            EntityType = type;
            Names = names ?? MetaDataManager.GetUniqueKeyNames( type ) ;
            Values = keys;
        }

        internal QueryType QueryType
        {
            get { return queryType; }
            set { queryType = value; }
        }

        internal DirtyTrail Audit { get; set; }
        internal int StartRange { get; set; }
        internal int EndRange { get; set; }        
    }
}
