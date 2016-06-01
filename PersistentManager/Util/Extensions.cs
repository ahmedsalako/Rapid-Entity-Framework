using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentManager.Metadata;
using System.Collections.Specialized;
using PersistentManager.Util;
using PersistentManager.Query;
using PersistentManager.Descriptors;
using System.Collections;
using System.Runtime.CompilerServices;

namespace PersistentManager
{
    internal static class Extensions
    {
        internal static T As<T>(this object current)
        {
            return (T) current;
        }

        internal static object New(this object current)
        {
            return new object();
        }

        internal static TCast Cast<TCast>( this object value )
        {
            return (TCast) value;
        }

        internal static bool IsDeferedExecution( this Type type )
        {
            if ( type == typeof( IDeferedExecution ) )
                return true;

            return type.GetInterface( "IDeferedExecution" ).IsNotNull( );
        }

        internal static bool IsCollection( this Type type )
        {
            if ( type == typeof( IEnumerable ) )
                return true;

            return type.GetInterface( "IEnumerable" ).IsNotNull( ) && type != typeof(string);
        }

        internal static bool Implements( this Type type , Type interfaze )
        {
            return type.GetInterface( interfaze.Name , true ).IsNotNull( );
        }

        internal static bool IsString( this object value )
        {
            if( value.IsNull() )
                return false;

            if ( value == typeof( string ) )
                return true;

            return ( value.GetType( ) == typeof( string ) );
        }

        internal static bool IsClassOrInterface( this Type type )
        {
            return ( type != typeof( string ) && ( type.IsClass || type.IsInterface ) );
        }

        internal static bool IsGroupingType( this Type type )
        {
            return type.Name.Contains( Constant.IGROUPING );
        }

        internal static string AddAs( this string instance )
        {            
            if ( instance.IsNull( ) ) return instance;
            if ( instance.HasAs( ) ) return instance;

            return string.Format( " {0} AS {1} " , instance , instance.EraseAlias( ) );
        }

        internal static bool HasAs( this string instance )
        {
            if ( instance.IsNull( ) ) return false;

            return instance.SpecializedContains( Constant.AS );
        }

        internal static bool Is(this string instance, string value)
        {
            return instance == value;
        }

        internal static string Dot( this string alias , string name )
        {
            return AliasBuilder.Build( alias , name );
        }

        internal static string ConcatWithUnderScore( this string first , string second )
        {
            return string.Format( "{0}_{1}" , first , second );
        }

        internal static IEnumerable<int> GetIndices( this IEnumerable enumerable )
        {
            return GetIndices( enumerable.OfType<object>().ToArray() );
        }

        internal static IEnumerable<int> GetIndices( this ArrayList enumerable )
        {
            return GetIndices( enumerable.OfType<object>().ToArray() );
        }

        internal static IEnumerable<int> GetIndices( this object[] array )
        {
            int index = 0;
            foreach ( object value in array )
                yield return index++;
        }

        internal static IEnumerable<int> GetIndices<T>( this T[] t )
        {
            int index = 0;
            foreach ( T value in t )
                yield return index++;
        }

        internal static bool IsPersistenceEntity( this Type type )
        {
            return MetaDataManager.IsPersistentable( type );
        }

        internal static bool IsCompilerGenerated( this Type type )
        {
            var compilerGenerateds = type.GetCustomAttributes( typeof( CompilerGeneratedAttribute ) , true );

            return ( compilerGenerateds != null && compilerGenerateds.Length > 0 );
        }

        internal static EntityMetadata GetMetataData( this Type entity )
        {
            return EntityMetadata.GetMappingInfo( entity );
        }

        internal static IEnumerable<string> Properties( this EntityMetadata metadata )
        {
            foreach ( var property in metadata.GetAllPersistentFieldIncludeBase( ) )
            {
                yield return property.ClassDefinationName;
            }
        }

        internal static IEnumerable<object> Values( this EntityMetadata metadata )
        {
            foreach ( var property in metadata.GetAllPersistentFieldIncludeBase( ) )
            {
                yield return property.FieldValue;
            }
        }

        internal static string[] Dot( this string alias , string[] names )
        {
            return AliasBuilder.Build2( alias , names );
        }

        internal static int ToInt(this object value)
        {
            if(value.IsNull())
                return 0;

            return int.Parse(value.ToString());
        }

        internal static bool SpecializedContains( this string text1 , string text2 )
        {
            if (text1.IsNull() && text2.IsNull())
                return false;

            if (text1.IsNull() || text2.IsNull())
                return false;

            return ( text1.ToLower( ).Trim( ).Contains( text2.ToLower( ).Trim( ) ) );
        }

        internal static string GetAliased( this string text1 )
        {
            if ( text1.Contains( Constant.AS ) )
            {
                text1 = text1.Split( new[] { Constant.AS } , StringSplitOptions.None )[1];
            }

            return text1;
        }

        internal static bool AreEquals( this string text1 , string text2 )
        {
            if (text1.IsNull() && text2.IsNull())
                return true;

            if (text1.IsNull() || text2.IsNull())
                return false;

            return ( text1.ToLower( ).Trim( ) == text2.ToLower( ).Trim( ) );
        }

        internal static string EraseAlias( this string value )
        {
            if ( value.IsNullOrEmpty( ) || !value.Contains(".") )
                return value;

            List<string> pathExpressions = new List<string>( StringUtil.Split( value , "." ) );

            if( pathExpressions.Count > 0 )
            {
                pathExpressions.RemoveAt( 0 );
            }

            return string.Join( "." , pathExpressions.ToArray() );
        }

        internal static string ElementsToString(this StringCollection collection)
        {
            return StringUtil.ToString(collection);
        }

        internal static string ElementsToString(this StringCollection collection, string separator)
        {
            return ArrayUtil.ToString(ArrayUtil.SeperateWith(separator, collection.OfType<string>().ToArray()));
        }

        internal static bool ElementsContains( this StringCollection collection , string text )
        {
            foreach ( string index in collection )
            {
                if ( index.GetAliased().SpecializedContains( text.GetAliased() ) )
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ToLowerEquals(this string thisString, string text)
        {
            return thisString.ToLower() == text.ToLower();
        }
    }
}
