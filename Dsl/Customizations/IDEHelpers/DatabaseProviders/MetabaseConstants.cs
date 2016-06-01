using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace consist.RapidEntity.Customizations.IDEHelpers.DatabaseProviders
{
    public static class MetabaseConstants
    {
        public const string PRIMARY_KEY_TABLE_NAME = "PRIMARY_KEY_TABLE_NAME";
        public const string PRIMARY_KEY_COLUMN_NAME = "PRIMARY_KEY_COLUMN_NAME";
        public const string FOREIGN_KEY_TABLE_NAME = "FOREIGN_KEY_TABLE_NAME";
        public const string FOREIGN_KEY_COLUMN_NAME = "FOREIGN_KEY_COLUMN_NAME";
        public const string FOREIGN_KEY_NAME = "FOREIGN_NAME";
        public const string PRIMARY_KEY_NAME = "PRIMARY_NAME";
    }

    public static class OleDbMetabaseConstants
    {
        public const string PKTABLE_NAME = "PK_TABLE_NAME";
        public const string PKCOLUMN_NAME = "PK_COLUMN_NAME";
        public const string FKTABLE_NAME = "FK_TABLE_NAME";
        public const string FKCOLUMN_NAME = "FK_COLUMN_NAME";
        public const string FK_NAME = "FK_NAME";
        public const string PK_NAME = "PK_NAME";
        public const string COLUMN_NAME = "COLUMN_NAME";
    }

    public static class SqlServerMetabaseConstants
    {
        public const string PKTABLE_NAME = "PKTABLE_NAME";
        public const string PKCOLUMN_NAME = "PKCOLUMN_NAME";
        public const string FKTABLE_NAME = "FKTABLE_NAME";
        public const string FKCOLUMN_NAME = "FKCOLUMN_NAME";
        public const string FK_NAME = "FK_NAME";
        public const string PK_NAME = "PK_NAME";
        public const string COLUMN_NAME = "COLUMN_NAME";
    }

    public enum MetabaseType : int
    {
        OleDbMetaProvider,
        SqlServerMetaProvider,
        OracleMetaProvider,
        MySqlMetaProvider,
        DB2Provider,
        SQLLite3Provider,
    }
}
