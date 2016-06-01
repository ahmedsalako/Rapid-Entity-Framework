using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Exceptions;

namespace PersistentManager.Query
{
    public sealed class Aggregate
    {
        private string name;

        private Aggregate( string name , FunctionCall function )
        {
            this.Function = function;
            this.name = name;
        }

        internal string Name
        {
            get { return name; }
            set { name = value; }
        }

        internal FunctionCall Function { get; set; }

        public static Aggregate Sum( string name )
        {
            return new Aggregate( name , FunctionCall.Sum );
        }

        private static void AddCriteria( object name , FunctionCall function )
        {
            This current = This.GetCurrentScopeObject( );

            if ( current.IsNotNull( ) )
            {
                current.Keyword.AddProjectionExpression( 
                                QueryPart.SELECT , 
                                function ,
                                current.Keyword.GetParameterName( name ) 
                            );

                return;
            }

            Throwable.ThrowException( "Aggregate functions must be contained in a RQL query ! " );
        }

        public static Aggregate Max( string name )
        {
            return new Aggregate( name , FunctionCall.Max );
        }

        public static Aggregate Min( string name )
        {
            return new Aggregate( name , FunctionCall.Min );
        }

        public static Aggregate AVG( string name )
        {
            return new Aggregate( name , FunctionCall.Avg );
        }        

        public static Aggregate Count( string name )
        {
            return new Aggregate( name , FunctionCall.Count );
        }        

        public static Aggregate Property( string name )
        {
            return new Aggregate( name , FunctionCall.None );
        }

        public static T Property<T>( object name )
        {
            AddCriteria( name , FunctionCall.None );

            return default( T );
        }

        public static T Min<T>( object name )
        {
            AddCriteria( name , FunctionCall.Min );

            return default( T );
        }

        public static T Max<T>( object name )
        {
            AddCriteria( name , FunctionCall.Max );

            return default( T );
        }

        public static T Sum<T>( object name )
        {
            AddCriteria( name , FunctionCall.Sum );

            return default( T );
        }

        public static T AVG<T>( object name )
        {
            AddCriteria( name , FunctionCall.Avg );

            return default( T );
        }

        public static T Count<T>( object name )
        {
            AddCriteria( name , FunctionCall.Count );

            return default( T );
        }

        internal static Aggregate GetAggregate( string name , FunctionCall call )
        {
            if ( call == FunctionCall.Count )
                return Aggregate.Count( name );
            else if( call == FunctionCall.Max )
                return Aggregate.Max( name );
            else if( call == FunctionCall.Min )
                return Aggregate.Min( name );

            return Aggregate.Property( name );
        }
    }
}
