using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using PersistentManager.Query.Keywords;
using PersistentManager.Query;
using PersistentManager.Metadata;
using System.Diagnostics;

namespace PersistentManager.Ghosting
{
    public class CallStack
    {
        internal const string VIRTUAL_ASSEMBLY_NAME = "internal.query.metadata";
        const string version = "1.0.0.0";

        static AssemblyName assemblyName;
        static ModuleBuilder moduleBuilder;

        static AssemblyBuilder assembly;
        static AppDomain curAppDomain;
        static IDictionary<string , Type> cache;
        static object locker = new object( );

        static CallStack( )
        {
            assemblyName = new AssemblyName( VIRTUAL_ASSEMBLY_NAME );
            assemblyName.Version = new Version( version );
            curAppDomain = Thread.GetDomain( );
            assembly = curAppDomain.DefineDynamicAssembly( assemblyName , AssemblyBuilderAccess.Run );
            moduleBuilder = assembly.DefineDynamicModule( VIRTUAL_ASSEMBLY_NAME );
            cache = new Dictionary<string , Type>( );
        }

        private static ModuleBuilder ModuleBuilder
        {
            get { return moduleBuilder; }
        }

        internal static void IncrementSessionCount( int count )
        {
            ScopeContext.SetData( Constant.QUERY_SESSION , count );
        }

        internal static int GetCurrentSessionCount( )
        {
            object count = ScopeContext.GetData<int>( Constant.QUERY_SESSION );

            return count.IsNotNull( ) ? ( int )count : 0;
        }

        internal static void ResetCurrentSessionCount( )
        {
            IncrementSessionCount( 0 );
        }

        
        internal static T CreateAlias<T>( )
        {
            return ( T )CreateAlias( typeof( T ) , string.Empty );
        }

        public static object CreateAlias1( string property , object entity )
        {
            PropertyInfo propertyInfo = entity.GetType( ).GetProperty( property );
            string identifier = GetIdentifier( entity ).Dot( property );
            
            return CreateAlias( propertyInfo.PropertyType , identifier ) ;
        }

        internal static object CreateAlias(  Type type , string identifier )
        {
            lock ( locker )
            {
                if ( !cache.ContainsKey( type.Name ) )
                {
                    TypeBuilder proxy = ModuleBuilder.DefineType(type.Name, TypeAttributes.Class | TypeAttributes.Public, typeof(Object), GetInterfaces(type));
                    FieldBuilder fieldBuilder = proxy.DefineField( "ALIAS" , typeof( string ) , FieldAttributes.Public );

                    proxy = Emit( proxy , type );

                    ConstructorInfo attribute = typeof( DebuggerDisplayAttribute ).GetConstructor( new Type[] { typeof( string ) } );
                    CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder( attribute , new object[] { "ALIAS" } );
                    proxy.SetCustomAttribute( attributeBuilder );

                    Type proxyType = proxy.CreateType( );
                    cache.Add( type.Name , proxyType );

                    return SetIdentifier( Activator.CreateInstance( proxyType ) , identifier );
                }
            }

            return SetIdentifier( Activator.CreateInstance( cache[type.Name] ) , identifier );
        }

        private static Type[] GetInterfaces(Type type)
        {
            return type.IsInterface ? new Type[] { type } : Type.EmptyTypes;
        }

        private static object SetIdentifier( object queryable , string identifier )
        {
            Type queryableType = queryable.GetType( );
            string ALIAS = string.Empty;
            if ( identifier.IsNullOrEmpty() )
            {
                ALIAS = "t";
                int count = GetCurrentSessionCount( );
                ALIAS = ALIAS + count;
                IncrementSessionCount( ++count );                
            }
            else
            {
                ALIAS = identifier;
            }

            queryableType.GetField( "ALIAS" ).SetValue( queryable , ALIAS );
            return queryable;
        }

        internal static string GetIdentifier( object queryable )
        {
            return ( string )queryable.GetType( ).GetField( "ALIAS" ).GetValue( queryable );
        }

        private static TypeBuilder Emit(TypeBuilder proxy, Type parent)
        {
            return parent.IsInterface ? EmitInterfaceProperties(proxy, parent) :
                                        EmitClassProperties(proxy, parent);
        }

        private static TypeBuilder EmitInterfaceProperties( TypeBuilder proxy , Type parent )
        {
            foreach ( PropertyInfo propertyInfo in parent.GetProperties() )
            {
                FieldBuilder field = proxy.DefineField(string.Concat("_", propertyInfo.Name), propertyInfo.PropertyType, FieldAttributes.Public);
                PropertyBuilder propertyBuilder = proxy.DefineProperty(propertyInfo.Name, PropertyAttributes.HasDefault, propertyInfo.PropertyType, new[] { propertyInfo.PropertyType });

                if (propertyInfo.CanWrite)
                {
                    MethodBuilder setMethod = proxy.DefineMethod("set_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, null, new[] { propertyInfo.PropertyType });
                    GenerateSetMethodBody(setMethod.GetILGenerator(), parent , propertyInfo , field );
                    propertyBuilder.SetSetMethod(setMethod);
                }

                if (propertyInfo.CanRead)
                {
                    MethodBuilder getMethod = proxy.DefineMethod("get_" + propertyInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis, propertyInfo.PropertyType, Type.EmptyTypes);
                    GenerateGetMethodBody(getMethod.GetILGenerator(), parent , propertyInfo , field );
                    propertyBuilder.SetGetMethod(getMethod);                
                }
            }

            return proxy;
        }

        private static TypeBuilder EmitClassProperties( TypeBuilder proxy , Type parent )
        {
            foreach ( PropertyInfo propertyInfo in parent.GetProperties( ))
            {
                if ( !MetaDataManager.IsPersistent( propertyInfo ) )
                    continue;

                if ( !propertyInfo.GetGetMethod().IsVirtual )
                    continue;

                if ( propertyInfo.CanWrite )
                {                    
                    MethodBuilder setMethod = proxy.DefineMethod( "set_" + propertyInfo.Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , null , new[] { propertyInfo.PropertyType } );
                    GenerateSetMethodBody( setMethod.GetILGenerator( ) , parent , propertyInfo , null );
                    proxy.DefineMethodOverride( setMethod , propertyInfo.GetSetMethod( ) );
                }

                if ( propertyInfo.CanRead )
                {
                    MethodBuilder getMethod = proxy.DefineMethod( "get_" + propertyInfo.Name , MethodAttributes.Public | MethodAttributes.Virtual , CallingConventions.HasThis , propertyInfo.PropertyType , Type.EmptyTypes );
                    GenerateGetMethodBody( getMethod.GetILGenerator( ) , parent , propertyInfo , null  );
                    proxy.DefineMethodOverride( getMethod , propertyInfo.GetGetMethod( ) );
                }
            }

            proxy.SetParent(parent);

            return proxy;
        }

        private static void GenerateSetMethodBody(ILGenerator iLGenerator, Type owner, PropertyInfo property , FieldBuilder field)
        {
            iLGenerator.DeclareLocal( typeof( object ) );
            iLGenerator.Emit( OpCodes.Nop );
            iLGenerator.Emit( OpCodes.Ldarg_0 );
            iLGenerator.Emit( OpCodes.Call , typeof( CallStack ).GetMethod( "CheckCall1Set" ) );

            iLGenerator.Emit( OpCodes.Stloc_0 );
            iLGenerator.Emit( OpCodes.Ldarg_0 );
            iLGenerator.Emit( OpCodes.Ldarg_1 );

            if (owner.IsInterface)
            {
                iLGenerator.Emit(OpCodes.Stfld, field );
            }
            else
            {
                iLGenerator.Emit(OpCodes.Call, property.GetSetMethod());
            }

            iLGenerator.Emit( OpCodes.Ret );
        }

        private static void GenerateGetMethodBody( ILGenerator iLGenerator , Type owner , PropertyInfo property , FieldBuilder field )
        {
            if ( property.PropertyType.IsClassOrInterface( ) && !property.PropertyType.IsCollection() )
            {                
                iLGenerator.Emit( OpCodes.Nop );
                iLGenerator.Emit( OpCodes.Ldstr , property.Name );
                iLGenerator.Emit( OpCodes.Ldarg_0 );
                iLGenerator.Emit( OpCodes.Castclass , typeof( Object ) );
                iLGenerator.Emit( OpCodes.Call , typeof( PersistentManager.Ghosting.CallStack ).GetMethod( "CreateAlias1") );
                iLGenerator.Emit( OpCodes.Ret );
            }
            else
            {
                iLGenerator.Emit( OpCodes.Nop );
                iLGenerator.Emit( OpCodes.Ldarg_0 );
                iLGenerator.Emit( OpCodes.Ldstr , property.Name );
                iLGenerator.Emit( OpCodes.Ldarg_0 );
                iLGenerator.Emit( OpCodes.Call , typeof( CallStack ).GetMethod( "StoreCall1" ) );
                if (owner.IsInterface)
                {
                    iLGenerator.Emit(OpCodes.Ldfld, field);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Call, property.GetGetMethod());
                }                
                iLGenerator.Emit( OpCodes.Ret );
            }
        }

        public static void CheckCall1Set( )
        {
            throw new Exception( "Queryable instance property cannot be set " );
        }

        public static void StoreCall1( string fieldName , object entity )
        {
            Guid scopeId = GetCurrentScopeId( );

            if ( scopeId != Guid.Empty )
            {
                Queue<string> currentCalls = ( Queue<string> )ScopeContext.GetData<Queue<string>>( scopeId.ToString( ) );

                if ( currentCalls.IsNull( ) )
                {
                    currentCalls = new Queue<string>( );
                    ScopeContext.SetData( scopeId.ToString( ) , currentCalls );
                }

                currentCalls.Enqueue( GetIdentifier( entity ) + "." + fieldName );
            }
        }

        private static Guid GetCurrentScopeId( )
        {
            return PathExpressionFactory.GetCurrentScopeId( );
        }

        internal static IEnumerable<string> GetAllParameters( object[] parameters )
        {
            return GetAllParameters( GetCurrentScopeId( ) , parameters );
        }

        internal static IEnumerable<string> GetAllParameters( Guid ScopeId , object[] parameters )
        {
            foreach ( object value in parameters )
            {
                yield return GetParameter( ScopeId , value );
            }
        }

        internal static string GetParameter( object parameter )
        {
            return GetParameter( GetCurrentScopeId( ) , parameter );
        }

        internal static string GetParameter( Guid ScopeId , object parameter )
        {
            if ( parameter.IsNotNull( ) && parameter.GetType( ).IsClassOrInterface( ) )
            {
                return GetIdentifier( parameter );
            }

            return GetParameters( ScopeId , new[] { parameter } ).FirstOrDefault( );
        }

        internal static IEnumerable<string> GetParameters( object[] arguments , List<IDeferedExecution> deferedCollection )
        {
            List<object> newArguments = new List<object>( );
            foreach ( var arg in arguments )
            {
                if ( arg.IsNotNull() && arg is IDeferedExecution )
                {
                    deferedCollection.Add( ( IDeferedExecution )arg );
                }
                else if ( arg.IsNotNull( ) && arg.GetType( ).IsClassOrInterface() )
                {
                    if ( arg.GetType( ).IsCompilerGenerated( ) )
                    {
                        foreach ( PropertyInfo property in arg.GetType( ).GetProperties( ) )
                        {
                            foreach ( string expression in GetParameters( new[] { property.GetValue( arg , null ) } , deferedCollection ) )
                            {
                                newArguments.Add( expression );
                            }
                        }
                    }
                    else
                    {
                        newArguments.Add( GetIdentifier( arg ) );
                    }
                }
                else
                {
                    newArguments.Add( arg );
                }
            }

            return GetParameters( GetCurrentScopeId( ) , newArguments.ToArray( ) );
        }

        internal static IEnumerable<string> GetParameters( object[] arguments , SyntaxContainer query )
        {
            return GetParameters( arguments , query.DeferedSelect );
        }

        private static IEnumerable<string> GetParameters( Guid currentScope , object[] arguments )
        {
            var callStack = ScopeContext.GetData<Queue<string>>( currentScope.ToString( ) );

            if ( arguments.IsNotNull( ) )
            {
                foreach ( var arg in arguments )
                {
                    if ( arg.IsNull( ) || arg.GetType( ) != typeof( string ) )
                    {
                        if ( callStack.IsNotNull( ) && callStack.Count > 0 )
                        {
                            yield return callStack.Dequeue( );
                        }
                    }
                    else
                    {
                        yield return arg.ToString( );
                    }
                }
            }
            else
            {
                throw new Exception( "Query Arguments cannot be null or Empty " );
            }
        }

        internal static string GetLastParameter( )
        {
            return GetLastParameter( GetCurrentScopeId( ) );
        }

        internal static string GetLastParameter( Guid ScopeId )
        {
            if ( ScopeId != Guid.Empty )
            {
                var args = ScopeContext.GetData<Queue<string>>( ScopeId.ToString( ) );
                return args.IsNotNull( ) && args.Count > 0 ? args.Dequeue( ) : null;
            }

            return null;
        }

        internal static string ReadLastParameter( Guid ScopeId )
        {
            if ( ScopeId != Guid.Empty )
            {
                var args = ScopeContext.GetData<Queue<string>>( ScopeId.ToString( ) );

                
                return args.IsNotNull( ) && args.Count > 0 ? args.Last( ) : null;
            }

            return null;
        }
    }
}
