using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Descriptors;
using System.Data;
using System.Collections;
using PersistentManager.Query.Projections.ReturnTypes;
using System.Reflection;
using PersistentManager.Metadata;

namespace PersistentManager.Query.Projections
{
    internal class ClassResultHandler : AbstractResultHandler
    {
        internal QueryReturnType QueryReturnType { get; set; }
        internal Queue<Criteria> Criterias { get; set; }

        internal ClassResultHandler( Type type , QueryReturnType queryReturnType , ProjectionBinder Result , List<Criteria> criterias , Queue<IDeferedExecution> defered )
            : base()
        {
            ProjectedCriterias = criterias.Where( c => c.IsProjected ).ToList( );
            criterias = criterias.Where( c => !c.IsProjected ).ToList( );

            Criterias = new Queue<Criteria>( criterias );

            if ( queryReturnType == QueryReturnType.CompilerGenerated )
            {
                this.Result = ProjectionBinder.GetProjectionsFromCompilerGenerated( type , Criterias , ProjectedCriterias , defered );
                this.AddColumns( "instance" );
            }
            else if ( queryReturnType == QueryReturnType.KnownClassType )
            {
                this.Result = ProjectionBinder.RebindProjections( Result , Criterias );
                this.AddColumns( "instance" );
            }
            else
            {
                this.Result = ProjectionBinder.RebindProjectionsForDataTable( Criterias );
                this.AddColumns( criterias.Select( c => c.Name ).ToList( ) );
            }

            this.Criterias = this.Result.Unused;
            this.QueryReturnType = queryReturnType;            
            this.Type = type;   
        }

        internal override IList<DataRow> PrepareResult( IDataReader DataReader )
        {
            NamedResults = new Dictionary<string , object>( );

            while ( DataReader.Read( ) )
            {
                SetValues( BindResults( DataReader )  );
            }

            return ResultSets;
        }

        internal IList<DataRow> PrepareResultForDataTable( IDataReader DataReader )
        {
            NamedResults = new Dictionary<string , object>( );

            while ( DataReader.Read( ) )
            {
                BindResults( DataReader );
            }

            return ResultSets;
        }

        private object BindResults( IDataReader DataReader )
        {
            IList<PropertyBound> Compositions = new List<PropertyBound>( );

            foreach ( PropertyBound binding in Result.Properties )
            {
                PropertyMetadata property = binding.Metadata;

                if ( binding.IsDefered )
                {
                    Set( binding.LeftName , HandleDefered( ( IDeferedExecution )binding.Value , DataReader ) );
                }
                else if ( binding.IsExtendedProjection )
                {
                    Set( binding.LeftName , binding.ResultHandler.BindResults( DataReader ) );
                }
                else if ( binding.Criteria.HasCorrelatedPath( ) )
                {
                    Set( binding.LeftName , HandleCorrelation( binding , DataReader ) );
                }
                else if ( binding.LeftType.IsPersistenceEntity( ) || ( property.IsNotNull( ) && property.IsOneSided )  )
                {
                    CompositeCriteria composite = ( CompositeCriteria )binding.Criteria;
                    Type type = property.IsNotNull( ) ? property.PropertyType : composite.DeclaringType;

                    Set( binding.LeftName , LoadSingleWithoutRead( type , composite , DataReader ) );
                }
                else if ( property.IsNotNull( ) && property.IsManySided )
                {
                    CompositeCriteria composite = ( CompositeCriteria )binding.Criteria;

                    Set(
                            binding.LeftName ,
                            HandleMany( property , composite.GetValues( DataReader ).ToArray( ) )
                        );
                }
                else if ( property.IsNotNull( ) && property.IsOneSided )
                {
                    CompositeCriteria composite = ( CompositeCriteria )binding.Criteria;

                    Set( binding.LeftName , HandleSingle( property , composite.GetValues( DataReader ).ToArray( ) ) );
                }
                else if ( property.IsNotNull( ) && property.IsPlaceHolding && binding.Criteria is CompositeCriteria )
                {
                    CompositeCriteria composite = ( CompositeCriteria )binding.Criteria;
                    object placeholderInstance = MetaDataManager.MakeInstance( binding.Metadata.PropertyType );

                    foreach ( Criteria criteria in composite.Criterions )
                    {
                        MetaDataManager.SetPropertyValue( criteria.Name , placeholderInstance , DataReader[criteria.Alias] );
                    }

                    Set( binding.LeftName , placeholderInstance );
                }
                else
                {
                    object value = DataReader[binding.Criteria.Alias];

                    if ( value is DBNull )
                    {
                       value = Null.Default( binding.LeftType );
                    }

                    Set( binding.LeftName , value );
                }
            }

            return BindAndReturn( );
        }

        protected object BindAndReturn( )
        {
            try
            {
                if ( QueryReturnType == QueryReturnType.CompilerGenerated )
                {
                    return ReturnType.CreateAnonymousInstance( Type , NamedResults );
                }
                else if ( QueryReturnType == QueryReturnType.KnownClassType )
                {
                    return ReturnType.CreateKnownType( Type , NamedResults );
                }
                else
                {
                    BindResult( NamedResults.Values.ToArray( ) );
                    return null;
                }
            }
            finally
            {
                NamedResults.Clear( );
            }
        }

        protected void SetValues( object value )
        {
            BindResult( value );            
        }

        protected void Set( string property , object value )
        {
            if ( QueryReturnType != QueryReturnType.DataTable )
            {
                PropertyInfo propertyInfo = Type.GetProperty( property );

                if ( propertyInfo.PropertyType.IsValueType )
                {
                    NamedResults.Add( property , DataType.ConvertValue( propertyInfo.PropertyType , value ) );
                }
                else
                {
                    NamedResults.Add( property , value );
                }
            }
            else
            {
                NamedResults.Add( property , value );
            }
        }
    }
}
