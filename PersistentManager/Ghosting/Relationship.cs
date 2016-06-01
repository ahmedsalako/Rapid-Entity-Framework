using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using PersistentManager.Descriptors;
using System.Reflection;
using PersistentManager.Initializers.Interfaces;
using System.Collections;

namespace PersistentManager.Ghosting
{
    internal class Relationship
    {
        internal static void EmitRelationships( Type livingType , TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            if ( livingType.IsInterface )
            {
                EmitInterfaceRelationships( livingType , GhostClass , fieldBuilder );
            }
            else
            {
                EmitClassRelationships( livingType , GhostClass , fieldBuilder );
            }
        }

        private static void EmitInterfaceRelationships( Type livingType , TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            foreach ( EntityRelation relation in EntityRelation.DeriveRelations( livingType ) )
            {
                PropertyBuilder propertyBuilder = GhostClass.DefineProperty( relation.Property.Name ,
                                                    PropertyAttributes.HasDefault ,
                                                    relation.Property.PropertyType , new[] { relation.Property.PropertyType } );

                switch ( relation.RelationshipType )
                {
                    case RelationshipType.OneToMany:
                        CreateOneToManyMethodByInterface( relation.Property , GhostClass , livingType , propertyBuilder , fieldBuilder );
                        break;
                    case RelationshipType.OneToOne:
                        CreateOneToOneMethodByInterface( relation.Property , GhostClass , livingType , propertyBuilder , fieldBuilder );
                        break;
                    case RelationshipType.ManyToMany:
                        CreateOneToManyMethodByInterface( relation.Property , GhostClass , livingType , propertyBuilder , fieldBuilder );
                        break;
                    default:
                        break;
                }
            }
        }

        private static void EmitClassRelationships( Type livingType , TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            foreach ( EntityRelation relation in EntityRelation.DeriveRelations( livingType ) )
            {
                switch ( relation.RelationshipType )
                {
                    case RelationshipType.OneToMany:
                        CreateOneToManyMethod( relation.Property , GhostClass , fieldBuilder );
                        break;
                    case RelationshipType.OneToOne:
                        CreateOneToOneMethod( relation.Property , GhostClass , fieldBuilder );
                        break;
                    case RelationshipType.ManyToMany:
                        CreateManyToManyMethod( relation.Property , GhostClass , fieldBuilder , livingType );
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CreateManyToManyMethod( PropertyInfo propertyInfo , TypeBuilder GhostClass , FieldBuilder fieldBuilder , Type baseType )
        {
            Type returnType = propertyInfo.PropertyType; //ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor(propertyInfo.PropertyType);

            //Create a GetGetMethod that overrides the base class GetGetMethod
            MethodBuilder innerMethod =
                GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );

            MethodInfo setMethodInfo = GhostClass.BaseType.GetMethod( propertyInfo.GetSetMethod( ).Name , new Type[] { propertyInfo.PropertyType } );

            MethodBuilder lazyMethod = GhostClass.DefineMethod( GhostClass + "_FastLazy" , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );
            GhostClass.DefineMethodOverride( innerMethod , propertyInfo.GetGetMethod( ) );
            ILGenerator LazyMethodIL = lazyMethod.GetILGenerator( );

            LazyMethodIL.DeclareLocal( returnType ); //// Declare the "List" local
            LazyMethodIL.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local
            LazyMethodIL.DeclareLocal( typeof( int ) );
            Label label = LazyMethodIL.DefineLabel( );

            LazyMethodIL.Emit( OpCodes.Nop );
            LazyMethodIL.Emit( OpCodes.Ldarg_0 );
            LazyMethodIL.EmitCall( OpCodes.Call , baseType.GetProperty( propertyInfo.Name ).GetGetMethod( ) , Type.EmptyTypes );
            LazyMethodIL.Emit( OpCodes.Stloc_0 );
            LazyMethodIL.Emit( OpCodes.Ldloc_0 );
            LazyMethodIL.Emit( OpCodes.Castclass , returnType );

            //GetMethodIL.Emit(OpCodes.Ldarg_1);
            LazyMethodIL.Emit( OpCodes.Brtrue_S , label );
            LazyMethodIL.Emit( OpCodes.Ldarg_0 );

            ////Call the LivingType's Parent, Check if the collection is already filled up
            LazyMethodIL.Emit( OpCodes.Ldfld , fieldBuilder );
            LazyMethodIL.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            LazyMethodIL.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );
            LazyMethodIL.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            LazyMethodIL.Emit( OpCodes.Stloc_1 );
            // Load local "List"

            LazyMethodIL.Emit( OpCodes.Ldloc_1 );
            LazyMethodIL.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "GetAllType" ) ); // Call The Dictionary GetAllType (virtual)
            LazyMethodIL.Emit( OpCodes.Stloc_0 );
            LazyMethodIL.Emit( OpCodes.Ldloc_0 );
            LazyMethodIL.Emit( OpCodes.Castclass , returnType );

            LazyMethodIL.Emit( OpCodes.Ldarg_0 );
            LazyMethodIL.Emit( OpCodes.Ldloc_0 );
            LazyMethodIL.Emit( OpCodes.Call , setMethodInfo );

            LazyMethodIL.Emit( OpCodes.Ret );
            LazyMethodIL.MarkLabel( label );

            LazyMethodIL.Emit( OpCodes.Ldloc_0 );
            LazyMethodIL.Emit( OpCodes.Castclass , returnType );

            LazyMethodIL.Emit( OpCodes.Ret );

            ILGenerator GetMethodIL = innerMethod.GetILGenerator( );

            GetMethodIL.DeclareLocal( returnType ); //// Declare the "IFrameworkList" local
            GetMethodIL.DeclareLocal( typeof( int ) );

            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.EmitCall( OpCodes.Callvirt , lazyMethod , Type.EmptyTypes );
            GetMethodIL.Emit( OpCodes.Stloc_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Castclass , returnType );

            GetMethodIL.Emit( OpCodes.Ret );
        }

        private static void CreateOneToOneMethod( PropertyInfo propertyInfo , TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            //Create a GetGetMethod that overrides the base class GetGetMethod
            MethodBuilder innerMethod =
                GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );

            ILGenerator GetMethodIL = innerMethod.GetILGenerator( );

            GetMethodIL.DeclareLocal( propertyInfo.PropertyType ); //// Declare the reference type local
            GetMethodIL.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local

            GetMethodIL.Emit( OpCodes.Nop );
            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldfld , fieldBuilder );
            GetMethodIL.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );

            //if (propertyInfo.PropertyType.IsValueType)
            //    GetMethodIL.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

            GetMethodIL.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            GetMethodIL.Emit( OpCodes.Stloc_1 );
            // Load local "ILazy Loader handler"
            GetMethodIL.Emit( OpCodes.Ldloc_1 );
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "GetType" ) ); // Call The Dictionary GetAllType (virtual)
            GetMethodIL.Emit( OpCodes.Stloc_0 );

            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Call , propertyInfo.GetSetMethod( ) );

            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            // GetMethodIL.Emit( OpCodes.Castclass , propertyInfo.PropertyType );            

            //GetMethodIL.Emit( OpCodes.Ldloc_0 );

            // Return
            GetMethodIL.Emit( OpCodes.Ret );

            GhostClass.DefineMethodOverride( innerMethod , propertyInfo.GetGetMethod( ) );

            MethodBuilder setMethod = GhostClass.DefineMethod( propertyInfo.GetSetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new Type[] { propertyInfo.PropertyType } );
            ILGenerator setMethodGen = setMethod.GetILGenerator( );
            setMethodGen.DeclareLocal( propertyInfo.PropertyType ); //// Declare the reference type local
            setMethodGen.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local

            setMethodGen.Emit( OpCodes.Nop );
            setMethodGen.Emit( OpCodes.Ldarg_0 );
            setMethodGen.Emit( OpCodes.Ldfld , fieldBuilder );
            setMethodGen.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            setMethodGen.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );
            setMethodGen.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            setMethodGen.Emit( OpCodes.Stloc_1 );
            // Load local "ILazy Loader Handler"
            setMethodGen.Emit( OpCodes.Ldloc_1 );
            setMethodGen.Emit( OpCodes.Ldarg_1 );
            setMethodGen.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "PersistChildObject" ) );
            setMethodGen.Emit( OpCodes.Ret );

            GhostClass.DefineMethodOverride( setMethod , propertyInfo.GetSetMethod( ) );
        }

        private static void CreateOneToOneMethodByInterface( PropertyInfo propertyInfo , TypeBuilder GhostClass , Type interfaze , PropertyBuilder property , FieldBuilder fieldBuilder )
        {
            MethodBuilder innerMethod =
                GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );

            ILGenerator GetMethodIL = innerMethod.GetILGenerator( );
            GetMethodIL.DeclareLocal( propertyInfo.PropertyType ); //// Declare the reference type local
            GetMethodIL.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local
            GetMethodIL.Emit( OpCodes.Nop );
            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldfld , fieldBuilder );
            GetMethodIL.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );

            GetMethodIL.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );
            GetMethodIL.Emit( OpCodes.Stloc_1 );
            // Load local "ILazy Loader handler"
            GetMethodIL.Emit( OpCodes.Ldloc_1 );
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "GetType" ) ); // Call The Dictionary GetAllType (virtual)
            GetMethodIL.Emit( OpCodes.Stloc_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Castclass , propertyInfo.PropertyType );
            // Return
            GetMethodIL.Emit( OpCodes.Ret );

            property.SetGetMethod( innerMethod );


            MethodBuilder setMethod = GhostClass.DefineMethod( propertyInfo.GetSetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new Type[] { propertyInfo.PropertyType } );
            ILGenerator setMethodGen = setMethod.GetILGenerator( );
            setMethodGen.DeclareLocal( propertyInfo.PropertyType ); //// Declare the reference type local
            setMethodGen.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local


            setMethodGen.Emit( OpCodes.Nop );
            setMethodGen.Emit( OpCodes.Ldarg_0 );
            setMethodGen.Emit( OpCodes.Ldfld , fieldBuilder );
            setMethodGen.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            setMethodGen.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );
            setMethodGen.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            setMethodGen.Emit( OpCodes.Stloc_1 );
            // Load local "ILazy Loader Handler"
            setMethodGen.Emit( OpCodes.Ldloc_1 );
            setMethodGen.Emit( OpCodes.Ldarg_1 );
            setMethodGen.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "PersistChildObject" ) );
            setMethodGen.Emit( OpCodes.Ret );

            property.SetSetMethod( setMethod );
        }

        private static void CreateOneToManyMethodByInterface( PropertyInfo propertyInfo , TypeBuilder GhostClass , Type interfaze , PropertyBuilder property , FieldBuilder fieldBuilder )
        {
            //Type returnType = ConcreteCollectionDiscovery.GetConcreteFrameworkImplementor( propertyInfo.PropertyType );
            Type returnType = propertyInfo.PropertyType;

            MethodBuilder innerMethod = GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name ,
                                        MethodAttributes.Public | MethodAttributes.Virtual ,
                                        CallingConventions.HasThis , propertyInfo.PropertyType ,
                                        Type.EmptyTypes );

            MethodInfo setMethodInfo = interfaze.GetMethod( propertyInfo.GetSetMethod( ).Name , new Type[] { propertyInfo.PropertyType } );

            ILGenerator GetMethodIL = innerMethod.GetILGenerator( );
            GetMethodIL.DeclareLocal( returnType );
            GetMethodIL.DeclareLocal( typeof( ILazyLoader ) );
            GetMethodIL.Emit( OpCodes.Nop );
            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldfld , fieldBuilder );
            GetMethodIL.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );

            GetMethodIL.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );
            // Store in local "Delegate Executor"
            GetMethodIL.Emit( OpCodes.Stloc_1 );
            // Load local "List"
            GetMethodIL.Emit( OpCodes.Ldloc_1 );
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "GetAllType" ) ); // Call The Dictionary GetAllType (virtual)
            GetMethodIL.Emit( OpCodes.Stloc_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Castclass , returnType );

            GetMethodIL.Emit( OpCodes.Ret );

            property.SetGetMethod( innerMethod );

            //Set Method
            MethodBuilder setMethod = GhostClass.DefineMethod( propertyInfo.GetSetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new Type[] { propertyInfo.PropertyType } );
            ILGenerator setMethodGen = setMethod.GetILGenerator( );
            setMethodGen.DeclareLocal( propertyInfo.PropertyType ); //// Declare the reference type local
            setMethodGen.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "DelegateExecutor" local


            setMethodGen.Emit( OpCodes.Nop );
            setMethodGen.Emit( OpCodes.Ldarg_0 );
            setMethodGen.Emit( OpCodes.Ldfld , fieldBuilder );
            setMethodGen.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            setMethodGen.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );
            setMethodGen.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            setMethodGen.Emit( OpCodes.Stloc_1 );
            // Load local "ILazy Loader Handler"
            setMethodGen.Emit( OpCodes.Ldloc_1 );
            setMethodGen.Emit( OpCodes.Ldarg_1 );
            setMethodGen.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "PersistChildObject" ) );
            setMethodGen.Emit( OpCodes.Ret );

            property.SetSetMethod( setMethod );
        }

        private static void CreateOneToManyMethod( PropertyInfo propertyInfo , TypeBuilder GhostClass , FieldBuilder fieldBuilder )
        {
            Type returnType = propertyInfo.PropertyType;

            //Create a GetGetMethod that overrides the base class GetGetMethod
            MethodBuilder innerMethod =
                GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );

            MethodInfo setMethodInfo = GhostClass.BaseType.GetMethod( propertyInfo.GetSetMethod( ).Name , new Type[] { propertyInfo.PropertyType } );

            ILGenerator GetMethodIL = innerMethod.GetILGenerator( );

            GetMethodIL.DeclareLocal( returnType ); //// Declare the "List" local
            GetMethodIL.DeclareLocal( typeof( ILazyLoader ) ); //// Declare the "Interface ILazyLoader" local



            GetMethodIL.Emit( OpCodes.Nop );
            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldfld , fieldBuilder );
            GetMethodIL.Emit( OpCodes.Ldstr , propertyInfo.Name ); //Load the key, to fetch from the Dictionary object
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( IDictionary ).GetMethod( "get_Item" , new Type[] { typeof( Object ) } ) );

            //if (propertyInfo.PropertyType.IsValueType)
            //    GetMethodIL.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

            GetMethodIL.Emit( OpCodes.Castclass , typeof( ILazyLoader ) );

            // Store in local "Delegate Executor"
            GetMethodIL.Emit( OpCodes.Stloc_1 );
            // Load local "List"
            GetMethodIL.Emit( OpCodes.Ldloc_1 );
            GetMethodIL.Emit( OpCodes.Callvirt , typeof( ILazyLoader ).GetMethod( "GetAllType" ) ); // Call The Dictionary GetAllType (virtual)
            GetMethodIL.Emit( OpCodes.Stloc_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Castclass , returnType );

            GetMethodIL.Emit( OpCodes.Ldarg_0 );
            GetMethodIL.Emit( OpCodes.Ldloc_0 );
            GetMethodIL.Emit( OpCodes.Call , propertyInfo.GetSetMethod( ) );

            // Return
            GetMethodIL.Emit( OpCodes.Ret );

            GhostClass.DefineMethodOverride( innerMethod , propertyInfo.GetGetMethod( ) );
        }
    }
}
