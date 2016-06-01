using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Descriptors;
using PersistentManager.Query.Keywords;
using PersistentManager.Util;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal class ConditionAction : ActionBase , IQueryBuilderStrategy
    {
        internal ConditionAction( ) : base( QueryPart.CONDITIONS ) { }

        void IQueryBuilderStrategy.Execute( )
        {
            int count = 0;

            foreach ( Criteria criteria in Criterias )
            {
                if ( !criteria.Name.IsNullOrEmpty( ) && !criteria.HasCorrelatedSubQueryValue( ) )
                {
                    string alias = criteria.OwnerAlias;
                    string name = criteria.Name;

                    string columnName = GetColumnName( ContextData , criteria , ref alias , name );
                    {
                        //If the condition value is a referer property or a property of a joined entity
                        //There is no point creating an ADO.NET parameter with the @,?,: character
                        object value = criteria.Value ?? string.Empty;
                        string matchingAlias = GetAttachedAlias( value.ToString( ) );

                        string[] parameters = GetParameters( ref count , criteria , alias , columnName , matchingAlias );

                        parameters = criteria.FormatParameterNames( parameters );

                        if ( ( criteria.QueryPart == QueryPart.WHERE && !Tokens.HasWhereClause ) || !Tokens.HasWhereClause )
                        {
                            Tokens.Add( QueryPart.WHERE , AddCondition( QueryPart.WHERE , ContextData.QueryContext , Tokens , criteria , alias , columnName , parameters ) );
                            Tokens.HasWhereClause = true;
                        }
                        else if ( criteria.QueryPart == QueryPart.AND_NOT )
                        {
                            Tokens.AddFormatted( QueryPart.AND , " {0} ({1}) " , Dialect.AND_NOT , AddCondition( QueryPart.AND , ContextData.QueryContext , Tokens , criteria , alias , columnName , parameters ) );
                        }
                        else if ( criteria.QueryPart == QueryPart.OR )
                        {
                            Tokens.AddFormatted( QueryPart.OR , " {0} ({1}) " , Dialect.OR , AddCondition( QueryPart.AND , ContextData.QueryContext , Tokens , criteria , alias , columnName , parameters ) );
                        }
                        else
                        {
                            Tokens.AddFormatted( QueryPart.AND , " {0} ({1}) " , Dialect.AND , AddCondition( QueryPart.AND , ContextData.QueryContext , Tokens , criteria , alias , columnName , parameters ) );
                        }

                        CreateProviderParameterName( criteria , matchingAlias , parameters );
                    }
                }
                else if ( criteria.HasCorrelatedSubQueryValue( ) )
                {
                    AddCorrelation( criteria , Tokens.HasWhereClause ? QueryPart.AND : QueryPart.WHERE );
                    Tokens.HasWhereClause = true;
                }
            }
        }

        private void CreateProviderParameterName( Criteria criteria , string matchingAlias , string[] parameters )
        {
            if ( criteria.IsMultiValue )
            {
                foreach ( int index in criteria.MultiValue.GetIndices( ) )
                {
                    Tokens.CreateProviderParameter( CurrentProvider , parameters[index] , criteria.FormatParameterValue( criteria.MultiValue[index] ) );
                }
            }
            else if ( string.IsNullOrEmpty( matchingAlias ) && criteria.RequiresParameter( ) && ( parameters.IsNotNull() && parameters.Length > 0 ) )
            {
                Tokens.CreateProviderParameter( CurrentProvider , ArrayUtil.GetFirstElement( parameters ) , criteria.FormatParameterValue( criteria.Value ) );
            }
        }

        private void AddCorrelation( Criteria criteria , QueryPart part )
        {
            if ( criteria.HasCorrelatedSubQueryValue( ) )
            {
                SyntaxContainer subQuery = criteria.CorrelatedSubQuery;
                RDBMSDataStore dataStoreQuery = new RDBMSDataStore( new ContextData( subQuery , subQuery.QueryContext ) , CurrentProvider );

                string query = dataStoreQuery.ExecuteSelect( true ).SQLTokenizer.ToString( );
                int count = criteria.IsMultiValue ? criteria.MultiValue.Count : 1;
                string[] parameters = GetParameters( ref count , criteria , string.Empty , string.Empty , string.Empty );

                string right = string.Empty;
                string left = string.Empty;

                if ( Tokens.HasWhereClause )
                {
                    Tokens.AddFormatted( QueryPart.AND , " {0} " , Dialect.AND );
                }

                if ( criteria.Position == Operand.Left )
                {
                    left = "(" + query + ")";
                    Tokens.Add( part , AddCondition( Tokens , criteria , left , parameters ) );
                }
                else
                {
                    left = parameters[0];
                    right = "(" + query + ")";

                    Tokens.AddFormatted( part , " {0} {1} {2} " , left , criteria.GetConditionAsString( ) , right );
                }                

                Tokens.Parameters.AddRange( dataStoreQuery.ContextData.SQLTokenizer.Parameters );
                CreateProviderParameterName( criteria , string.Empty , parameters );
            }
        }

        private string GetColumnName( ContextData ContextData , Criteria criteria , ref string alias , string name )
        {            
            return Tokens.EraseAlias( criteria.Name.ToString( ) );
        }

        private string[] GetParameters( ref int count , Criteria criteria , string alias , string columnName , string matchingAlias )
        {
            List<string> parameters = new List<string>( );
            if ( criteria.IsMultiValue )
            {
                foreach ( int index in criteria.MultiValue.GetIndices() )
                {
                    parameters.Add( GetParameter( ref count , criteria.MultiValue[index] , alias , columnName , matchingAlias ) );
                }
            }
            else
            {
                parameters.Add( GetParameter( ref count , criteria.Value , alias , columnName , matchingAlias ) );
            }

            return parameters.ToArray( );
        }

        private string GetParameter( ref int count , object value , string alias , string columnName , string matchingAlias )
        {
            string parameter = !matchingAlias.IsNullOrEmpty( ) ? SetQueryField( matchingAlias , Tokens.EraseAlias( value.ToString( ) ) ) :
                SQLBuilderHelper.CreateNamedParameter( Tokens.QueryableEntityType , ContextData.QueryContext.QueryType ,
                string.Format( "{0}{1}_{2}" , columnName , alias , count++ ) );

            return parameter;
        }

        private string AddCondition( QueryPart part , QueryContext context , SQLTokenizer Tokens , Criteria criteria , string alias , string columnName , string[] parameters )
        {            
            if ( criteria.HasCorrelatedSubQueryValue( ) )
            {
                SyntaxContainer subQuery = criteria.CorrelatedSubQuery;
                RDBMSDataStore dataStoreQuery = new RDBMSDataStore( new ContextData( subQuery , subQuery.QueryContext ) , CurrentProvider );
                string query = dataStoreQuery.ExecuteSelect( true ).SQLTokenizer.ToString( );

                Tokens.Parameters.AddRange( dataStoreQuery.ContextData.SQLTokenizer.Parameters );

                return Tokens.Format( " {0} {1} {2} ", SetQueryField( criteria , alias , columnName ), criteria.GetConditionAsString(), "(" + query + ")");
            }

            return AddCondition( Tokens , criteria , SetQueryField( criteria , alias , columnName ) , parameters );
        }

        private string AddCondition( SQLTokenizer Tokens , Criteria criteria , string leftOperand , string[] parameters )
        {
            if ( criteria.Condition == Condition.NOT_IN || criteria.Condition == Condition.IN )
            {
                string values = StringUtil.ToString( parameters , Quote.None );
                return Tokens.Format( criteria.GetConditionAsString( ) , leftOperand , values );
            }
            else if ( criteria.Condition == Condition.Between )
            {
                return Tokens.Format( criteria.GetConditionAsString( ) , leftOperand , parameters[0] , parameters[1] );
            }
            else
            {
                return Tokens.Format( " {0} {1} {2} " , leftOperand , criteria.GetConditionAsString( ) , ArrayUtil.GetFirstElement( parameters ) );
            }
        }
    }
}
