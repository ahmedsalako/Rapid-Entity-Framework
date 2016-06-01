using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PersistentManager.Descriptors;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using PersistentManager.Exceptions.EntityManagerException;
using PersistentManager.Exceptions;
using System.Collections;

namespace PersistentManager.GAC
{
    internal static class GacLoader
    {
        const string DRIVER_EXCEPTION = "Could not load provider from internal GAC. {0}: ";
        static IDictionary<Guid , Assembly> _gac = new Dictionary<Guid , Assembly>( );
        static XDocument xmlDescriptor;

        static GacLoader( )
        {
            Assembly assembly = Assembly.GetExecutingAssembly( );
            Stream providersStream = assembly.GetManifestResourceStream( Constant.ProviderXMLEmbeddedResourceLocation );
            StringReader reader = new StringReader( ( ( TextReader )( new StreamReader( providersStream ) ) ).ReadToEnd( ) );

            xmlDescriptor = XDocument.Load( reader );
        }

        public static IDbConnection GetConnection( Guid providerId , string connectionString , bool isEmbedded )
        {
            Type connectionType = GetConnectionType( providerId , isEmbedded );

            IDbConnection connection = ( IDbConnection )Activator.CreateInstance( connectionType );
            connection.ConnectionString = connectionString.Trim( );

            return connection;
        }

        public static IDbCommand GetCommand( Guid providerId , bool isEmbedded )
        {
            Type commandType = GetCommandType( providerId , isEmbedded );

            return ( IDbCommand )Activator.CreateInstance( commandType );
        }

        public static IDbDataParameter GetParameter( Guid providerId , string name , object value , bool isEmbedded )
        {
            Type parameterType = GetParameterType( providerId , isEmbedded );

            return ( IDbDataParameter )Activator.CreateInstance( parameterType , name , value );
        }

        private static Type GetConnectionType( Guid providerId , bool isEmbedded )
        {
            ProviderDescriptor descriptor = LoadDescriptor( providerId );
            Assembly loadedAssembly = LoadAssembly( isEmbedded , descriptor );

            return loadedAssembly.GetType( descriptor.ConnectionQualifiedName , true );
        }

        private static Type GetCommandType( Guid providerId , bool isEmbedded )
        {
            ProviderDescriptor descriptor = LoadDescriptor( providerId );
            Assembly loadedAssembly = LoadAssembly( isEmbedded , descriptor );

            return loadedAssembly.GetType( descriptor.CommandQualifiedName , true );
        }

        private static Type GetParameterType( Guid providerId , bool isEmbedded )
        {
            ProviderDescriptor descriptor = LoadDescriptor( providerId );
            Assembly loadedAssembly = LoadAssembly( isEmbedded , descriptor );

            return loadedAssembly.GetType( descriptor.ParameterQualifiedName , true );
        }

        private static Assembly LoadAssembly( bool isEmbedded , ProviderDescriptor descriptor )
        {
            Assembly loadedAssembly;

            if( isEmbedded )
            {
                loadedAssembly = LoadFromEmbeddedResource( descriptor );
            }
            else
            {
                loadedAssembly = LoadFromDisk( descriptor );
            }
            return loadedAssembly;
        }

        private static Assembly LoadFromDisk( ProviderDescriptor descriptor )
        {
            if( _gac.ContainsKey( descriptor.UniqueId ) )
                return _gac[descriptor.UniqueId];

            Assembly assembly = Assembly.LoadFile( Path.Combine( Environment.CurrentDirectory , descriptor.AssemblyName ) );

            Throwable.ThrowOnTrue( assembly.IsNull( ) , string.Format( DRIVER_EXCEPTION , descriptor.AssemblyName ) );

            _gac.Add( descriptor.UniqueId , assembly );

            return assembly;
        }

        private static Assembly LoadFromEmbeddedResource( ProviderDescriptor descriptor )
        {
            if( _gac.ContainsKey( descriptor.UniqueId ) )
                return _gac[descriptor.UniqueId];

            Assembly executingAssembly = Assembly.GetExecutingAssembly( );
            byte[] bytes;

            using( Stream stream = executingAssembly.GetManifestResourceStream
                ( string.Format( "{0}.{1}" , Constant.GACLocation , descriptor.AssemblyName ) ) )
            {
                bytes = new byte[( int )stream.Length];
                stream.Read( bytes , 0 , bytes.Length );
            }

            Assembly providerAssembly = Assembly.Load( bytes );

            Throwable.ThrowOnTrue( providerAssembly.IsNull( ) , string.Format( DRIVER_EXCEPTION , descriptor.AssemblyName ) );

            _gac.Add( descriptor.UniqueId , providerAssembly );

            return providerAssembly;
        }

        private static ProviderDescriptor LoadDescriptor( Guid providerId )
        {
            return ParseXMLDescriptor( providerId );
        }

        private static ProviderDescriptor ParseXMLDescriptor( Guid providerId )
        {
            IEnumerable<ProviderDescriptor> descriptors = from provider in xmlDescriptor.Descendants( "Provider" )
                                                          where providerId.ToString( ) == ( string )provider.Element( "UniqueId" ).Value.Trim( ).ToLower( )
                                                          select new ProviderDescriptor
                                                          {
                                                              AssemblyName = ( string )provider.Element( "AssemblyName" ).Value ,
                                                              ConnectionQualifiedName = ( string )provider.Element( "ConnectionQualifiedName" ).Value ,
                                                              ProviderName = ( string )provider.Element( "Name" ).Value ,
                                                              Type = ( string )provider.Element( "Type" ).Value ,
                                                              UniqueId = providerId ,
                                                              CommandQualifiedName = ( string )provider.Element( "CommandQualifiedName" ).Value ,
                                                              ParameterQualifiedName = ( string )provider.Element( "ParameterQualifiedName" ).Value
                                                          };

            return descriptors.FirstOrDefault( );
        }
    }
}
