using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using System.Data;
using System.Data.Common;
using PersistentManager.Metadata;
using PersistentManager.Util;
using PersistentManager.Cache;
using PersistentManager.Query.QueryEngine;
using PersistentManager.Query.Sql;
using System.Reflection;
using System.Linq;
using PersistentManager.Query.Projections;
using PersistentManager.Query.Projections.ReturnTypes;
using PersistentManager.Initializers;

namespace PersistentManager.Query
{
    internal class QueryResult : IQueryResult
    {
        private SyntaxContainer syntax;
        private DataRow[] resultRows;
        private DbDataReader dataReader;
        private SQLTokenizer sqlTokens;

        internal QueryResult( SyntaxContainer syntax , DbDataReader dataReader , SQLTokenizer sqlTokens )
        {
            this.syntax = syntax;
            this.dataReader = dataReader;
            this.sqlTokens = sqlTokens;
            resultRows = BindResults( );
        }

        public DataRow[] Rows
        {
            get
            {
                return resultRows;
            }
        }

        public StringBuilder QueryString
        {
            get { return sqlTokens.Tokens; }
        }

        public DbDataReader DataReader
        {
            get { return dataReader; }
            set { dataReader = value; }
        }

        public object this[int index , int column]
        {
            get
            {
                return Rows[index][column];
            }
        }

        public IEnumerator GetEnumerator( )
        {
            return Rows.GetEnumerator( );
        }

        private bool IsColumnBasedQuery( )
        {
            return ( sqlTokens.SelectArguments.Count > 0 );
        }

        public IEnumerable<T> ConvertToProjection<T>( T value )
        {
            foreach ( DataRow row in resultRows )
            {
                object[] values = new object[row.ItemArray.Length];

                foreach( int index in row.ItemArray.GetIndices())
                {
                    if ( row.ItemArray[index] is DBNull )
                    {
                        PropertyInfo property = value.GetType( ).GetProperties( )[index];
                        values[index] = Null.Default( property.PropertyType );
                    }
                    else
                    {
                        values[index] = row.ItemArray[index];
                    }
                }

                yield return ( T )Activator.CreateInstance( typeof( T ) , values );
            }
        }

        private List<Criteria> RebindCriteria( List<Criteria> criterias )
        {
            List<Criteria> newCriterias = new List<Criteria>( );

            foreach ( var group in criterias.GroupBy( c => c.Ordinal ) )
            {
                if ( group.Count( ) > 1 || group.First( ).GroupId != Guid.Empty )
                {
                    newCriterias.Add(
                                        new CompositeCriteria
                                            (
                                                group.ToList( ) ,
                                                CompositeType.PlaceHolding 
                                            )
                                            as Criteria
                                    );
                }
                else
                {
                    foreach ( Criteria criteria in group.ToList( ) )
                    {
                        newCriterias.Add( criteria as Criteria );
                    }
                }
            }

            return newCriterias;
        }

        private DataRow[] BindResults( )
        {
            List<Criteria> criterias = syntax.GetQueryByPart( QueryPart.SELECT )
                                             .Where( c => !c.ContainedInFunction )
                                             .OrderBy( c => c.Ordinal ).ToList( );

            criterias = RebindCriteria( criterias );

            List<DataRow> columnResult = new List<DataRow>( );

            if ( IsColumnBasedQuery( ) && syntax.ResultIsCompilerGenerated )
            {
                return new ClassResultHandler(
                            syntax.CompilerGeneratedResultType ,
                            QueryReturnType.CompilerGenerated ,
                            null ,
                            criterias ,
                            new Queue<IDeferedExecution>( syntax.DeferedSelect )
                            )
                            .PrepareResult( dataReader ).ToArray( );
            }
            else if ( syntax.HasProjectionBinding )
            {
                return new ClassResultHandler(
                            syntax.Factory.ProjectionBinder.LeftAssignment ,
                            QueryReturnType.KnownClassType ,
                            syntax.Factory.ProjectionBinder ,
                            criterias ,
                            new Queue<IDeferedExecution>( syntax.DeferedSelect )
                            )
                            .PrepareResult( dataReader ).ToArray( );
            }
            else
            {
                return new ClassResultHandler(
                            typeof(DataRow) ,
                            QueryReturnType.DataTable ,
                            null ,
                            criterias ,
                            new Queue<IDeferedExecution>( syntax.DeferedSelect )
                            )
                            .PrepareResultForDataTable( dataReader ).ToArray( );
            }
        }
    }
}
