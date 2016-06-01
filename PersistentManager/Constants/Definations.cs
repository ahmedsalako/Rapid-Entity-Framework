using System;
using System.Collections.Generic;
using System.Text;
using PersistentManager.Query;

namespace PersistentManager
{
    internal enum TransactionState
    {
        NotKnown = 0,
        Committed,
        RolledBack,
    }

    internal enum QueryReturnType : int
    {
        None = 0,
        CompilerGenerated = 1,
        KnownClassType = 2,
        DataTable = 3,
    }

    public enum TransactionCacheBoundary
    {
        EntityManagerScope = 0 ,
        CrossBoundary = 1,  
    }

    internal enum Operand : int
    {
        None,
        Left,
        Right,
    }

    internal enum RapidServiceType
    {
        MetaDataService,
        ProviderService,
        QueryBuilderService,
        CachingService,
        EntityProxyService,
    }

    internal enum CompositeType
    {
        None,
        PlaceHolding,
        EntityClass,
        OneSideRelationship,
        GroupCondition,
    }

    internal enum QueryType : int
    {
        Select = 0,
        Insert = 1 ,
        Update = 3,
        Delete = 4,
        SelectAll = 5,
        AlterTable = 6,
        SelectByCriteria = 7,
        SelectAllInRange = 8,
        NOTSET = 9,
        SelectCount = 10,
        SelectFirst = 11,
        SELECT_MANY_TO_MANY = 12,
    }

    internal enum JoinType : int
    {
        NOTSET,
        LeftJoin,
        RightJoin,
        CrossJoin,
        InnerJoin,
    }

    internal enum LockStatus
    {
        Locked = 0x100,
        Released = 0x110,
    }

    internal enum DataTypeEnum : int
    {
        Integer = 0,
        String = 1,
        Image = 2,
        Float = 3,
        DateTime = 4,
    }

    internal enum ParameterType : int
    {
        Input = 0,
        Output = 1,
    }

    public enum RelationshipType
    {
        OneToOne = 100,
        OneToMany = 200,
        ManyToOne = 300,
        ManyToMany = 400,
    }

    internal enum RuntimeBehaviour : int
    {
        CreateSchemaFirst,
        DeleteAndRecreate,
        DoNothing,
    }

    public enum Condition
    {
        NOTSET = 0,
        Equals = 100,
        GreaterThan = 150,
        LessThan = 200,
        GreaterThanEqualsTo = 250,
        LessThanEqualsTo = 310,
        NotEquals = 400,
        Default = Equals,
        Is = 450,
        IsNull = 460,
        IsNotNull = 470,
        StartsWith = 480,
        EndsWith = 490,
        Contains = 500,
        NotContains = 580,
        IN = 590,
        NOT_IN = 600,
        Between = 601,
    }

    internal enum Quote
    {
        None,
        Double,
        Single,
    }

    internal enum SelfProjectionType
    {
        None = 0, Self , Other
    }

    internal enum ProjectionFunction
    {
        NOTSET = 0, 
        COUNT = 451,
        TOP = 452,
        SUM = 453,
        MIN = 454,
        MAX = 455,
        AVG = 456,
        Last = 458,
        ANY = 460,
        Reverse = 461,
        ALL = 462,
        Substract = FunctionCall.Add ,
        Division = FunctionCall.Division ,
        Multiply = FunctionCall.Multiply ,
        Concat = FunctionCall.Concat ,
    }

    public enum ProviderDialect
    {
        SqlProvider = 0,
        OleDbProvider = 1,
        OracleProvider = 2,
        MySQLProvider = 3,
        Db2Provider = 4 ,
        SQLLite3,
        Default = OleDbProvider,        
    }

    internal enum FunctionCall : int
    {
        None ,
        Add ,
        Subtract ,
        Division ,
        Multiply ,
        Concat ,
        Trim ,
        TrimRight ,
        TrimLeft ,
        Coalesce ,
        Month ,
        Year ,
        Day ,
        Second ,
        Hour ,
        Minute ,
        LowerCase ,
        UpperCase ,
        Max ,
        Min ,
        Sum ,
        Avg ,
        Count ,
        StringLength ,
    }

    internal enum QueryPart : int
    {
        NONE = 0 ,
        SELECT = 1 ,
        WHERE = 2 ,
        ORDERBY = 4 ,
        GroupBy = 6 ,
        FROM = 8 ,
        ALIAS = 10 ,
        OR = 12 ,
        AND = 14 ,
        AND_NOT = 15 ,
        IN = 16 ,
        JOIN = 18 ,
        INSERT = 20 ,
        DELETE = 22 ,
        ENTITY_TYPE = 24 ,
        ENTITY_VALUE = 26 ,
        ENTITY_PARAMETERS = 28 ,
        UPDATE = 30 ,
        WHERE_ENTITY = 32 ,
        COUNT = 34 ,
        ORDERBY_ASC = 36 ,
        ORDERBY_DESC = 38 ,
        FUNCTION = 40 ,
        JOINED_SUBQUERY = 42 ,
        JOINED_INHERITANCE = 44 ,
        DISCRIMINATOR = 46 ,
        JOIN_WITH = 48 ,
        Appender = 50 ,
        ORDERBY_DIRECTION = 52 ,
        SELECT_HEADER = 54 ,
        //AggregateProjection = 56,
        JOINED_RELATED_INHERITANCE = 58 ,
        FROM_CLAUSE_SUBQUERY = 60 ,
        JOIN_LEFT = 62 ,
        JOIN_RIGHT = 64 ,
        UNCHECKED_SELECT = 66 ,
        CONDITIONS = 68 ,
        JOINED_EMBEDDED = 69 ,
        Aggregate_OrderBy = 70 ,
        Select_Projections = 71 ,
        HIDDEN = 72 ,
    }

    internal enum SecondaryFunctionCall : int
    {
        Substract = FunctionCall.Add ,
        Division = FunctionCall.Division ,
        Multiply = FunctionCall.Multiply ,
        Concat = FunctionCall.Concat ,
    }

    internal enum ORDERBY
    {
        ASC ,
        DESC ,
    }

    internal class EmbeddedProviderGUID
    {
        internal const string MYSQL_GUID = "{7107042E-9611-4ac9-BC86-BB3AFF0AABF6}";
        internal const string DB2_GUID = "{654C0B8E-8127-4389-81E6-4A29390D012D}";
        internal const string SQLLite_GUID = "{B4C606F1-DBBD-477a-8630-C95F8E477990}";
    }

    internal class Constant
    {
        internal const string CACHE_KEY_FORMAT = "{0}{1}_{2}";
        internal const string VIRTUAL_PROPERTY_PREFIX = "_Ghost_Skeleton_Property";
        internal const string CACHE_PLACEHOLDER_PROPERTY = "RapidEntityCachePropertyPlaceholder";
        internal const string GHOST_TRANSACTION_NAME = "Ghost_Transaction";
        internal const string DIRTY_CHECK = "DIRTY_CHECK";        
        internal const string ORPHAN_NAMING = "{0}_orphan";
        internal const string AUDIT_TRAIL = "{0}_AUDIT";
        internal const string RUNTIME_TRANSTRACTION = "RuntimeManagerVersion2_0";
        internal const string WHITE_SPACE = " ";
        internal const string ThisDLL = "PersistentManager";
        internal const string GACLocation = ThisDLL + "." + "GAC";
        internal const string RAPID_EMBEDDED_XML_MAPPING_SUFFIX = "-rapid.xml";
        internal const string ProviderXMLEmbeddedResourceLocation = GACLocation + ".DatabaseProviders.xml";
        internal const string DEFAULT_CONNECTION_KEY = "DEFAULT::RAPID::CONNECTION";
        internal const string SESSION_STACK = "SESSION_STACK";
        internal const string INFERED_PARAMETRES = "Infered-Parameters-Query";
        internal const string QUERY_SESSION = "Query-Session";
        internal const string QUERY_SCOPE = "QUERY::SCOPE";
        internal const string IGROUPING = "IGrouping";
        internal const string GenericIGrouping = "IGrouping`2";
        internal const string AS = " AS ";
        internal const string SELF_TRACKING_LIST_NAME = "REFSelfTrackingList";
    }

    internal class AttributeProperty
    {
        internal const string NAME = "Name";
        internal const string IS_UNIQUE = "IsUnique";
        internal const string DB_TYPE = "DbDataType";
        internal const string ALLOW_NULL = "AllowNullValue";
        internal const string IS_IMPORTED = "IsImported";
        internal const string TYPE = "Type";
        internal const string RELATION_COLUMN = "RelationColumn";
        internal const string JOIN_TABLE = "JoinTable";
        internal const string JOIN_COLUMN = "JoinColumn";
        internal const string OWNER_COLUMN = "OwnerColumn";
        internal const string CASCADE = "Cascade";
        internal const string VALUE = "Value";
        internal const string IS_COMPOSITE_KEY = "IsCompositeKey";
        internal const string KEY_NAME = "KeyName";
        internal const string AutoKey = "AutoKey";
        internal const string Types = "Types";
        internal const string ENTITY_ALWAYS_REFRESH = "AlwaysRefresh";
        internal const string EXCLUDE = "Exclude";
        internal const string KEY_PRIORITY = "Priority";
        internal const string LEFT_KEY = "LeftKey";
        internal const string RIGHT_KEY = "RightKey";
        internal const string IsSplit = "IsSplit";
    }

    internal static class EnumExtensions
    {
        internal static bool IsCombining( this FunctionCall function )
        {
            switch ( function )
            {
                case FunctionCall.Add:
                case FunctionCall.Coalesce:
                case FunctionCall.Subtract:
                case FunctionCall.Multiply:
                case FunctionCall.Division:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsAggregate( this FunctionCall function )
        {
            switch ( function )
            {
                case FunctionCall.Max:
                case FunctionCall.Min:
                case FunctionCall.Sum:
                case FunctionCall.Avg:
                case FunctionCall.Count:
                    return true;
                default:
                    return false;
            }
        }
    }
}
