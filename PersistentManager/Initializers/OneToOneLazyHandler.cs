using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Initializers.Interfaces;
using System.Reflection;
using PersistentManager.Descriptors;
using System.Collections;
using System.Data.Common;
using PersistentManager.Query;
using PersistentManager.Query.Sql;
using System.Data;
using PersistentManager.Cache;
using PersistentManager.Util;
using PersistentManager.Metadata;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Initializers
{
    public class OneToOneLazyHandler : Lazy , ILazyLoader
    {
        PropertyInfo ownerProperty;
        private Type lazyType;
        GetType _GetType;
        Type ownerType;        

        public OneToOneLazyHandler( Type entityType, PropertyInfo property, PropertyMetadata propertyMetadata, object[] entityKeys )
            : base( property.PropertyType )
        {
            this.lazyType = propertyMetadata.RelationType ;
            this.OwnerKey = entityKeys ;
            this.ownerProperty = property;
            this.ownerType = entityType ;
            this.propertyMetadata = propertyMetadata;
        }

        public object Entity { get; set; }

        public object[] OwnerKey { get; set; }

        public object[] RelationKeys { get; set; }

        public IEnumerable<object> ReferencedKeys
        {
            get
            {
                foreach ( JoinMetadata join in propertyMetadata.JoinDetails )
                {
                    int keyIndex = EntityMetadata.GetMappingInfo( ownerType )
                        .GetKeyIndexByMapping( join.JoinColumn );

                    yield return OwnerKey[keyIndex];
                }
            }
        }

        public object GetAllType( )
        {
            throw new NotImplementedException( );
        }

        public new object GetType( )
        {
            _GetType = new GetType( GetReference );
            return _GetType.Invoke( lazyType , propertyMetadata.JoinDetails.Select( j => j.RelationColumn ).ToArray()  , OwnerKey );
        }

        public object GetReference( Type layType , string[] relation , object[] ownerKeys )
        {
            if ( IsPersisted )
            {
                try
                {
                    if( Entity.IsNotNull( ) )
                        return Entity;

                    SessionRuntime manager = SessionRuntime.GetInstance( );
                    EntityMetadata metadata = MetaDataManager.PrepareMetadata( ownerType );

                    PropertyMetadata column = metadata.GetPropertyMappingIncludeBase( ownerProperty.Name );

                    if ( column.IsEntitySplitJoin )
                    {
                        return GetEntitySplit( ownerKeys , manager , lazyType );
                    }
                    else
                    {
                        return GetRelatedEntity( ownerKeys , manager , column );
                    }
                }
                catch ( Exception x )
                {
                    return null;
                }
            }
            else if ( HasChanges )
            {
                return Orphans[0];
            }

            return null;
        }

        private object GetEntitySplit( object[] ownerKey , SessionRuntime manager , Type lazyType )
        {
            return Entity = manager.LoadEntity( lazyType , ownerKey );
        }

        private object GetRelatedEntity( object[] ownerKey , SessionRuntime manager , PropertyMetadata column )
        {
            //Imported is used on the entity with the primary keys. The owner of the relationship
            if ( !column.IsNull( ) && column.IsImported )
            {
                return Entity = manager.FindFirst( ownerProperty.PropertyType , RelationColumns , ReferencedKeys.ToArray( ) );
            }

            return Entity = GetRelation( manager );
        }

        internal object GetRelation( SessionRuntime runtime )
        {
            string[] keys = EntityMetadata.GetMappingInfo( ownerType )
                                          .Keys.Select( c => c.ClassDefinationName )
                                          .ToArray( );

            SyntaxContainer syntax = new From( ownerType ).As( "a" )
                                            .Where( keys , OwnerKey )
                                            .Select
                                            (
                                                new[]
                                                { 
                                                    propertyMetadata.ClassDefinationName 
                                                }
                                            )
                                            .GetSyntax( );


            QueryContext context = syntax.ExecuteQuery( );
            context.DataReader.Read( );
             
            return runtime.LoadSingleWithoutRead( propertyMetadata.PropertyType , (CompositeCriteria) syntax.GetQueryByPart( QueryPart.SELECT ).FirstOrDefault( ) , context.DataReader );
        }

        public override void PersistChildObject( object child )
        {
            if ( IsPersisted )
            {
                //Insert entity into datastore
                SessionRuntime manager = SessionRuntime.GetInstance( );
                //EntityMetadata metadata = MetaDataManager.PrepareMetadata( ownerType );

                Entity = manager.CreateOrUpdate( child );

                //Entity = child;
            }
            else if ( Orphans.Count <= 0 )
            {
                Orphans.Add( child );
            }
            else
            {
                Orphans.Clear( );
                Orphans.Add( child );
            }
        }

        public bool ChildExist( object child )
        {
            throw new NotImplementedException( );
        }

        public int CountDataStore
        {
            get
            {
                throw new NotImplementedException( );
            }
            set
            {
                throw new NotImplementedException( );
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException( );
            }
            set
            {
                throw new NotImplementedException( );
            }
        }

        public bool RemoveChild( object child )
        {
            throw new NotImplementedException( );
        }

        public object GetIndex( int index )
        {
            throw new NotImplementedException( );
        }

        public void RemoveAt( int index )
        {
            throw new NotImplementedException( );
        }

        public IDataReader GetDataReader( )
        {
            throw new NotImplementedException( );
        }

        public IEnumerable<T> GetEntityReader<T>( )
        {
            throw new NotImplementedException( );
        }

        public void RemoveAllChildren( )
        {
            throw new NotImplementedException( );
        }
    }
}
