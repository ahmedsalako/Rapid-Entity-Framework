using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PersistentManager.Query.Keywords;

namespace PersistentManager.Query.Projections.ReturnTypes
{
    internal class ReturnType
    {
        internal static PropertyInfo GetPropertyInfo( Type Type , string PropertyName )
        {
            return Type.GetProperty( PropertyName );
        }

        internal static Type GetPropertyType( Type Type , string PropertyName , bool StripGenerics )
        {
            if ( StripGenerics )
            {
                PropertyInfo propertyInfo = GetPropertyInfo( Type , PropertyName );

                if ( propertyInfo.PropertyType.IsGenericType )
                {
                    return propertyInfo.PropertyType.GetGenericArguments( )[0];
                }
                else
                {
                    return propertyInfo.PropertyType;
                }
            }

            return GetPropertyInfo( Type , PropertyName ).PropertyType;
        }

        internal static object Create( Type Type , IDictionary<string , object> NamedResults  )
        {
            if ( Type.IsCompilerGenerated( ) )
            {
                return CreateAnonymousInstance( Type , NamedResults );
            }

            return CreateKnownType( Type , NamedResults );
        }

        internal static object CreateAnonymousInstance( Type Type , IDictionary<string , object> NamedResults )
        {
            return MetaDataManager.MakeInstance( Type , MakeArguments( NamedResults ) );
        }

        internal static object[] MakeArguments( IDictionary<string , object> NamedResults )
        {
            return NamedResults.Values.ToArray( );
        }

        internal static object CreateKnownType( Type Type , IDictionary<string , object> NamedResults  )
        {
            object entity = MetaDataManager.MakeInstance( Type );

            foreach ( KeyValuePair<string , object> pair in NamedResults )
            {
                MetaDataManager.SetPropertyValue( pair.Key , entity , pair.Value );
            }

            return entity;
        }

        internal static SyntaxContainer SetReturnType( Type projection , Type EntityType , SyntaxContainer syntaxContainer )
        {
            if ( projection.IsGenericType && !projection.IsCompilerGenerated() )
            {
                if ( !projection.IsCollection( ) )
                {
                    projection = projection.GetGenericArguments( )[0];

                    if ( projection.Name == Constant.GenericIGrouping )
                    {
                        projection = projection.GetGenericArguments( )[0];
                    }
                }
            }

            if ( projection.IsCompilerGenerated( ) )
            {
                syntaxContainer.CompilerGeneratedResultType = projection;
                syntaxContainer.ResultIsCompilerGenerated = true;
            }
            else if ( projection.IsClassOrInterface( ) && projection != EntityType )
            {
                if ( projection.IsCollection( ) )
                {
                    syntaxContainer.ResultIsCollection = true;
                }
                else
                {
                    syntaxContainer.ResultIsEntityClass = true;
                }
            }

            syntaxContainer.ReturnType = projection;

            return syntaxContainer;
        }

        internal static object CompareAndCast( Type type , object value )
        {
            return null;
        }
    }
}
