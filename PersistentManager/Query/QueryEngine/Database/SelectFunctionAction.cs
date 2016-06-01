using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Query.Sql;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;

namespace PersistentManager.Query.QueryEngine.Database
{
    class SelectFunctionAction : ActionBase , IQueryBuilderStrategy
    {
        internal override bool CanExecute
        {
            get
            {
                return ( Criterias.Count > 0 || ( Syntax.FunctionType != ProjectionFunction.NOTSET && Syntax.FunctionType != ProjectionFunction.Last ) );
            }
        }

        internal SelectFunctionAction( ) : base( QueryPart.FUNCTION ) { }

        public void Execute( )
        {
            ProjectionFunction functionType = Syntax.FunctionType;

            if ( functionType == ProjectionFunction.COUNT )
            {
                Criteria first = Criterias.FirstOrDefault( );
                object value = first.IsNull( ) ? 1 : first.Value;
                Tokens.ClearTokens( QueryPart.SELECT );
                Tokens.AddFormatted( QueryPart.FUNCTION , " {0} " , Dialect.PrepareCount( value.ToString( ) ) );
            }
            else if ( functionType == ProjectionFunction.TOP || functionType == ProjectionFunction.Last )
            {
                Criteria first = Criterias.FirstOrDefault( );
                int index = first.Value.ToInt( );
                Tokens = CurrentProvider.GetFilteredQuerySyntax( Tokens , index );
            }
        }
    }
}
