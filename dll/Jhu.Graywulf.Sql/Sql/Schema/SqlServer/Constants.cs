using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.Schema.SqlServer
{
    public static class Constants
    {
        public const string SqlServerProviderName = "System.Data.SqlClient";
        public const string DefaultSchemaName = "dbo";

        /// <summary>
        /// SQL Server object type indentifiers, as used in the sys.objects table
        /// </summary>
        public static readonly Map<DatabaseObjectType, string> SqlServerObjectTypeIds = new Map<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "U" },
            { DatabaseObjectType.View, "V" },
            { DatabaseObjectType.SqlTableValuedFunction, "TF"},
            { DatabaseObjectType.SqlInlineTableValuedFunction, "IF"},
            { DatabaseObjectType.ClrTableValuedFunction, "FT"},
            { DatabaseObjectType.SqlScalarFunction, "FN" },
            { DatabaseObjectType.ClrScalarFunction, "FS" },
            { DatabaseObjectType.SqlStoredProcedure, "P" },
            { DatabaseObjectType.ClrStoredProcedure, "PC" },
        };

        /// <summary>
        /// SQL Server object type names
        /// </summary>
        public static readonly Dictionary<DatabaseObjectType, string> SqlServerObjectTypeNames = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.SqlTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.SqlInlineTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.ClrTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.SqlScalarFunction, "FUNCTION" },
            { DatabaseObjectType.ClrScalarFunction, "FUNCTION" },
            { DatabaseObjectType.SqlStoredProcedure, "PROCEDURE" },
            { DatabaseObjectType.ClrStoredProcedure, "PROCEDURE" },
        };

        public const string MetaSummary = "meta.summary";
        public const string MetaRemarks = "meta.remarks";
        public const string MetaUrl = "meta.url";
        public const string MetaIcon = "meta.icon";
        public const string MetaDocPage = "meta.docpage";
        public const string MetaExample = "meta.example";
        public const string MetaClass = "meta.class";
        public const string MetaQuantity = "meta.quantity";
        public const string MetaUnit = "meta.unit";
        public const string MetaFormat = "meta.format";


        public const string TypeNameUnknown = "unknown";
        public const string TypeNameTinyInt = "tinyint";
        public const string TypeNameSmallInt = "smallint";
        public const string TypeNameInt = "int";
        public const string TypeNameBigInt = "bigint";
        public const string TypeNameBit = "bit";
        public const string TypeNameDecimal = "decimal";
        public const string TypeNameSmallMoney = "smallmoney";
        public const string TypeNameMoney = "money";
        public const string TypeNameNumeric = "numeric";
        public const string TypeNameReal = "real";
        public const string TypeNameFloat = "float";
        public const string TypeNameDate = "date";
        public const string TypeNameTime = "time";
        public const string TypeNameSmallDateTime = "smalldatetime";
        public const string TypeNameDateTime = "datetime";
        public const string TypeNameDateTime2 = "datetime2";
        public const string TypeNameDateTimeOffset = "datetimeoffset";
        public const string TypeNameChar = "char";
        public const string TypeNameVarChar = "varchar";
        public const string TypeNameText = "text";
        public const string TypeNameNChar = "nchar";
        public const string TypeNameNVarChar = "nvarchar";
        public const string TypeNameNText = "ntext";
        public const string TypeNameXml = "xml";
        public const string TypeNameBinary = "binary";
        public const string TypeNameVarBinary = "varbinary";
        public const string TypeNameImage = "image";
        public const string TypeNameSqlVariant = "sql_variant";
        public const string TypeNameTimestamp = "timestamp";
        public const string TypeNameUniqueIdentifier = "uniqueidentifier";

        public static readonly Dictionary<string, SqlDbType> SqlDataTypes = new Dictionary<string, SqlDbType>(SchemaManager.Comparer)
        {
            { TypeNameTinyInt, SqlDbType.TinyInt },
            { TypeNameSmallInt, SqlDbType.SmallInt },
            { TypeNameInt, SqlDbType.Int },
            { TypeNameBigInt, SqlDbType.BigInt },
            { TypeNameBit, SqlDbType.Bit },
            { TypeNameDecimal, SqlDbType.Decimal },
            { TypeNameSmallMoney, SqlDbType.SmallMoney },
            { TypeNameMoney, SqlDbType.Money },
            { TypeNameNumeric, SqlDbType.Decimal },
            { TypeNameReal, SqlDbType.Real },
            { TypeNameFloat, SqlDbType.Float },
            { TypeNameDate, SqlDbType.Date },
            { TypeNameTime, SqlDbType.Time },
            { TypeNameSmallDateTime , SqlDbType.SmallDateTime },
            { TypeNameDateTime, SqlDbType.DateTime },
            { TypeNameDateTime2, SqlDbType.DateTime2 },
            { TypeNameDateTimeOffset, SqlDbType.DateTimeOffset },
            { TypeNameChar, SqlDbType.Char },
            { TypeNameVarChar, SqlDbType.VarChar },
            { TypeNameText, SqlDbType.Text },
            { TypeNameNChar, SqlDbType.NChar },
            { TypeNameNVarChar, SqlDbType.NVarChar },
            { TypeNameNText, SqlDbType.NText },
            { TypeNameXml, SqlDbType.Xml },
            { TypeNameBinary, SqlDbType.Binary },
            { TypeNameVarBinary, SqlDbType.VarBinary },
            { TypeNameImage, SqlDbType.Image },
            { TypeNameSqlVariant, SqlDbType.Variant },
            { TypeNameTimestamp, SqlDbType.Timestamp },
            { TypeNameUniqueIdentifier, SqlDbType.UniqueIdentifier }
        };
    }
}
