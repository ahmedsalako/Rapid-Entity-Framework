using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using PersistentManager.Descriptors;
using PersistentManager.Initializers.Interfaces;
using System.Collections;

namespace PersistentManager.Ghosting
{
    internal class Constructor
    {
        internal static ConstructorInfo Emit( TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            //Create Default constructor
            ConstructorBuilder defaultCtor = GhostClass.DefineDefaultConstructor( MethodAttributes.Public );

            ConstructorBuilder customCtor = GhostClass.DefineConstructor( MethodAttributes.Public , CallingConventions.HasThis ,
                                                        new Type[] { typeof( IDictionary<string , ILazyLoader> ) } );

            ILGenerator ctorILGen = customCtor.GetILGenerator( );
            ctorILGen.DeclareLocal( typeof( ArrayList ) );
            ctorILGen.Emit( OpCodes.Ldarg_0 );
            ctorILGen.Emit( OpCodes.Ldarg_1 );
            ctorILGen.Emit( OpCodes.Stfld , fieldBuilder );
            ctorILGen.Emit( OpCodes.Ret );

            return customCtor;
        }

        internal static void EmitCloneMethod( TypeBuilder GhostClass , Type livingType )
        {
            if ( !livingType.IsInterface )
            {
                MethodBuilder cloneMethod = GhostClass.DefineMethod( "RapidClone" , MethodAttributes.Public , livingType , new[] { livingType } );
                ILGenerator generator = cloneMethod.GetILGenerator( );
                LocalBuilder lbf = generator.DeclareLocal( livingType );

                ConstructorInfo constructor = livingType.GetConstructor( Type.EmptyTypes );

                generator.Emit( OpCodes.Newobj , constructor );
                generator.Emit( OpCodes.Stloc_0 );

                foreach ( PropertyInfo property in livingType.GetProperties( ) )
                {
                    if ( ( property.PropertyType.IsValueType || property.PropertyType == typeof( string ) ) && ( property.CanWrite && property.CanRead ) )
                    {
                        generator.Emit( OpCodes.Ldloc_0 );
                        generator.Emit( OpCodes.Ldarg_0 );
                        generator.Emit( OpCodes.Callvirt , property.GetGetMethod( ) );
                        generator.Emit( OpCodes.Callvirt , property.GetSetMethod( ) );
                    }
                    else if ( property.PropertyType.IsClass && ( property.CanWrite && property.CanRead ) )
                    {

                    }
                }
                generator.Emit( OpCodes.Ldloc_0 );
                generator.Emit( OpCodes.Ret );
            }
        }

        internal static void EmitCreateInstance( TypeBuilder GhostClass , Type livingType , ConstructorInfo ctor )
        {
            MethodBuilder factoryMethod = GhostClass.DefineMethod( "GetInstance" , MethodAttributes.Static | MethodAttributes.Public , livingType , new Type[] { typeof( IDictionary<string , ILazyLoader> ) } );
            ILGenerator generator = factoryMethod.GetILGenerator( );

            //generator.Emit(OpCodes.Ldarg_0);
            generator.Emit( OpCodes.Ldarg_1 );
            generator.Emit( OpCodes.Newobj , ctor );
            generator.Emit( OpCodes.Ret );
        }
    }
}
