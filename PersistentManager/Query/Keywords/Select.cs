using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query.Keywords;
using PersistentManager.Query.Sql;
using System.Data.Common;
using PersistentManager.Util;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using PersistentManager.Ghosting;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;

namespace PersistentManager.Query
{
    internal delegate void DeferedSelect( params object[] parameters );
    internal delegate void DeferedSelectAggregate( params Aggregate[] aggregates );

    internal class Select : Keyword
    {
        DeferedSelect deferedSelect; object[] parameters;
        DeferedSelectAggregate selectAgregate; Aggregate[] aggregates;

        internal Select()
        {

        }

        internal Select(params Aggregate[] aggregates )
        {
            selectAgregate = new DeferedSelectAggregate( AddParameters );
            this.aggregates = GetParameterNames( aggregates ).ToArray( );
        }

        internal Select( object parameter , params object[] projections )
        {
            deferedSelect = new DeferedSelect( AddParameters );

            if ( parameter.IsNotNull( ) && SyntaxContainer.IsCompilerGeneratedType( parameter.GetType( ) ) )
            {
                ResultIsCompilerGenerated = true;
                CompilerGeneratedResultType = parameter.GetType( );

                this.parameters = GetParameterNames( GetAnonymousParameters( parameter ).ToArray( ) , DeferedSelect );
            }
            else if ( parameter.IsNotNull( ) && parameter.GetType( ).IsClassOrInterface( ) && projections.IsNotNull( ) )
            {
                this.parameters = GetParameterNames( projections );
            }
            else if ( parameter.IsNotNull( ) && parameter.GetType( ).IsClassOrInterface( ) )
            {
                this.parameters = new object[] { GetParameterName( parameter ) };
            }
            else
            {
                this.parameters = new object[] { GetParameterName( parameter ) };
            }
        }

        internal Select( params object[] parameters )
        {
            deferedSelect = new DeferedSelect( AddParameters );
            this.parameters = GetParameterNames( parameters );
        }

        private static IEnumerable<object> GetAnonymousParameters( object parameter )
        {
            Type anonymous = parameter.GetType( );
            if ( ( anonymous.IsClass && anonymous != typeof( string ) ) )
            {
                foreach ( var arg in anonymous.GetProperties( ) )
                {
                    yield return arg.GetValue( parameter , null );
                }
            }
        }

        private void AddParameters( object[] parameters )
        {
            if ( parameters.IsNotNull( ) )
            {
                AddProjectionExpression( ArrayUtil.ConvertAll<string>( parameters ) , QueryPart.SELECT );
            }
        }

        private void AddParameters( Aggregate[] aggregates )
        {
            foreach ( var value in aggregates )
            {
                AddProjectionExpression( QueryPart.SELECT , value.Function , value.Name.ToString( ) );
            }
        }

        internal void ExecuteDefered( PathExpressionFactory Path  )
        {
            this.Path = Path;
            if ( deferedSelect.IsNotNull( ) )
                deferedSelect.Invoke( parameters );
            else if ( selectAgregate.IsNotNull( ) )
                selectAgregate.Invoke( aggregates );
        }
    }
}
