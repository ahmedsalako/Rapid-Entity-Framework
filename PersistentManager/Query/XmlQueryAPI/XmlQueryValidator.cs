using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using PersistentManager.Exceptions;
using System.Xml.Serialization;
using PersistentManager.Query.Keywords;
using PersistentManager.Descriptors;
using PersistentManager.Runtime.APIs.Xml;

namespace PersistentManager.Query.XmlQueryAPI
{
    public class XmlQueryValidator 
    {
        static XDocument XSD { get; set; }

        static XmlSchemaSet Schema { get; set; }

        static XmlQueryValidator( )
        {
            Assembly assembly = Assembly.GetAssembly( typeof( XmlQueryValidator ) );
            Stream stream = assembly.GetManifestResourceStream( "PersistentManager.Query.XmlQueryAPI.RapidXmlSchema.xsd" );

            using ( StreamReader streamReader = new StreamReader( stream ) )
            {
                StringReader xsdStream = new StringReader( streamReader.ReadToEnd( ) );
                XSD = XDocument.Load( xsdStream );
            }

            XmlSchemaSet schemaSet = new XmlSchemaSet( );
            schemaSet.Add( null , XSD.CreateReader( ) );
            schemaSet.Compile( );

            Schema = schemaSet;
        }

        public static void ValidateProperly( XmlReader reader )
        {
            XmlReaderSettings settings = new XmlReaderSettings( );
            settings.Schemas = Schema;
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler( Settings_ValidationEventHandler );

            while ( reader.Read( ) ) ;
        }

        public static XDocument ExecuteQuery( XDocument xmlDocument )
        {            
            xmlDocument.Validate( Schema , Settings_ValidationEventHandler );

            XmlSerializer serializer = new XmlSerializer( typeof( Query ) );

            SyntaxContainer syntax = GenerateSyntax( (Query) serializer.Deserialize( xmlDocument.CreateReader() ) );
            IQueryResult results = syntax.SelectResult();


            return SerializeQueryResult( serializer , results );
        }

        public static XDocument SerializeQueryResult( XmlSerializer serializer , IQueryResult results )
        {
            Query query = new Query( );
            query.row = new RowSetType[results.Rows.Length];
            int count = 0;

            foreach ( var row in results.Rows )
            {
                int rowid = count++;
                RowSetType rowset = new RowSetType( );
                rowset.rowid = ( rowid ).ToString( );
                rowset.column = new ColumnSetType[row.ItemArray.Length];                

                foreach ( var index in row.ItemArray.GetIndices( ) )
                {
                    rowset.column[index] = new ColumnSetType( );
                    //rowset.column[index].Name = key
                    rowset.column[index].Value = ( row.ItemArray[index] ).ToString( );
                }

                query.row[rowid] = rowset;
            }

            MemoryStream buffer = new MemoryStream( );
            XmlTextWriter xmlTextWriter = new XmlTextWriter( buffer , Encoding.UTF8 );

            serializer.Serialize( xmlTextWriter , query );

            buffer = ( MemoryStream ) xmlTextWriter.BaseStream;

            return XDocument.Load( new StringReader( UTF8Encoder.ByteArrayToString( buffer.ToArray( ) ) ) );
        }

        public static SyntaxContainer GenerateSyntax( Query query )
        {
            Keyword keyword = new Keyword( );
            EntityMetadata classMetadata = EntityMetadata.GetMappingInfo( query.From.Typeof );
            keyword.Path = new PathExpressionFactory( classMetadata.Type , keyword );
            keyword.Path.Main.CanonicalAlias = query.From.As;

            AddWhereExpression( query , keyword );

            if( query.GroupBy.IsNotNull() )
            {
                AddExpression( query.GroupBy.Expression , keyword , QueryPart.GroupBy );
            }

            if ( query.OrderByAscending.IsNotNull( ) )
            {
                keyword.Path.ORDERBY = ORDERBY.ASC;
                AddExpression( query.OrderByAscending.Expression , keyword , QueryPart.ORDERBY );
            }

            if ( query.OrderbyDescending.IsNotNull( ) )
            {
                keyword.Path.ORDERBY = ORDERBY.DESC;
                AddExpression( query.OrderbyDescending.Expression , keyword , QueryPart.ORDERBY );
            }

            if ( query.Select.IsNotNull( ) )
            {
                AddExpression( query.Select.Expression , keyword , QueryPart.SELECT );
            }

            return keyword.GetSyntax( );
        }

        private static void AddExpression( ExpressionType[] expressions , Keyword keyword , QueryPart queryPart )
        {
            if ( expressions.IsNotNull( ) )
            {
                foreach ( ExpressionType expression in expressions )
                {
                    keyword.AddProjectionExpression( expression.Name , queryPart );
                }
            }
        }

        private static void AddWhereExpression( Query query , Keyword keyword )
        {
            if ( query.Where.IsNotNull( ) && query.Where.Expression.IsNotNull( ) )
            {
                foreach ( ExpressionType expression in query.Where.Expression )
                {
                    keyword.AddConditionExpression( expression.Name , QueryPart.WHERE , GetCondition( expression.Condition ) , expression.Value );
                }
            }
        }

        public static Condition GetCondition( ConditionalType condition )
        {
            switch ( condition )
            {
                case ConditionalType.Between :
                    return Condition.Between;
                case ConditionalType.Contains:
                    return Condition.Contains;
                case ConditionalType.EndsWith:
                    return Condition.EndsWith;
                case ConditionalType.Equals:
                    return Condition.Equals;
                case ConditionalType.GreaterThan:
                    return Condition.GreaterThan;
                case ConditionalType.GreaterThanEqualsTo:
                    return Condition.GreaterThanEqualsTo;
                case ConditionalType.IN:
                    return Condition.IN;
                case ConditionalType.Is:
                    return Condition.Is;
                case ConditionalType.IsNotNull:
                    return Condition.IsNotNull;
                case ConditionalType.IsNull:
                    return Condition.IsNull;
                case ConditionalType.LessThan:
                    return Condition.LessThan;
                case ConditionalType.LessThanEqualsTo:
                    return Condition.LessThanEqualsTo;
                case ConditionalType.NOT_IN:
                    return Condition.NOT_IN;
                case ConditionalType.NotContains:
                    return Condition.NotContains;
                case ConditionalType.NotEquals:
                    return Condition.NotEquals;
                case ConditionalType.StartsWith:
                    return Condition.StartsWith;
                default:
                    return Condition.NOTSET;
            }
        }

        public static void ExecuteQuery( string xml )
        {
            ExecuteQuery( XDocument.Load( new StringReader( xml ) ) );
        }

        internal static void Settings_ValidationEventHandler( object sender , ValidationEventArgs e )
        {
            Throwable.ThrowException( string.Format( "XML document does not conform to schema set {0} " , e.Message ) );
        }
    }
}
