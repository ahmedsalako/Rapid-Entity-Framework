using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace consist.RapidEntity.Customizations.Extensions
{
    public static class DataTypeMapping
    {
        static IDictionary<Type, DbType> mappings = new Dictionary<Type, DbType>();

        static DataTypeMapping()
        {
            mappings.Add(typeof(String), DbType.String);
            mappings.Add(typeof(DateTime), DbType.DateTime);
            mappings.Add(typeof(DateTimeOffset), DbType.DateTimeOffset);
            mappings.Add(typeof(Object), DbType.Object);
            //mappings.Add(typeof(Date), DbType.Date);
            //mappings.Add(typeof(Time), DbType.Time);
            mappings.Add(typeof(bool), DbType.Boolean);
            mappings.Add(typeof(byte), DbType.Byte);
            mappings.Add(typeof(sbyte), DbType.SByte);
            mappings.Add(typeof(decimal), DbType.Decimal);
            mappings.Add(typeof(double), DbType.Double);
            mappings.Add(typeof(float), DbType.Single);
            mappings.Add(typeof(int), DbType.Int32);
            mappings.Add(typeof(uint), DbType.UInt32);
            mappings.Add(typeof(long), DbType.Int64);
            mappings.Add(typeof(ulong), DbType.UInt64);
            mappings.Add(typeof(short), DbType.Int16);
            mappings.Add(typeof(ushort), DbType.UInt16);
            mappings.Add(typeof(Guid), DbType.Guid);
            mappings.Add(typeof(byte[]), DbType.Binary);
            mappings.Add(typeof(Enum), DbType.Int32);           
        }

        public static DbType ConvertToSQLType( Type clrType )
        {
            if ( mappings.ContainsKey( clrType ) )
            {
                return (DbType)mappings[clrType];
            }

            return DbType.Object;
        }

        public static Type[] ConvertToCLRType(DbType dbType)
        {
            return (from value in mappings.Where(v => v.Value == dbType)
                   select value.Key).ToArray();
        }
    }
}
