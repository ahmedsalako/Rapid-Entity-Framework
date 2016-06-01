using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.Projections
{
    internal class ProjectionBinder
    {
        internal Queue<PropertyBound> properties = new Queue<PropertyBound>( );
        internal Type RightAssignment { get; set; }
        internal Type LeftAssignment { get; set; }
        internal Queue<Criteria> Unused { get; set; }

        internal List<PropertyBound> Properties
        {
            get { return properties.ToList() ;  }
        }        

        internal ProjectionBinder( )
        {

        }

        internal ProjectionBinder( Type leftAssignment , Type rightAssignment  )
            : this( )
        {
            LeftAssignment = leftAssignment;
            RightAssignment = rightAssignment;
        }

        internal void AddProperty( string leftName , Type type , string rightName  , object value )
        {
            properties.Enqueue( new PropertyBound( leftName , type , rightName , value ) );        
        }

        internal void AddProperty( PropertyBound property )
        {
            properties.Enqueue( property );
        }

        internal object[] GetProperties()
        {
            return GetProjectedPropertyValues().ToArray();
        }

        internal IEnumerable<object> GetProjectedPropertyValues()
        {
            foreach (PropertyBound property in Properties)
            {
                yield return property.Value;
            }
        }

        public static ProjectionBinder RebindProjections( ProjectionBinder Binder , Queue<Criteria> criterias )
        {
            foreach ( int index in criterias.GetIndices( ) )
            {
                Criteria criteria = criterias.Dequeue( );
                string name = criteria.Name.EraseAlias( );

                PropertyMetadata metadata = null;

                if ( Binder.Properties[index].LeftType.IsClassOrInterface( ) )
                {
                    metadata = criteria.DeclaringType.GetMetataData( )
                        .PropertyMapping( name );
                }

                Binder.Properties[index].Criteria = criteria;
                Binder.Properties[index].Metadata = metadata;
                Binder.Properties[index].RightName = name;
                Binder.Properties[index].Value = criteria.HasCorrelatedPath( ) ? criteria : null;
            }

            return Binder;
        }

        public static ProjectionBinder RebindProjectionsForDataTable( Queue<Criteria> criterias )
        {
            ProjectionBinder binder = new ProjectionBinder( );

            foreach ( int index in criterias.GetIndices( ) )
            {
                Criteria criteria = criterias.Dequeue( );
                string name = criteria.Name.EraseAlias( );

                PropertyMetadata metadata = criteria.DeclaringType.GetMetataData( )
                                            .PropertyMapping( criteria.PropertyName ?? criteria.Name );

                Type leftType = null;

                if ( metadata.IsNull( ) )
                {
                    if ( criteria is CompositeCriteria )
                    {
                        leftType = criteria.DeclaringType;
                    }
                    else
                    {
                        leftType = typeof( object );
                    }
                }
                else
                {
                    leftType = metadata.PropertyType;
                }

                PropertyBound property = new PropertyBound( name , leftType , string.Empty , null , metadata , criteria );
                binder.AddProperty( property );
            }

            return binder;
        }

        public static ProjectionBinder GetProjectionsFromCompilerGenerated( Type Projected , Queue<Criteria> criterias , List<Criteria> projectedCriterias , Queue<IDeferedExecution> defered )
        {
            ParameterInfo[] parameters = Projected.GetConstructors( )[0].GetParameters( );
            ProjectionBinder binder = new ProjectionBinder( );
            binder.Unused = criterias;

            foreach ( int index in parameters.GetIndices( ) )
            {
                ParameterInfo parameter = parameters[index];
                
                if ( parameter.ParameterType.IsDeferedExecution( ) )
                {
                    binder.AddProperty( new PropertyBound( parameter.Name , parameter.ParameterType ,  true , defered.Dequeue( ) ) );
                    continue;
                }
                else if ( parameter.ParameterType.IsCompilerGenerated( ) )
                {
                    ClassResultHandler handler = new ClassResultHandler
                                                (
                                                   parameter.ParameterType ,
                                                   QueryReturnType.CompilerGenerated ,
                                                   null ,
                                                   binder.Unused.ToList( ) ,
                                                   new Queue<IDeferedExecution>( )
                                                );

                    handler.ProjectedCriterias = projectedCriterias;

                    binder.AddProperty( new PropertyBound
                                            (
                                                parameter.Name ,
                                                parameter.ParameterType ,
                                                string.Empty ,
                                                null ,
                                                null ,
                                                handler
                                             )
                                      );

                    binder.Unused = handler.Criterias ;

                    continue;
                }

                Criteria criteria = binder.Unused.Dequeue( );

                if ( criteria is Criteria && criteria.Cast<Criteria>( ).HasCorrelatedPath( ) )
                {
                    binder.AddProperty( new PropertyBound( parameter.Name , parameter.ParameterType , string.Empty , null , null , criteria ) );
                }
                else if ( criteria is CompositeCriteria )
                {
                    PropertyMetadata metadata = EntityMetadata.GetMappingInfo( criteria.Cast<Criteria>( ).DeclaringType )
                                                             .PropertyMapping( criteria.PropertyName ?? criteria.Name );

                    binder.AddProperty( new PropertyBound( parameter.Name , parameter.ParameterType , string.Empty , null , metadata , criteria ) );
                }
                else if( !parameter.ParameterType.IsDeferedExecution( ) )
                {
                    PropertyMetadata metadata = EntityMetadata.GetMappingInfo( criteria.Cast<Criteria>( ).DeclaringType )
                                                             .PropertyMapping( criteria.PropertyName ?? criteria.Name );

                    binder.AddProperty( new PropertyBound( parameter.Name , parameter.ParameterType , criteria.PropertyName , null , metadata , criteria.Cast<Criteria>( ) ) );
                }
            }

            return binder;
        }
    }
}
