using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

namespace PersistentManager.Ghosting
{
    delegate object CreateCtor( object[] args );

    internal static class ReflectionManager
    {
        public static ConstructorInfo GetMethodInfo2( Type type , ConstructorInfo ctor )
        {
            return type.GetConstructor( Type.EmptyTypes ); //new GenericConstructorInfo(type, ctor);
        }

        private static CreateCtor GenericCreate( )
        {
            DynamicMethod method = new DynamicMethod( "CreateIntance" , typeof( object ) , new Type[] { typeof( object[] ) } , Assembly.GetExecutingAssembly( ).ManifestModule , true );

            ILGenerator gen = method.GetILGenerator( );
            gen.Emit( OpCodes.Ldarg_0 );//arr
            gen.Emit( OpCodes.Ldc_I4_0 );
            gen.Emit( OpCodes.Ldelem_Ref );
            gen.Emit( OpCodes.Unbox_Any , typeof( Type ) );

            gen.Emit( OpCodes.Ldarg_0 );//arr
            gen.Emit( OpCodes.Ldc_I4_1 );
            gen.Emit( OpCodes.Ldelem_Ref );
            gen.Emit( OpCodes.Castclass , typeof( ConstructorInfo ) );
            gen.Emit( OpCodes.Newobj , typeof( ReflectionManager ).GetMethod( "GetMethodInfo" ) );
            gen.Emit( OpCodes.Ret );

            return ( CreateCtor )method.CreateDelegate( typeof( CreateCtor ) );
        }

        public static T New<T>( this Type value , params object[] parameters )
        {
            return ( T )value.GetConstructor( Type.EmptyTypes ).Invoke( parameters );
        }

        public static object New( this Type type , params object[] parameters )
        {
            return type.GetConstructor( Type.EmptyTypes ).Invoke( parameters );
        }

        public static void SetPropertyValue( this Type type , object entity , object value , string name )
        {
            type.GetProperty( name ).GetSetMethod( ).Invoke( entity , new[] { value } );
        }

        public static object GetPropertyValue( this object entity , string name )
        {
            return entity.GetType( ).GetProperty( name ).GetGetMethod( ).Invoke( entity , null );
        }

        public static object Clone( this Type type , object entity , int depth )
        {
            object cloned = type.New( );

            foreach ( PropertyInfo property in type.GetProperties( ) )
            {
                if ( property.PropertyType.IsValueType || property.PropertyType == typeof( string ) )
                {
                    type.SetPropertyValue( cloned , property.GetGetMethod( ).Invoke( entity , null ) , property.Name );
                }
                else if ( property.PropertyType.GetInterface( "IEnumerable" ) != null && depth == 1 )
                {
                    Type listType = ConcreteCollectionDiscovery.GetConcreteImplementorWithGenericType( property.PropertyType );
                    object listInstance = listType.New( );
                    type.SetPropertyValue( cloned , listInstance , property.Name );

                    IEnumerator enumerator = ( ( IEnumerable )entity.GetPropertyValue( property.Name ) ).GetEnumerator( );

                    while ( enumerator.MoveNext( ) )
                    {
                        object current = enumerator.Current;
                        ConcreteCollectionDiscovery.AddElement( listInstance , current.GetType( ).BaseType.Clone( enumerator.Current , 2 ) );
                    }
                }
                else if ( property.PropertyType.IsClass && depth == 1 )
                {
                    object current = entity.GetPropertyValue( property.Name );

                    if ( current != null )
                    {
                        type.SetPropertyValue( cloned , current , property.Name );
                    }
                }
            }

            return cloned;
        }
    }
}
