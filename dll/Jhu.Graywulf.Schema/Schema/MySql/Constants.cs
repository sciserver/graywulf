using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema.MySql
{
    public static class Constants
    {
        public const string MySqlProviderName = "MySql.Data.MySqlClient";

        /// <summary>
        /// SQL Server object type indentifiers, as used in the sys.objects table
        /// </summary>
        public static readonly Map<DatabaseObjectType, string> MySqlObjectTypeIds = new Map<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            {DatabaseObjectType.StoredProcedure,"PROCEDURE"},
            {DatabaseObjectType.ScalarFunction,"FUNCTION"},
            //{DatabaseObjectType.TableValuedFunction,"TFUNCTION"},
           
            
            /*
             { DatabaseObjectType.SqlTableValuedFunction, "FUNCTION"},
            //{ DatabaseObjectType.ClrTableValuedFunction, "FT"},
            { DatabaseObjectType.SqlScalarFunction, "FN" },
            { DatabaseObjectType.ClrScalarFunction, "FS" },
            { DatabaseObjectType.SqlStoredProcedure, "PROCEDURE" },
            { DatabaseObjectType.ClrStoredProcedure, "sPROCEDUEa" },
             */
        };

        public static readonly Dictionary<DatabaseObjectType, string> MySqlObjectTypeNames = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "ROUTINE" },
            { DatabaseObjectType.ScalarFunction, "ROUTINE" },
            
            /*
            { DatabaseObjectType.SqlTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.SqlInlineTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.ClrTableValuedFunction, "FUNCTION"},
            { DatabaseObjectType.SqlScalarFunction, "ROUTINE" },
            { DatabaseObjectType.ClrScalarFunction, "ROUTINE" },
            { DatabaseObjectType.SqlStoredProcedure, "ROUTINE" },
            { DatabaseObjectType.ClrStoredProcedure, "ROUTINE" },*/
        };


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
        public const string TypeNameYear = "year";//int16
        public const string TypeNameTinyBlob = "tinyblob";//char lenght 255 (2^8 - 1)
        public const string TypeNameBlob = "blob";//char lenght 65535 (2^16 - 1)
        public const string TypeNameMediumBlob = "mediumblob";//char lenght 16777215 (2^24 - 1)
        public const string TypeNameLongBlob = "longblob";//char lenght 4294967295 or 4G (2^32 - 1)
        public const string TypeNameSet = "set";//char a lot of value max 64
        public const string TypeNameEnum = "enum";//char one value
        public const string TypeNameTinyText = "tinytext";//char  length 255 (2^8 - 1)
        public const string TypeNameMediumText = "mediumtext";//char length 16777215 (2^24 - 1)
        public const string TypeNameLongText = "longtext";//char length 4294967295 (2^32 - 1)
        public const string TypeNameGeometry = "geometry";

    }
}
