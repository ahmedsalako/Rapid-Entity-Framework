using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using PersistentManager.Linq;
using PersistentManager.Query;
using PersistentManager.Query.Keywords;
using PersistentManager.Query.Sql;
using PersistentManager.Cache;
using PersistentManager.Ghosting;

namespace PersistentManager.Descriptors
{
    internal class QueryParameter<T> : IParameter<T>
    {
        private AS As { get; set; }

        internal QueryParameter( )
        {
            As = new From( typeof( T ) ).As( "t0" );
        }

        public IParameter<T> Add( object name , object value )
        {
            As.Where( name , value );
            return this;
        }

        public IEnumerable<T> OrderBy( params object[] names )
        {
            As.OrderBy( names );
            return Search( );
        }

        public IParameter<T> Add( string name , object value )
        {
            As.Where( name , value );
            return this;
        }

        public IParameter<T> Add( object name , Condition condition , object value )
        {
            As.Where( name , condition , value );
            return this;
        }

        //Method in limbo
        private IParameter<T> Add( Expression<Func<T , bool>> expression )
        {
            QueryTranslator translator = new QueryTranslator( typeof( T ) );
            //queryRecord.AddRange(translator.Translate<T>(expression));

            return this;
        }

        public IEnumerable<T> Search( )
        {
            QueryContext context = As.Select( ).ExecuteQuery( );

            while ( context.DataReader.Read( ) )
            {
                yield return ( T )SelectExpression.ReturnSingle
                                                        (
                                                                context.MetaStructure.Type ,
                                                                context.DataReader , false
                                                         );
            }
        }
    }
}
