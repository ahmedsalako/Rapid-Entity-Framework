using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;
using PersistentManager.Cache;

namespace PersistentManager.Ghosting.Event
{
    public class PropertyChangedEmiter
    {
        public static void Emit( TypeBuilder GhostClass , Type reflectedType )
        {
            CreateEvent( GhostClass , reflectedType );
        }

        private static void CreateEvent( TypeBuilder GhostClass , Type reflectedType )
        {
            FieldBuilder propertyChangedEvent = GhostClass.DefineField( "PropertyChanged" , typeof( PropertyChangingEventHandler ) , FieldAttributes.Public | FieldAttributes.HasDefault );
            ConstructorInfo CreateEventArgs = typeof( PropertyChangedEventArgs ).GetConstructor( new Type[] { typeof( String ) } );

            CustomAttributeBuilder xmlIgnore = new CustomAttributeBuilder( typeof( XmlIgnoreAttribute ).GetConstructor( Type.EmptyTypes ) , new object[] { } );
            CustomAttributeBuilder debuggerBrowsable = new CustomAttributeBuilder( typeof( DebuggerBrowsableAttribute ).GetConstructor( new Type[] { typeof( DebuggerBrowsableState ) } ) , new object[] { DebuggerBrowsableState.Never } );

            propertyChangedEvent.SetCustomAttribute( xmlIgnore );
            propertyChangedEvent.SetCustomAttribute( debuggerBrowsable );

            MethodBuilder RaisePropertyChanged = EmitPropertyChangedRaiser( GhostClass , propertyChangedEvent , CreateEventArgs );
            MethodBuilder AddPropertyChanged = EmitAddPropertyChangedBody( GhostClass , propertyChangedEvent );
            MethodBuilder RemovePropertyChanged = EmitRemovePropertyChangedBody( GhostClass , propertyChangedEvent );

            EventBuilder raiseEvent = GhostClass.DefineEvent( "PropertyChanged" , EventAttributes.None , typeof( PropertyChangedEventHandler ) );
            raiseEvent.SetRaiseMethod( RaisePropertyChanged );
            raiseEvent.SetAddOnMethod( AddPropertyChanged );
            raiseEvent.SetRemoveOnMethod( RemovePropertyChanged );

            if (reflectedType.IsInterface)
            {
                EnsureInterfacePropertyChangedEvent(GhostClass, reflectedType, RaisePropertyChanged);
            }
            else
            {
                EnsurePropertyChangedEvent(GhostClass, reflectedType, RaisePropertyChanged);
            }
        }

        private static MethodAttributes GetMethodAttributes( )
        {
            return MethodAttributes.Public | MethodAttributes.Virtual |
                MethodAttributes.SpecialName | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot;
        }

        private static MethodBuilder EmitAddPropertyChangedBody( TypeBuilder GhostClass , FieldBuilder propertyChangedEvent )
        {
            MethodInfo DelegateCombine = typeof( Delegate ).GetMethod( "Combine" , new Type[] { typeof( Delegate ) , typeof( Delegate ) } );
            MethodBuilder AddPropertyChanged = GhostClass.DefineMethod( "add_PropertyChanged" , GetMethodAttributes( ) ,
            typeof( void ) , new Type[] { typeof( PropertyChangedEventHandler ) } );

            ILGenerator generator = AddPropertyChanged.GetILGenerator( );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldfld , propertyChangedEvent );
            generator.Emit( OpCodes.Ldarg_1 );
            generator.Emit( OpCodes.Call , DelegateCombine );
            generator.Emit( OpCodes.Castclass , typeof( PropertyChangedEventHandler ) );
            generator.Emit( OpCodes.Stfld , propertyChangedEvent );
            generator.Emit( OpCodes.Ret );

            return AddPropertyChanged;
        }

        private static MethodBuilder EmitRemovePropertyChangedBody( TypeBuilder GhostClass , FieldBuilder propertyChangedEvent )
        {
            MethodInfo DelegateRemove = typeof( Delegate ).GetMethod( "Remove" , new Type[] { typeof( Delegate ) , typeof( Delegate ) } );

            MethodBuilder RemovePropertyChanged = GhostClass.DefineMethod(
                "remove_PropertyChanged" , MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot ,
                typeof( void ) , new Type[] { typeof( PropertyChangedEventHandler ) } );

            ILGenerator generator = RemovePropertyChanged.GetILGenerator( );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldfld , propertyChangedEvent );
            generator.Emit( OpCodes.Ldarg_1 );
            generator.Emit( OpCodes.Call , DelegateRemove );
            generator.Emit( OpCodes.Castclass , typeof( PropertyChangedEventHandler ) );
            generator.Emit( OpCodes.Stfld , propertyChangedEvent );
            generator.Emit( OpCodes.Ret );

            return RemovePropertyChanged;
        }

        private static MethodBuilder EmitPropertyChangedRaiser( TypeBuilder GhostClass , FieldBuilder propertyChangedEvent , ConstructorInfo CreateEventArgs )
        {
            MethodInfo InvokeDelegate = typeof( PropertyChangedEventHandler ).GetMethod( "Invoke" );

            MethodBuilder RaisePropertyChanged = GhostClass.DefineMethod(
                 "OnPropertyChanged" , MethodAttributes.Public ,
                 typeof( void ) , new Type[] { typeof( String ) } );

            ILGenerator generator = RaisePropertyChanged.GetILGenerator( );
            Label isDelegateOk = generator.DefineLabel( );

            generator.DeclareLocal( typeof( PropertyChangedEventHandler ) );
            generator.Emit( OpCodes.Nop );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldfld , propertyChangedEvent );
            generator.Emit( OpCodes.Stloc_0 );
            generator.Emit( OpCodes.Ldloc_0 );
            generator.Emit( OpCodes.Ldnull );
            generator.Emit( OpCodes.Ceq );
            generator.Emit( OpCodes.Brtrue , isDelegateOk );
            generator.Emit( OpCodes.Ldloc_0 );
            generator.Emit( OpCodes.Ldarg_0 );
            generator.Emit( OpCodes.Ldarg_1 );
            generator.Emit( OpCodes.Newobj , CreateEventArgs );
            generator.Emit( OpCodes.Callvirt , InvokeDelegate );
            generator.MarkLabel( isDelegateOk );
            generator.Emit( OpCodes.Ret );

            return RaisePropertyChanged;
        }

        private static void EnsureInterfacePropertyChangedEvent( TypeBuilder GhostClass, Type reflectedType, MethodBuilder RaisePropertyChanged )
        {
            foreach ( PropertyInfo propertyInfo in  reflectedType.GetProperties() )
            {
                if ( !MetaDataManager.IsPersistent( propertyInfo ) )
                    continue;

                if (MetaDataManager.IsUniqueIdentifier(propertyInfo.Name, reflectedType))
                    continue;

                if ( !MetaDataManager.IsLazyLoadableProperty( propertyInfo ) )
                {
                    DefineInterfaceProperty(GhostClass, RaisePropertyChanged, propertyInfo);
                }
            }
        }

        private static void DefineInterfaceProperty(TypeBuilder GhostClass, MethodBuilder RaisePropertyChanged, PropertyInfo propertyInfo)
        {
            FieldBuilder field = GhostClass.DefineField( string.Concat("_", propertyInfo.Name ) , propertyInfo.PropertyType, FieldAttributes.Public);
            PropertyBuilder propertyBuilder = GhostClass.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyInfo.PropertyType, new[] { propertyInfo.PropertyType });
            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.Virtual;

            if (propertyInfo.CanWrite)
            {
                MethodBuilder setMethod = DefineInterfaceSetMethod( GhostClass , field , RaisePropertyChanged , propertyInfo , attributes );
                propertyBuilder.SetSetMethod(setMethod);
            }

            if (propertyInfo.CanRead)
            {
                MethodBuilder getMethod = DefineInterfaceGetMethod(GhostClass, field , propertyInfo, attributes);
                propertyBuilder.SetGetMethod(getMethod);
            }
        }

        private static void EnsurePropertyChangedEvent( TypeBuilder GhostClass , Type reflectedType , MethodBuilder RaisePropertyChanged )
        {
            foreach ( PropertyInfo propertyInfo in reflectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if ( !MetaDataManager.IsPersistent( propertyInfo ) )
                    continue;

                if ( MetaDataManager.IsUniqueIdentifier( propertyInfo.Name , reflectedType ) )
                    continue;

                if ( propertyInfo.GetSetMethod( ).IsVirtual && !MetaDataManager.IsLazyLoadableProperty( propertyInfo ) )
                {
                    MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.Virtual;

                    if ( propertyInfo.CanRead )
                    {
                        MethodBuilder getMethod = DefineGetMethod( GhostClass , propertyInfo , attributes );
                        GhostClass.DefineMethodOverride( getMethod , propertyInfo.GetGetMethod() );                        
                    }

                    if ( propertyInfo.CanWrite )
                    {
                        MethodBuilder setMethod = DefineSetMethod( GhostClass , RaisePropertyChanged , propertyInfo , attributes );
                        GhostClass.DefineMethodOverride(setMethod, propertyInfo.GetSetMethod());                        
                    }
                }
            }
        }

        private static MethodBuilder DefineInterfaceSetMethod(TypeBuilder GhostClass, FieldBuilder field , MethodBuilder RaisePropertyChanged, PropertyInfo propertyInfo, MethodAttributes attributes)
        {
            MethodBuilder setMethod = GhostClass.DefineMethod(propertyInfo.GetSetMethod().Name, attributes, null, new Type[] { propertyInfo.PropertyType });

            ILGenerator setIL = setMethod.GetILGenerator();
            setIL.DeclareLocal(typeof(object));
            setIL.DeclareLocal(typeof(object));
            Label isContainedInTCache = setIL.DefineLabel(); //Checks if object is in transactional cache
            setIL.Emit(OpCodes.Nop);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);


            if (propertyInfo.PropertyType.IsValueType)
                setIL.Emit(OpCodes.Box, propertyInfo.PropertyType);

            setIL.Emit(OpCodes.Stloc_1);

            setIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
            setIL.Emit(OpCodes.Call, RaisePropertyChanged);

            setIL.Emit(OpCodes.Nop);

            setIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Castclass, typeof(Object));
            setIL.Emit(OpCodes.Ldloc_1);
            setIL.Emit(OpCodes.Call, typeof(PersistentManager.Cache.RREChangeListeners).GetMethod("SetInstanceProperty"));
            setIL.Emit(OpCodes.Stloc_0);
            setIL.Emit(OpCodes.Ldloc_0);

            if (propertyInfo.PropertyType.IsValueType)
            {
                setIL.Emit(OpCodes.Brtrue, isContainedInTCache); //If its null

                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);

                setIL.Emit(OpCodes.Stfld, field);
                setIL.Emit(OpCodes.Ret);

                setIL.MarkLabel(isContainedInTCache);

                setIL.Emit(OpCodes.Ret);
            }
            else
            {
                setIL.Emit(OpCodes.Brtrue, isContainedInTCache); //If its null
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);

                setIL.Emit(OpCodes.Stfld, field);
                setIL.Emit(OpCodes.Ret);

                setIL.MarkLabel(isContainedInTCache);

                setIL.Emit(OpCodes.Ret);
            }
            return setMethod;
        }

        private static MethodBuilder DefineInterfaceGetMethod(TypeBuilder GhostClass, FieldBuilder field , PropertyInfo propertyInfo, MethodAttributes attributes)
        {
            MethodBuilder getMethod =
                GhostClass.DefineMethod(propertyInfo.GetGetMethod().Name, attributes, propertyInfo.PropertyType, Type.EmptyTypes);


            ILGenerator getIL = getMethod.GetILGenerator();
            getIL.DeclareLocal(typeof(object));
            Label isNotInContext = getIL.DefineLabel();
            getIL.Emit(OpCodes.Nop);

            getIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Castclass, typeof(Object));

            getIL.Emit(OpCodes.Call, typeof(PersistentManager.Cache.RREChangeListeners).GetMethod("GetInstanceProperty"));
            getIL.Emit(OpCodes.Stloc_0);
            getIL.Emit(OpCodes.Ldloc_0);

            if (propertyInfo.PropertyType.IsValueType)
            {
                getIL.Emit(OpCodes.Brtrue, isNotInContext);

                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, field);
                getIL.Emit(OpCodes.Ret);

                getIL.MarkLabel(isNotInContext);

                getIL.Emit(OpCodes.Ldloc_0);
                getIL.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                getIL.Emit(OpCodes.Ret);
            }

            else
            {
                getIL.Emit(OpCodes.Brtrue, isNotInContext); //If its null

                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, field);
                getIL.Emit(OpCodes.Ret);

                getIL.MarkLabel(isNotInContext);

                getIL.Emit(OpCodes.Ldloc_0);
                getIL.Emit(OpCodes.Ret);
            }
            return getMethod;
        }

        private static MethodBuilder DefineSetMethod( TypeBuilder GhostClass , MethodBuilder RaisePropertyChanged , PropertyInfo propertyInfo , MethodAttributes attributes )
        {
            MethodBuilder setMethod = GhostClass.DefineMethod( propertyInfo.GetSetMethod( ).Name , attributes , null , new Type[] { propertyInfo.PropertyType } );

            ILGenerator setIL = setMethod.GetILGenerator( );
            setIL.DeclareLocal( typeof( object ) );
            setIL.DeclareLocal( typeof( object ) );
            Label isContainedInTCache = setIL.DefineLabel( ); //Checks if object is in transactional cache
            setIL.Emit( OpCodes.Nop );
            setIL.Emit( OpCodes.Ldarg_0 );
            setIL.Emit( OpCodes.Ldarg_1 );


            if ( propertyInfo.PropertyType.IsValueType )
                setIL.Emit( OpCodes.Box , propertyInfo.PropertyType );

            setIL.Emit( OpCodes.Stloc_1 );

            setIL.Emit( OpCodes.Ldstr , propertyInfo.Name );
            setIL.Emit( OpCodes.Call , RaisePropertyChanged );

            setIL.Emit( OpCodes.Nop );

            setIL.Emit( OpCodes.Ldstr , propertyInfo.Name );
            setIL.Emit( OpCodes.Ldarg_0 );
            setIL.Emit( OpCodes.Castclass , typeof( Object ) );
            setIL.Emit( OpCodes.Ldloc_1 );
            setIL.Emit( OpCodes.Call , typeof( RREChangeListeners ).GetMethod( "SetInstanceProperty" ) );
            setIL.Emit( OpCodes.Stloc_0 );
            setIL.Emit( OpCodes.Ldloc_0 );

            if ( propertyInfo.PropertyType.IsValueType )
            {
                setIL.Emit( OpCodes.Brtrue , isContainedInTCache ); //If its null

                setIL.Emit( OpCodes.Ldarg_0 );
                setIL.Emit( OpCodes.Ldarg_1 );

                setIL.Emit( OpCodes.Call , propertyInfo.GetSetMethod( ) );
                setIL.Emit( OpCodes.Ret );

                setIL.MarkLabel( isContainedInTCache );

                setIL.Emit( OpCodes.Ret );
            }
            else
            {
                setIL.Emit( OpCodes.Brtrue , isContainedInTCache ); //If its null
                setIL.Emit( OpCodes.Ldarg_0 );
                setIL.Emit( OpCodes.Ldarg_1 );

                setIL.Emit( OpCodes.Call , propertyInfo.GetSetMethod( ) );
                setIL.Emit( OpCodes.Ret );

                setIL.MarkLabel( isContainedInTCache );

                setIL.Emit( OpCodes.Ret );
            }
            return setMethod;
        }

        private static MethodBuilder DefineGetMethod( TypeBuilder GhostClass , PropertyInfo propertyInfo , MethodAttributes attributes )
        {
            MethodBuilder getMethod =
                GhostClass.DefineMethod( propertyInfo.GetGetMethod( ).Name , attributes , propertyInfo.PropertyType , Type.EmptyTypes );


            ILGenerator getIL = getMethod.GetILGenerator( );
            getIL.DeclareLocal( typeof( object ) );
            Label isNotInContext = getIL.DefineLabel( );
            getIL.Emit( OpCodes.Nop );

            getIL.Emit( OpCodes.Ldstr , propertyInfo.Name );
            getIL.Emit( OpCodes.Ldarg_0 );
            getIL.Emit( OpCodes.Castclass , typeof( Object ) );

            getIL.Emit( OpCodes.Call , typeof( RREChangeListeners ).GetMethod( "GetInstanceProperty" ) );
            getIL.Emit( OpCodes.Stloc_0 );
            getIL.Emit( OpCodes.Ldloc_0 );

            if ( propertyInfo.PropertyType.IsValueType )
            {
                getIL.Emit( OpCodes.Brtrue , isNotInContext );

                getIL.Emit( OpCodes.Ldarg_0 );
                getIL.Emit( OpCodes.Call , propertyInfo.GetGetMethod( ) );
                getIL.Emit( OpCodes.Ret );

                getIL.MarkLabel( isNotInContext );

                getIL.Emit( OpCodes.Ldloc_0 );
                getIL.Emit( OpCodes.Unbox_Any , propertyInfo.PropertyType );
                getIL.Emit( OpCodes.Ret );
            }

            else
            {
                getIL.Emit( OpCodes.Brtrue , isNotInContext ); //If its null

                getIL.Emit( OpCodes.Ldarg_0 );
                getIL.Emit( OpCodes.Call , propertyInfo.GetGetMethod( ) );
                getIL.Emit( OpCodes.Ret );

                getIL.MarkLabel( isNotInContext );

                getIL.Emit( OpCodes.Ldloc_0 );
                getIL.Emit( OpCodes.Ret );
            }
            return getMethod;
        }

        private static MethodBuilder BuildPropertyChangedEventAdd( TypeBuilder GhostClass , string eventName , FieldInfo eventField )
        {
            MethodBuilder method = GhostClass.DefineMethod( "add_" + eventName ,
                  MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final
                | MethodAttributes.HideBySig | MethodAttributes.NewSlot );

            method.SetReturnType( typeof( void ) );
            method.SetParameters( typeof( PropertyChanged ) );

            ParameterBuilder value = method.DefineParameter( 1 , ParameterAttributes.None , "value" );
            ILGenerator addGen = method.GetILGenerator( );

            addGen.Emit( OpCodes.Ldarg_0 );
            addGen.Emit( OpCodes.Ldarg_0 );
            addGen.Emit( OpCodes.Ldfld , eventField );
            addGen.Emit( OpCodes.Ldarg_1 );
            addGen.Emit( OpCodes.Call , typeof( Delegate ).GetMethod( "Combine" , new Type[] { typeof( Delegate ) , typeof( Delegate ) } ) );
            addGen.Emit( OpCodes.Castclass , typeof( PropertyChanged ) );
            addGen.Emit( OpCodes.Stfld , eventField );
            addGen.Emit( OpCodes.Ret );

            return method;
        }

        private static MethodBuilder BuildPropertyChangedRemove( TypeBuilder GhostClass , string eventName , FieldInfo eventField )
        {
            MethodBuilder removeMethod = GhostClass.DefineMethod( "remove_" + eventName ,
                  MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final
                | MethodAttributes.HideBySig | MethodAttributes.NewSlot );


            removeMethod.SetReturnType( typeof( void ) );
            removeMethod.SetParameters(
                typeof( PropertyChanged )
                );

            ParameterBuilder value = removeMethod.DefineParameter( 1 , ParameterAttributes.None , "value" );
            ILGenerator removeGen = removeMethod.GetILGenerator( );

            removeGen.Emit( OpCodes.Ldarg_0 );
            removeGen.Emit( OpCodes.Ldarg_0 );
            removeGen.Emit( OpCodes.Ldfld , eventField );
            removeGen.Emit( OpCodes.Ldarg_1 );
            removeGen.Emit( OpCodes.Call , typeof( Delegate ).GetMethod( "Remove" , new Type[] { typeof( Delegate ) , typeof( Delegate ) } ) );
            removeGen.Emit( OpCodes.Castclass , typeof( PropertyChanged ) );
            removeGen.Emit( OpCodes.Stfld , eventField );
            removeGen.Emit( OpCodes.Ret );
            return removeMethod;
        }

    }
}
