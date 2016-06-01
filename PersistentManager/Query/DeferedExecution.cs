using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PersistentManager.Query.Keywords;
using System.Data;
using System.Diagnostics;
using PersistentManager.Descriptors;
using PersistentManager.Query.Sql;
using System.Data.Common;
using PersistentManager.Ghosting;
using PersistentManager.Query.Projections.ReturnTypes;
using PersistentManager.Metadata;

namespace PersistentManager.Query
{
    public interface IDeferedExecution 
    { 
    }

    public abstract class DeferedHandler 
    {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        protected SyntaxContainer Syntax{ get; set; }

        internal DeferedHandler Rebind( List<Criteria> criterias , IDataReader DataReader )
        {
            foreach ( Criteria criteria in criterias )
            {
                if ( criteria is CompositeCriteria )
                {
                    foreach ( Criteria criteria1 in ( criteria as CompositeCriteria ).Criterions )
                    {
                        Add( DataReader , criteria1 );
                    }
                }
                else
                {
                    Add( DataReader , criteria );
                }                
            }

            return this;
        }

        private void Add( IDataReader DataReader , Criteria criteria )
        {
            Criteria join = Syntax.GetQueryByJoin( criteria.Hash ).FirstOrDefault( );

            if ( join.IsNotNull( ) )
            {
                join.Value = DataReader[criteria.Alias];
            }
        }
    }

    public class DeferedExecution<T> : DeferedHandler , IDeferedExecution
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private T selector;

        internal DeferedExecution( SyntaxContainer syntax )
        {
            this.Syntax = syntax;
        }

        internal DeferedExecution( T selector , SyntaxContainer syntax )
        {
            this.selector = selector;
            this.Syntax = syntax;
        }

        public IList<T> Execute( )
        {
            return ExecuteInternal( ).ToList( );
        }

        public IList<T> Execute<T>( T anonymous )
        {
            Syntax.ReturnType = typeof( T );
            Syntax.ResultIsCompilerGenerated = true;
            Syntax.CompilerGeneratedResultType = Syntax.ReturnType;


            return ExecuteInternal<T>( Syntax.SelectResult( ) ).ToList( );
        }

        public IList<T> Limit( int Start , int End)
        {
            Syntax.Range( Start , End );

            return ExecuteInternal().ToList();
        }

        internal  QueryContext LimitQuery( int Start , int End )
        {
            Syntax.Range( Start , End );

            return Syntax.ExecuteRange( Start , End );
        }

        public IList<T> Reverse( )
        {
            IList<T> results = Execute();

            if (results.IsNotNull())
            {
                results.Reverse().ToList();
            }

            return results;
        }

        public T Last( )
        {
            Syntax.OrderBy = Syntax.OrderBy == ORDERBY.ASC ? ORDERBY.DESC : ORDERBY.ASC;

            return First( );
        }

        public int Count()
        {
            Syntax.RemovePart(QueryPart.SELECT);
            Syntax.RemovePart(QueryPart.ORDERBY);

            Syntax.Add( QueryPart.FUNCTION , ProjectionFunction.COUNT , 1 );
            Syntax.FunctionType = ProjectionFunction.COUNT;
            Syntax.IsScalar = true;

            object result = Syntax.ExecuteScalarInternal().ScalarResult;
            return ( int ) DataType.ConvertValue( typeof( int ) , result );
        }

        public SyntaxContainer GetSyntax( )
        {
            return Syntax;
        }

        public bool Any()
        {
            return Count() > 0;
        }

        public IList<T> Take( int count )
        {
            AddTopCriteria( count );

            return ExecuteInternal( ).ToList( );
        }

        public T First()
        {
            AddTopCriteria( 1 );

            return ExecuteInternal( ).ToList( ).FirstOrDefault( );
        }

        internal object LastEntity( IList list )
        {
            Syntax.OrderBy = Syntax.OrderBy == ORDERBY.ASC ? ORDERBY.DESC : ORDERBY.ASC;

            return FirstEntity( list );
        }

        internal object FirstEntity( IList list )
        {
            AddTopCriteria( 1 );

            foreach ( var value in ExecuteCompilerGenerated( list ) )
                return value;

            return null;
        }

        private void AddTopCriteria( int count )
        {
            Syntax.FunctionType = ProjectionFunction.TOP;
            Syntax.Add( QueryPart.FUNCTION , ProjectionFunction.TOP , count );
        }

        public IList<T> Distinct( )
        {
            Syntax.IsDistinct = true;
            return ExecuteInternal( ).ToList( );
        }

        internal IList ExecuteCompilerGenerated( IList list )
        {
            IQueryResult result = Syntax.SelectResult( );

            foreach ( DataRow value in result.Rows )
            {
                if ( !value.IsDBNull( ) )
                {
                    list.Add( value[0] );
                }
            }

            return list;
        }

        internal IEnumerable<R> ExecuteInternal<R>( IQueryResult result )
        {
            if ( typeof( R ) == typeof( IQueryResult ) )
                yield return ( R )result;
            else
            {
                foreach ( DataRow row in result.Rows )
                {
                    yield return row[0] is DBNull ? default( R ) : ( R ) row[0];
                }
            }
        }

        internal IEnumerable<T> ExecuteInternal( )
        {
            return ExecuteInternal<T>( Syntax.SelectResult() );
        }

        internal QueryContext ExecuteQuery( )
        {
            return Syntax.ExecuteQuery( );
        }

        internal QueryContext FirstInternal( )
        {
            AddTopCriteria( 1 );

            return ExecuteQuery( );
        }

        internal QueryContext TakeInternal( int count )
        {
            AddTopCriteria( count );

            return ExecuteQuery( );
        }

        internal QueryContext DistinctInternal()
        {
            Syntax.IsDistinct = true;
            return ExecuteQuery( );
        }
    }
}
