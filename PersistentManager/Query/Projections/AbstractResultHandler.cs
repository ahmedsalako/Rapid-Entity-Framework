using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data;
using PersistentManager.Descriptors;
using PersistentManager.Metadata;
using PersistentManager.Query.Projections.ReturnTypes;
using PersistentManager.Query.Keywords;
using PersistentManager.Initializers;

namespace PersistentManager.Query.Projections
{
    internal abstract class AbstractResultHandler
    {
        internal IDictionary<string , object> NamedResults { get; set; }
        internal List<Criteria> ProjectedCriterias { get; set; }
        internal IList<DataRow> ResultSets {get; set;}
        internal ProjectionBinder Result { get; set; }

        protected SessionRuntime Runtime { get; set; }
        protected DataTable DataTable { get; set; }
        protected Type Type { get; set; }

        internal AbstractResultHandler( )
        {
            this.ResultSets = new List<DataRow>();
            this.DataTable = new DataTable( );
            this.Runtime = SessionRuntime.GetInstance();
            this.NamedResults = new Dictionary<string , object>( );
        }

        protected void AddColumns( List<string> columns )
        {
            foreach ( int index in columns.GetIndices( ) )
            {
                AddColumns( "col" + index );
            }
        }

        protected void AddColumns( string columnName )
        {
            DataTable.Columns.Add( new DataColumn( columnName , typeof( object ) ) );
        }

        protected void BindResult( object[] values )
        {
            if ( DataTable.Columns.Count <= 0 )
            {
                foreach( int index in values.GetIndices())
                {
                    AddColumns( "col" + index );
                }
            }

            DataRow row = DataTable.NewRow( );
            foreach ( int index in values.GetIndices( ) )
            {
                row[index] = DataType.ConvertValue( DataTable.Columns[index].DataType , values[index] );
            }

            ResultSets.Add( row );
        }

        protected void BindResult( object value )
        {
            DataRow Row = DataTable.NewRow( );
            Row[0] = value;
            ResultSets.Add( Row );
        }

        internal object HandleMany( PropertyMetadata property , params object[] values )
        {
            string[] joins = property.IsManyToMany ? property.LeftKeys : property.JoinColumns;

            if( property.IsManyToMany )
            {

                ManyToManyLazyHandler handler = new ManyToManyLazyHandler
                                                    ( property.DeclaringType ,
                                                      null ,
                                                      property ,
                                                      values
                                                    );

                Type genericType = typeof( IList<> ).MakeGenericType( new Type[] { property.RelationType } );
                Type returnType = ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor( genericType );

                return ConcreteCollectionDiscovery.MakeInstance( returnType , handler , property.RelationType );
            }
            else
            {

                object value = Runtime.GetAllLazily
                                            (
                                                property.RelationType ,
                                                property.PropertyType.IsGenericType ,
                                                joins ,
                                                values
                                            );
                return value;
            }

        }

        internal object HandleDefered( IDeferedExecution execution , IDataReader DataReader )
        {
            return ( ( DeferedHandler )execution ).Rebind( ProjectedCriterias , DataReader );
        }

        internal object HandleCorrelation( PropertyBound property , IDataReader DataReader )
        {
            if ( property.Criteria.Cast<Criteria>().IsDeferedCorrelation )
            {
                IDictionary<string , object> NamedResults = new Dictionary<string , object>();
                Criteria criterion = property.Criteria.Cast<Criteria>( );
                SyntaxContainer syntax = criterion.CorrelatedSubQuery;     
           
                //Check if there is any cascading child that references this property and 

                foreach( Criteria criteria in ProjectedCriterias )
                {                 
                    Criteria join = syntax.GetQueryByJoin( criteria.Hash ).FirstOrDefault( );

                    if( join.IsNotNull() )
                    {
                        join.Value = DataReader[criteria.Alias]; //Joiner Value a hack
                    }                  
                }

                foreach ( PropertyBound bound in Result.Properties )
                {
                    if ( bound.Criteria.IsNotNull( ) && !bound.Criteria.Cast<Criteria>().IsProjected )
                    {
                        Criteria criteria = syntax.GetQueryByJoin( bound.Criteria.Cast<Criteria>( ).Hash ).FirstOrDefault( );

                        if ( criteria.IsNotNull( ) )
                        {
                            criteria.Value = DataReader[bound.Criteria.Alias]; //Joiner Value a hack
                        }
                    }
                }

                Type projection = ReturnType.GetPropertyType( Type , property.LeftName , true );
                syntax = ReturnType.SetReturnType( projection , criterion.DeclaringType , syntax );

                IList entities = ( IList )ConcreteCollectionDiscovery
                    .GenericCreate( typeof( List<> ) , projection );                

                entities = new DeferedExecution<object>( syntax ).ExecuteCompilerGenerated( entities );

                return Queryable.AsQueryable( entities );
            }
            else
            {
                return DataReader[property.Criteria.Alias];
            }
        }

        internal object HandleSingle( PropertyMetadata property , object[] values )
        {
            return Runtime.FindFirst( property.RelationType , property.JoinColumns , values );               
        }

        internal object HandleSingle( Type type , object[] values )
        {
            return Runtime.FindFirst( type , MetaDataManager.GetUniqueKeyNames( type ) , values );
        }

        internal object LoadSingleWithoutRead( Type type , IDataReader dataReader )
        {
            return Runtime.LoadSingleWithoutRead( type , dataReader );
        }

        internal object LoadSingleWithoutRead( Type type , CompositeCriteria criteria , IDataReader dataReader )
        {
            return Runtime.LoadSingleWithoutRead( type , criteria , dataReader );
        }

        internal object HandleSingleValue( PropertyMetadata property , params object[] values )
        {
            return DataType.ConvertValue( property.PropertyType , values[0] );
        }

        internal abstract IList<DataRow> PrepareResult( IDataReader DataReader );
    }
}
