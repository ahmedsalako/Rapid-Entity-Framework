using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PersistentManager;
using System.Configuration;
using PersistentManager.Query.Keywords;
using PersistentManager.Query;
using System.Data;
using Persistent.Entities;
using PersistentManager.Mapping;

namespace PersistentFrameworkTest
{
    static class Program
    {
        static ConfigurationFactory factory;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //For SQL Server
            //factory = ConfigurationFactory.GetInstance(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
            //factory.ProviderDialect = ProviderDialect.SqlProvider;

            //For Oracle DB
            //factory = ConfigurationFactory.GetInstance(ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString);
            //factory.ProviderDialect = ProviderDialect.OracleProvider;
            //factory.Prefetch(typeof(Customer), typeof(Product));

            //For Access DB
            factory = ConfigurationFactory.GetInstance(ConfigurationManager.ConnectionStrings["MSAccess"].ConnectionString);
            factory.TransactionCacheBoundary = TransactionCacheBoundary.CrossBoundary;
            factory.ProviderDialect = ProviderDialect.OleDbProvider;
            factory.CacheSettings( 10 , 100 );

            //Multiple Database test.
            //factory.RegisterSiblingConnection("Mysql2", ProviderDialect.MySQLProvider, ConfigurationManager.ConnectionStrings["MySql"].ConnectionString);
            //factory.RegisterSiblingConnection("Oracle2", ProviderDialect.OracleProvider, ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString);


            //For MySql DB
            //factory = ConfigurationFactory.GetInstance(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString);
            //factory.ProviderDialect = ProviderDialect.MySQLProvider;
            //factory.Prefetch(typeof(Customer), typeof(Product));

            factory.Compile("Persistent.Entities");

            XmlEntityMapping mapping = new XmlEntityMapping();
            mapping.Name = "Person";
            mapping.ClassName = "ClassName";
            mapping.Assemblyname = "System.Models.Person";

            mapping.Keys = new List<Key>{ new Key()
            {
                AllowNullValue = false ,
                AutoKey = true ,
                IsUnique = true ,
                Name = "PersonId" ,
                PropertyName = "PersonId"
            }}.ToArray();

            mapping.Fields = new Field[2]
            {
                new Field()
                { 
                    Name = "AskId" ,
                    PropertyName = "AskId"
                },
                new Field()
                { 
                    Name = "AskId2" ,
                    PropertyName = "AskId2"
                }
            };

            mapping.ManyToManys = new ManyToMany[2]
            {
                new ManyToMany{ PropertyName ="Many1" , OwnerColumn ="Yop" } ,
                new ManyToMany{ PropertyName ="Many1" , OwnerColumn ="Yop" }
            };

            System.Xml.XmlDocument doc = XmlMappingSerializer.SerializeEntity(mapping);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PersistentMDIParent());
        }
    }
}
