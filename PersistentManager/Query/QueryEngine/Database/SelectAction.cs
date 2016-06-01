using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class SelectAction : ActionBase , IQueryBuilderStrategy
    {
        internal IList<string> arguments = new List<string>( );
        internal IList<Criteria> CriteriasOverrides
        {
            get
            {
                if ( !EnforceDistinct )
                {
                    return base.Criterias.Where( c => !c.ContainedInFunction ).OrderBy( c => c.Ordinal ).ToList();
                }

                return base.Criterias.OrderBy( c => c.Ordinal ).ToList( );
            }
        }      
        internal bool EnforceDistinct { get; set; }
        internal const string OuterMostAlias = "R";        
        internal int aliasCount = 0;

        internal SelectAction( bool distinct ) : base( QueryPart.SELECT ) 
        { 
            EnforceDistinct = distinct; 
        }

        public void Execute( )
        {
            if ( Syntax.Parameters[QueryPart].IsNotNull( ) )
            {
                Tokens.IsColumnBasedQuery = true;
                int count = 0;

                foreach ( Criteria criteria in CriteriasOverrides )
                {
                    if ( criteria.HasCorrelatedPath( ) )
                    {
                        AddCorrelation( criteria , criteria.QueryPart , GetAlias( criteria , string.Empty ) , count );
                    }
                    else if ( criteria is ICompositeCriterion )
                    {
                        bool distinct = EnforceDistinct;

                        if ( !EnforceDistinct )
                        {
                            EnforceDistinct = ( ( ICompositeCriterion )criteria ).CompositeType == CompositeType.EntityClass;
                        }

                        foreach ( Criteria child in ( ( ICompositeCriterion ) criteria ).Criterions )
                        {
                            AddColumn( child , count );
                        }

                        EnforceDistinct = distinct;
                    }
                    else
                    {
                        AddColumn( criteria , count );
                    }
                }
            }
        }

        private void AddCorrelation( Criteria criteria , QueryPart part , string name , int count )
        {
            if ( criteria.CorrelatedPath.MainFunctionType != ProjectionFunction.NOTSET )
            {
                SyntaxContainer subQuery = criteria.CorrelatedSubQuery;
                RDBMSDataStore dataStoreQuery = new RDBMSDataStore( new ContextData( subQuery , subQuery.QueryContext ) , CurrentProvider );
                string query = dataStoreQuery.ExecuteSelect( false ).SQLTokenizer.ToString( );
                criteria.IsDeferedCorrelation = false;

                Tokens.AddFormatted( part , " {0} AS {1} " , "(" + query + ")" , name );
                Tokens.Parameters.AddRange( dataStoreQuery.ContextData.SQLTokenizer.Parameters );                
            }
            else if ( criteria.HasCorrelatedSubQueryValue( ) )
            {
                criteria.IsDeferedCorrelation = true;

                if ( ! criteria.Name.IsNullOrEmpty( ) )
                {
                    AddColumn( criteria , count );
                }
            }

            Tokens.SelectArguments.Add( name );
        }

        private bool IsDistinct( Criteria criteria )
        {
            if ( EnforceDistinct )
            {
               return !arguments.Contains( criteria.Name );
            }

            return true;
        }

        internal override string GetAlias( Criteria criteria , string column )
        {
            if ( EnforceDistinct )
            {
                return base.GetAlias( criteria , column );
            }

            criteria.Alias = criteria.Alias ?? string.Join(   "" , 
                                            new[] 
                                            { 
                                                OuterMostAlias , 
                                                ( aliasCount++ ).ToString( ) 
                                            } 
                                         );

            return NamingStrategy.DecorateName( criteria.Alias  );
        }

        private void AddColumn( Criteria criteria , int count )
        {
            if ( IsDistinct( criteria ) )
            {
                string aliasedColumn = GetAliasedColumn( criteria );
                arguments.Add( criteria.Name );
                {
                    AddToken( SetSelectASQueryField( criteria , aliasedColumn ) );
                    Tokens.SelectArguments.Add( criteria.Name );
                }
            }
        }
    }
}
