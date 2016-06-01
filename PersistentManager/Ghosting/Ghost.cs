using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using PersistentManager.Descriptors;
using PersistentManager.Initializers.Interfaces;
using System.Collections;
using PersistentManager.Mapping;
using PersistentManager.Util;
using PersistentManager.Ghosting.Event;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using System.Diagnostics;
using PersistentManager.Contracts;

namespace PersistentManager.Ghosting
{
    internal class Ghost
    {
        internal static void Create( Type livingType , bool canDispatchPropertyChanged )
        {
            TypeBuilder GhostClass = MakeGhost( livingType , canDispatchPropertyChanged );
            Create( GhostClass , livingType , canDispatchPropertyChanged );
            GhostClass.CreateType( );

            if ( canDispatchPropertyChanged )
                GhostAssemblyBuilder.AddGhost( livingType.Name , livingType );
            else
                GhostAssemblyBuilder.AddGhost( livingType.Name + Constant.GHOST_TRANSACTION_NAME , livingType );
        }

        private static TypeBuilder MakeGhost( Type livingType , bool canDispatchPropertyChanged )
        {
            if ( livingType.IsInterface )
            {
                if ( canDispatchPropertyChanged )
                {
                    return MakeGhost( livingType , typeof( Object ) , typeof( INotifyPropertyChanged ) , livingType , typeof(IGhostableProxy) );
                }
                else
                {
                    return MakeGhost( livingType , Constant.GHOST_TRANSACTION_NAME , typeof( Object ) , livingType , typeof( IGhostableProxy ) );
                }
            }
            else if ( livingType.IsClass )
            {
                //GhostClass.SetParent(livingType);
                if ( canDispatchPropertyChanged )
                {
                    return MakeGhost( livingType , livingType , typeof( INotifyPropertyChanged ) , typeof( IGhostableProxy ) );
                }
                else
                {
                    return MakeGhost( livingType , Constant.GHOST_TRANSACTION_NAME , livingType , typeof( IGhostableProxy ) );
                }
            }

            return null;
        }

        internal static Type CreateVirtualType( string tablename , string typename , Embedded[] keys , Embedded[] properties )
        {
            TypeBuilder GhostClass = MakeGhost( typename , typeof( Object ) , typeof( IVirtualGhost ) );

            ConstructorInfo entityAttribute = typeof( Mapping.Entity ).GetConstructor( new Type[1] { typeof( string ) } );

            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder( entityAttribute , new object[1] { tablename } );

            GhostClass.SetCustomAttribute( attributeBuilder );

            CreateVirtualProperties( GhostClass , keys , properties );

            Type type = GhostClass.CreateType( );

            GhostAssemblyBuilder.AddGhost( typename , type );

            return type;
        }

        private static TypeBuilder Create( TypeBuilder GhostClass , Type livingType , bool canDispatchPropertyChanged )
        {
            return Emit( GhostClass , livingType , canDispatchPropertyChanged );
        }

        private static TypeBuilder Emit( TypeBuilder GhostClass , Type livingType , bool canDispatchPropertyChanged )
        {
            FieldBuilder delegateListField = CreateField( GhostClass , "DelegateList" , typeof( IDictionary<string , ILazyLoader> ) , FieldAttributes.Public );
            FieldBuilder isLoadedField = CreateField( GhostClass , "IsLoaded" , typeof( bool ) , FieldAttributes.Public );

            //Change/Self Tracking
            FieldBuilder selfTracking = CreateField( GhostClass , Constant.SELF_TRACKING_LIST_NAME , typeof( IDictionary<string , IChangeCatalog> ) , FieldAttributes.Public );


            CustomAttributeBuilder xmlIgnore = new CustomAttributeBuilder( typeof( XmlIgnoreAttribute ).GetConstructor( Type.EmptyTypes ) , new object[] { } );
            CustomAttributeBuilder debuggerBrowsable = new CustomAttributeBuilder( typeof( DebuggerBrowsableAttribute ).GetConstructor( new Type[] { typeof( DebuggerBrowsableState ) } ) , new object[] { DebuggerBrowsableState.Never } );

            //Ignore this fields during xml serialization
            delegateListField.SetCustomAttribute( xmlIgnore );
            isLoadedField.SetCustomAttribute( xmlIgnore );
            selfTracking.SetCustomAttribute( xmlIgnore );

            delegateListField.SetCustomAttribute( debuggerBrowsable );
            isLoadedField.SetCustomAttribute( debuggerBrowsable );
            selfTracking.SetCustomAttribute( debuggerBrowsable );

            if ( canDispatchPropertyChanged )
            {
                InjectPropertyChanged( GhostClass , livingType );
            }

            ConstructorInfo ctor = Constructor.Emit( GhostClass , delegateListField );
            Constructor.EmitCloneMethod( GhostClass , livingType );
            Constructor.EmitCreateInstance( GhostClass , livingType , ctor );
            EmitKeyProperty( GhostClass , livingType );

            Relationship.EmitRelationships( livingType , GhostClass , delegateListField );

            return GhostClass;
        }

        private static TypeBuilder EmitKeyProperty( TypeBuilder GhostClass , Type reflectedType )
        {
            if ( reflectedType.IsInterface )
            {
                foreach ( PropertyInfo propertyInfo in reflectedType.GetProperties( ) )
                {
                    if ( MetaDataManager.IsUniqueIdentifier( propertyInfo.Name , reflectedType ) )
                    {

                        FieldBuilder field = GhostClass.DefineField( string.Concat( "_" , propertyInfo.Name ) , propertyInfo.PropertyType , FieldAttributes.Public );
                        PropertyBuilder propertyBuilder = GhostClass.DefineProperty( propertyInfo.Name , PropertyAttributes.HasDefault , propertyInfo.PropertyType , new[] { propertyInfo.PropertyType } );

                        if ( propertyInfo.CanWrite )
                        {
                            MethodBuilder setMethod = GhostClass.DefineMethod( "set_" + propertyInfo.Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new[] { propertyInfo.PropertyType } );
                            GenerateSetMethodBody( field , setMethod.GetILGenerator( ) );
                            propertyBuilder.SetSetMethod( setMethod );
                        }

                        if ( propertyInfo.CanRead )
                        {
                            MethodBuilder getMethod = GhostClass.DefineMethod( "get_" + propertyInfo.Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );
                            GenerateGetMethodBody( field , getMethod.GetILGenerator( ) );
                            propertyBuilder.SetGetMethod( getMethod );
                        }
                    }
                }
            }

            return GhostClass;
        }

        private static void InjectPropertyChanged( TypeBuilder GhostClass , Type reflectedType )
        {
            PropertyChangedEmiter.Emit( GhostClass , reflectedType );
        }

        private static void BindPropertyAttribute( PropertyInfo propertyInfo , PropertyBuilder propertyBuilder )
        {
            foreach ( object customAttribute in propertyInfo.GetCustomAttributes( false ) )
            {
                if ( customAttribute is Field )
                {
                    string name = customAttribute.GetType( ).Name;

                    ConstructorInfo defaultConstructor = customAttribute.GetType( ).GetConstructor( Type.EmptyTypes );

                    List<object> propertyValues = new List<object>( );
                    List<PropertyInfo> namedParameters = new List<PropertyInfo>( );

                    foreach ( PropertyInfo attributeProp in customAttribute.GetType( ).GetProperties( BindingFlags.Public | BindingFlags.Instance ) )
                    {
                        if ( attributeProp.CanWrite )
                        {
                            namedParameters.Add( attributeProp );
                            propertyValues.Add( MetaDataManager.GetPropertyValue( attributeProp.Name , customAttribute ) );
                        }
                    }


                    CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder( defaultConstructor , new object[0] , namedParameters.ToArray( ) , propertyValues.ToArray( ) );
                    propertyBuilder.SetCustomAttribute( customAttributeBuilder );
                }
            }
        }

        private static Type[] GetParameterTypes( ParameterInfo[] parameterInfos )
        {
            List<Type> parameters = new List<Type>( );

            foreach ( ParameterInfo parameterInfo in parameterInfos )
            {
                parameters.Add( parameterInfo.GetType( ) );
            }

            return parameters.ToArray( );
        }

        private static void CreateVirtualProperties( TypeBuilder GhostClass , Embedded[] keys , Embedded[] properties )
        {
            CreateVirtualProperties( GhostClass , keys , true );
            CreateVirtualProperties( GhostClass , properties , false );
        }

        private static void CreateInterfaceProxy( TypeBuilder GhostClass , Type type )
        {
            foreach ( PropertyInfo property in type.GetProperties( ) )
            {
                if ( type.IsInterface )
                {
                    FieldBuilder fieldBuilder = CreateField( GhostClass , property.Name + "_Field" , property.PropertyType , FieldAttributes.Public | FieldAttributes.HasDefault );
                    PropertyBuilder propertyBuilder = GhostClass.DefineProperty( property.Name , PropertyAttributes.HasDefault , property.PropertyType , new[] { property.PropertyType } );

                    MethodBuilder getMethod = GhostClass.DefineMethod( property.GetGetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , property.PropertyType , Type.EmptyTypes );
                    MethodBuilder setMethod = GhostClass.DefineMethod( property.GetSetMethod( ).Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new[] { property.PropertyType } );

                    GenerateGetMethodBody( fieldBuilder , getMethod.GetILGenerator( ) );
                    GenerateSetMethodBody( fieldBuilder , setMethod.GetILGenerator( ) );

                    propertyBuilder.SetGetMethod( getMethod );
                    propertyBuilder.SetSetMethod( setMethod );
                }
            }
        }

        private static void CreateVirtualProperties( TypeBuilder GhostClass , Embedded[] properties , bool asKeys )
        {
            foreach ( Embedded embedded in properties )
            {
                FieldBuilder fieldBuilder = CreateField( GhostClass , embedded.PropertyName + "_Field" , embedded.Type , FieldAttributes.Public | FieldAttributes.HasDefault );
                PropertyBuilder propertyBuilder = GhostClass.DefineProperty( embedded.PropertyName , PropertyAttributes.HasDefault , embedded.Type , new[] { embedded.Type } );

                MethodBuilder getMethod = GhostClass.DefineMethod( "get_" + embedded.PropertyName , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , embedded.Type , Type.EmptyTypes );
                MethodBuilder setMethod = GhostClass.DefineMethod( "set_" + embedded.PropertyName , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new[] { embedded.Type } );

                GenerateGetMethodBody( fieldBuilder , getMethod.GetILGenerator( ) );
                GenerateSetMethodBody( fieldBuilder , setMethod.GetILGenerator( ) );

                propertyBuilder.SetGetMethod( getMethod );
                propertyBuilder.SetSetMethod( setMethod );

                ConstructorInfo fieldAttribute = typeof( Field ).GetConstructor( new Type[3] { typeof( string ) , typeof( Boolean ) , typeof( Boolean ) } );

                CustomAttributeBuilder attributeBuilder =
                    new CustomAttributeBuilder( fieldAttribute , new object[3] { embedded.ColumnName , asKeys , false } );

                propertyBuilder.SetCustomAttribute( attributeBuilder );
            }
        }

        private static void GenerateSetMethodBody( FieldBuilder fieldBuilder , ILGenerator iLGenerator )
        {
            iLGenerator.Emit( OpCodes.Nop );
            iLGenerator.Emit( OpCodes.Ldarg_0 );
            iLGenerator.Emit( OpCodes.Ldarg_1 );
            iLGenerator.Emit( OpCodes.Stfld , fieldBuilder );
            iLGenerator.Emit( OpCodes.Ret );
        }

        private static void GenerateGetMethodBody( FieldBuilder fieldBuilder , ILGenerator iLGenerator )
        {
            iLGenerator.Emit( OpCodes.Nop );
            iLGenerator.Emit( OpCodes.Ldarg_0 );
            iLGenerator.Emit( OpCodes.Ldfld , fieldBuilder );
            iLGenerator.Emit( OpCodes.Ret );
        }

        private static FieldBuilder CreateField( TypeBuilder GhostClass , string name , Type fieldType , FieldAttributes fieldAttributes )
        {
            return GhostClass.DefineField( name , fieldType , fieldAttributes );
        }

        private static TypeBuilder MakeGhost( Type livingType , Type baseType , params Type[] interfaces )
        {
            return MakeGhost( livingType , string.Empty , baseType , interfaces );
        }

        private static TypeBuilder MakeGhost( Type livingType , string name , Type baseType , params Type[] interfaces )
        {
            return GhostAssemblyBuilder.ModuleBuilder.DefineType( livingType.Name + name , TypeAttributes.Class | TypeAttributes.Public , baseType , interfaces );
        }

        //private static TypeBuilder MakeGhost( Type livingType , string name , Type baseType , params Type[] interfaces )
        //{
        //    return MakeGhost( livingType + name , baseType , interfaces );
        //}

        private static TypeBuilder MakeGhost( string typename , Type baseType , params Type[] interfazes )
        {
            return GhostAssemblyBuilder.ModuleBuilder.DefineType( typename , TypeAttributes.Class | TypeAttributes.Public , baseType , interfazes );
        }
    }
}
