using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    public static class Constants
    {
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

        public const string SchemaColumnColumnName = "ColumnName";
        public const string SchemaColumnColumnOrdinal = "ColumnOrdinal";
        public const string SchemaColumnColumnSize = "ColumnSize";
        public const string SchemaColumnNumericPrecision = "NumericPrecision";
        public const string SchemaColumnNumericScale = "NumericScale";
        public const string SchemaColumnIsUnique = "IsUnique";
        public const string SchemaColumnIsKey = "IsKey";
        public const string SchemaColumnDataType = "DataType";
        public const string SchemaColumnAllowDBNull = "AllowDBNull";
        public const string SchemaColumnProviderType = "ProviderType";
        public const string SchemaColumnIsAliased = "IsAliased";
        public const string SchemaColumnIsExpression = "IsExpression";
        public const string SchemaColumnIsIdentity = "IsIdentity";
        public const string SchemaColumnIsAutoIncrement = "IsAutoIncrement";
        public const string SchemaColumnIsRowVersion = "IsRowVersion";
        public const string SchemaColumnIsHidden = "IsHidden";
        public const string SchemaColumnIsLong = "IsLong";
        public const string SchemaColumnIsReadOnly = "IsReadOnly";
        public const string SchemaColumnProviderSpecificDataType = "ProviderSpecificDataType";
        public const string SchemaColumnDataTypeName = "DataTypeName";

        public static readonly Map<Type, DatabaseObjectType> DatabaseObjectTypes = new Map<Type, DatabaseObjectType>()
        {
            { typeof(Table), DatabaseObjectType.Table },
            { typeof(View), DatabaseObjectType.View },
            { typeof(TableValuedFunction), DatabaseObjectType.TableValuedFunction },
            { typeof(ScalarFunction), DatabaseObjectType.ScalarFunction },
            { typeof(StoredProcedure), DatabaseObjectType.StoredProcedure },
        };

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
        /// Database object singular names
        /// </summary>
        public static readonly Dictionary<DatabaseObjectType, string> DatabaseObjectsName_Singular = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, DatabaseObjectNames.Table_Singular },
            { DatabaseObjectType.View, DatabaseObjectNames.View_Singular },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.ScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
        };

        /// <summary>
        /// Database object plural names
        /// </summary>
        public static readonly Dictionary<DatabaseObjectType, string> DatabaseObjectsName_Plural = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, DatabaseObjectNames.Table_Plural },
            { DatabaseObjectType.View, DatabaseObjectNames.View_Plural },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.ScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
        };
    }
}
