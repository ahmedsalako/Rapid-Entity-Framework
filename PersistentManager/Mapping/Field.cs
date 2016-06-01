using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace PersistentManager.Mapping
{
    [AttributeUsage( AttributeTargets.Property , AllowMultiple = false )]
    public class Field : Attribute
    {
        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlAttribute("exclude" , DataType = "string")]
        public string Exclude { get; set; }

        [XmlAttribute("allow-null")]
        public bool AllowNullValue { get; set; }

        [XmlAttribute("is-auto-key")]
        public virtual bool AutoKey { get; set; }

        [XmlAttribute("is-unique")]
        public virtual bool IsUnique { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("db-type")]
        public string DbDataType { get; set; }

        [XmlAttribute("length")]
        public string Length { get; set; }

        [XmlAttribute("class-property")]
        public string PropertyName { get; set; }

        [XmlAttribute("table-ref")]
        public string TableRef { get; set; }

        [XmlIgnore]
        internal PropertyInfo Property { get; set; }

        public Field( )
        {

        }

        public Field( string name , bool isUnique , bool allowNullValue )
        {
            Name = name;
            IsUnique = isUnique;
            AllowNullValue = allowNullValue;
        }

        public Field( string name )
        {
            Name = name;
        }

        internal bool IsEmbedded( )
        {
            return !IsNotEmbedded( );
        }

        internal bool OwnedByTable( string table )
        {
            return TableRef.AreEquals( table );
        }

        internal bool IsNotEmbedded( )
        {
            return TableRef.IsNullOrEmpty( );
        }
    }
}
