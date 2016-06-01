using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Provider;
using PersistentManager.Provider.Functions;
using PersistentManager.Util;

namespace PersistentManager.Query.QueryEngine.Database
{
    internal abstract class ActionBase
    {
        internal ProviderFunctions AvailableFunctions { get; set; }
        internal IDatabaseProvider CurrentProvider { get; set; }
        internal ContextData ContextData { get; set; }
        internal SQLTokenizer Tokens { get; set; }
        internal SyntaxContainer Syntax { get; set; }
        internal QueryContext QueryContext { get; set; }
        internal QueryPart QueryPart { get; set; }

        internal virtual IList<Criteria> Criterias
        {
            get { return Syntax.GetQueryByPart( QueryPart ); }
        }

        internal virtual bool CanExecute
        {
            get { return ( Criterias.IsNotNull( ) && Criterias.Count > 0 ) || Syntax.Parameters.ContainsKey( QueryPart ); }
        }

        protected ActionBase( QueryPart queryPart )
        {
            QueryPart = queryPart;
        }

        internal virtual ContextData Execute( ContextData ContextData , IDatabaseProvider provider )
        {
            this.ContextData = ContextData;
            this.Tokens = ContextData.SQLTokenizer;
            this.Syntax = ContextData.Syntax;
            this.QueryContext = ContextData.QueryContext;
            this.Tokens.QueryableEntityType = QueryContext.EntityType;
            this.AvailableFunctions = provider.GetProviderFunctions( );
            this.CurrentProvider = provider;

            if ( CanExecute )
            {
                ( ( IQueryBuilderStrategy )this ).Execute( );
            }

            return ContextData;
        }

        internal string GetAliasedColumn( Criteria criteria )
        {
            return SetQueryField( criteria.OwnerAlias , criteria.Name );
        }

        internal void AddToken( string token )
        {
            Tokens.Add( QueryPart , token );
        }

        internal string SetQueryField( string alias , string columnName )
        {
            return NamingStrategy.DecorateName( alias , columnName.EraseAlias( ) );
        }

        internal string SetQueryField( Criteria criteria , string alias , string columnName )
        {
            return SetFunctions( SetQueryField( alias , columnName ) , criteria.Functions );
        }

        internal string SetSelectASQueryField( Criteria criteria , string column )
        {
            return string.Format( " {0} AS {1} " , SetFunctions( criteria , column ) , GetAlias( criteria , column ) );
        }

        internal virtual string GetAlias( Criteria criteria , string column )
        {
            criteria.Alias = criteria.Name;

            return StringUtil.EraseAlias( column ).StripQuotes( );
        }

        internal string SetFunctions( Criteria criteria , string column )
        {
            return SetFunctions( column , criteria.Functions );
        }

        internal string CreateParameter( string name , object value )
        {
            return Tokens.CreateProviderParameter( CurrentProvider , name , value );
        }

        private string SetFunctions( string columnName , Queue<QueryFunction> functions )
        {
            if ( functions.IsNotNull( ) && functions.Count > 0 )
            {
                IList<int> indices = functions.GetIndices( ).ToList( );

                foreach ( int index in indices )
                {
                    QueryFunction function = functions.Dequeue( );

                    if ( function.Function != FunctionCall.None )
                    {
                        columnName = AvailableFunctions.GetFunctionLiteral( columnName , function.Value.IsNotNull( ) ? CreateParameter( "tet" , function.Value ) : GetRightOperand( function.CriteriaJoin ) , function.Function );
                    }
                }
            }

            return columnName;
        }

        private string GetRightOperand( Guid Guid )
        {
            if ( Guid != Guid.Empty )
            {
                Criteria criteria = Criterias.FirstOrDefault( c => c.Hash == Guid );
                return SetFunctions( criteria , GetAliasedColumn( criteria ) );
            }

            return string.Empty;
        }

        internal string GetAttachedAlias(string value)
        {
            string[] array = StringUtil.Split(value, ".");

            if (array.Length > 1 && Syntax.AllAliases.IsNotNull() )
            {
                if (Syntax.AllAliases.Contains(array[0]))
                {
                    return array[0];
                }
            }

            return string.Empty;
        }
    }
}
