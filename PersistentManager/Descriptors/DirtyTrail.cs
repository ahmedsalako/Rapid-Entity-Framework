using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Cache;
using PersistentManager.Initializers.Interfaces;
using PersistentManager.Metadata;
using PersistentManager.Descriptors;

namespace PersistentManager
{
    public class DirtyTrail : IEnumerable , IEnumerable<DirtyState>
    {
        IDictionary<string , DirtyState> Audits { get; set; }
        internal QueryType QueryType { get; private set; }
        internal string EntityName { get; private set; }
        internal Type EntityType { get; private set; }
        internal string Key { get; private set; }        

        internal DirtyState this[string i]
        {
            get { return Audits[i]; }
        }

        private DirtyTrail( QueryType queryType , Type entityType , object entity )
        {
            EntityName = entityType.Name;
            EntityType = entityType;
            QueryType = queryType;

            if ( QueryType == QueryType.Update )
            {
                Audits = new Dictionary<string , DirtyState>( );

                //There is no where else to check OneSided Changes, so it gat to be here
                Audits = DirtyState.CreateForUpdate( entityType , entity );
            }
            else
            {
                Audits = DirtyState.CreateForCreate( entityType , entity );
            }
        }

        internal DirtyTrail( QueryType queryType , Type type , object entity , string key ) 
            : this( queryType , type , entity )
        {
            Key = key;            
        }

        internal int Count
        {
            get
            {
                return Audits.Count;
            }
        }

        internal void AddAudit( string propertyName , string columnName , object value )
        {
            if ( Audits.ContainsKey( propertyName ) )
            {
                Audits[propertyName].Value = value;
            }
            else
            {
                Audits.Add( propertyName , new DirtyState( ) { PropertyName = propertyName , Value = value , ColumnName = columnName } );
            }
        }

        internal bool OwnsProperty( string propertyName )
        {
            return EntityType.GetMetataData().PropertyMapping( propertyName ).IsNotNull();
        }

        internal object GetPropertyValue( string propertyName )
        {
            if( IsAudited( propertyName ) )
            {
                return Audits[propertyName].Value;
            }

            return null;
        }

        private bool IsAudited( string propertyName )
        {
            return Audits.ContainsKey( propertyName ) ;
        }

        internal bool WasAudited( string property )
        {
            return IsAudited( property );
        }

        public bool ValuesAreEquals( string propertyName , object value )
        {
            if( this[propertyName].Value.IsNull( ) && value.IsNotNull( ) )
                return value.Equals( null );
            else if( this[propertyName].Value.IsNull( ) && value.IsNull( ) )
                return true;

            return this[propertyName].Value.Equals( value );
        }

        public IEnumerator GetEnumerator( )
        {
            return Audits.Values.GetEnumerator( );
        }

        #region IEnumerable<KeyValuePair<string,object>> Members

        IEnumerator<DirtyState> IEnumerable<DirtyState>.GetEnumerator( )
        {
            return Audits.Values.GetEnumerator( );
        }

        #endregion        
    }

    internal class DirtyState
    {
        internal string PropertyName { get; set; }
        internal string ColumnName { get; set; }
        internal object Value { get; set; }

        internal static IDictionary<string, DirtyState> CreateForUpdate( Type type , object entity )
        {
            IDictionary<string, DirtyState> changes = new Dictionary<string, DirtyState>();
            EntityMetadata entityMetadata = type.GetMetataData();

            foreach (PropertyMetadata column in entityMetadata.GetAll().Where(c => !c.IsManyToMany && !c.IsOneToMany && !c.IsImported && !c.IsPlaceHolding))
            {
                if (column.IsOneToOne || column.IsManyToOne)
                {
                    changes = CreateForOneToOne(type, entity, column, changes);
                }
            }

            return changes;
        }

        internal static IDictionary<string , DirtyState> CreateForCreate( Type type , object entity )
        {
            IDictionary<string , DirtyState> changes = new Dictionary<string , DirtyState>( );
            EntityMetadata metadata = type.GetMetataData( );

            foreach ( PropertyMetadata column in metadata.GetAll( ).Where( c => !c.IsManyToMany && !c.IsOneToMany && !c.IsImported && !c.IsPlaceHolding ) )
            {
                
                if ( column.IsOneToOne || column.IsManyToOne )
                {
                    changes = CreateForOneToOne( type , entity , column , changes );
                }
                else if ( column.IsDiscriminator )
                {
                    Add( changes , column.MappingName , new DirtyState { PropertyName = column.MappingName , ColumnName = column.MappingName , Value = column.FieldValue } );
                }
                else if ( column.IsCompositional && !column.IsAutoGenerated )
                {
                    object placeholder = MetaDataManager.GetPropertyValue( metadata.Placeholders.FirstOrDefault( p => p.CompositeId == column.CompositeId ).ClassDefinationName , entity );
                    object value = MetaDataManager.GetPropertyValue( column.ClassDefinationName , placeholder );

                    Add( changes , column.ClassDefinationName , new DirtyState { PropertyName = column.ClassDefinationName , ColumnName = column.MappingName , Value = value } );
                }
                else if ( column.IsInheritance && !column.IsAutoGenerated )
                {
                    Add(changes, column.ClassDefinationName,
                            new DirtyState
                            {
                                PropertyName = column.ClassDefinationName,
                                ColumnName = column.MappingName,
                                Value = MetaDataManager.GetPropertyValue( column.ClassDefinationName , entity )
                            }
                        );
                }
                else if ( !column.IsAutoGenerated )
                {
                    object value = MetaDataManager.GetPropertyValue( column.ClassDefinationName , entity );
                    Add( changes , column.ClassDefinationName , new DirtyState { PropertyName = column.ClassDefinationName , ColumnName = column.MappingName , Value = value } );
                }
            }

            return changes;
        }

        private static IDictionary<string, DirtyState> CreateForOneToOne( Type type , object entity , PropertyMetadata column , IDictionary<string , DirtyState> changes )
        {
            if ( column.IsOneToOne || column.IsManyToOne )
            {
                object relation = MetaDataManager.GetPropertyValue(column.ClassDefinationName, entity);

                if ( column.IsImported || relation.IsNull() )
                    return changes;
                   
                if (column.IsEntitySplitJoin)
                {
                    foreach (var change in CreateForCreate(column.PropertyType, relation).Values)
                    {
                        if (!changes.ContainsKey(change.PropertyName))
                        {
                            changes.Add(change.PropertyName, change);
                        }
                    }
                }
                else
                {
                    object[] keys = MetaDataManager.GetUniqueKeys(relation);
                    EntityMetadata parentMetadata = EntityMetadata.GetMappingInfo(column.PropertyType);

                    foreach (JoinMetadata join in column.JoinDetails)
                    {
                        DirtyState change = new DirtyState();
                        change.PropertyName = join.RelationColumn;
                        change.ColumnName = join.RelationColumn;
                        change.Value = keys[parentMetadata.GetKeyIndexByMapping(join.JoinColumn)];
                        Add(changes, change.PropertyName, change);
                    }
                }
            }

            return changes;
        }

        internal static void Add( IDictionary<string , DirtyState> changes  , string key , DirtyState value )
        {
            if  ( IsValid( value ) )
            {
                if (!changes.ContainsKey(key))
                {
                    changes.Add(key, value);
                }
            }
        }

        internal static bool IsValid( DirtyState value )
        {
            if (null != value.Value && value.Value.GetType() == typeof(DateTime))
            {
                if (((DateTime)value.Value) == DateTime.MinValue)
                {
                    return false;
                }
            }
            else if (value.Value == null)
            {
                return false;
            }

            return true;
        }
    }
}
