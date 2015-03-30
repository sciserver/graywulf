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
        public const string DefaultSchemaName = "dbo";      // TODO: this is not necessary, some tests still use it though

        /// <summary>
        /// MySql object type indentifiers
        /// </summary>
        public static readonly Map<DatabaseObjectType, string> MySqlObjectTypeIds = new Map<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            {DatabaseObjectType.StoredProcedure,"PROCEDURE"},
            {DatabaseObjectType.ScalarFunction,"FUNCTION"},
        };

        public static readonly Dictionary<DatabaseObjectType, string> MySqlObjectTypeNames = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "ROUTINE" },
            { DatabaseObjectType.ScalarFunction, "ROUTINE" },
        };

        public const string TypeNameUnknown = "unknown";
        public const string TypeNameTinyInt = "tinyint";
        public const string TypeNameSmallInt = "smallint";
        public const string TypeNameInt = "int";
        public const string TypeNameMediumInt = "mediumint";
        public const string TypeNameBigInt = "bigint";
        public const string TypeNameFloat = "float";
        public const string TypeNameDouble = "double";
        public const string TypeNameDecimal = "decimal";
        public const string TypeNameDate = "date";
        public const string TypeNameYear = "year";//int16
        public const string TypeNameTime = "time";
        public const string TypeNameDateTime = "datetime";
        public const string TypeNameTimestamp = "timestamp";
        public const string TypeNameTinyText = "tinytext";//char  length 255 (2^8 - 1)
        public const string TypeNameText = "text";
        public const string TypeNameMediumText = "mediumtext";//char length 16777215 (2^24 - 1)
        public const string TypeNameLongText = "longtext";//char length 4294967295 (2^32 - 1)
        public const string TypeNameTinyBlob = "tinyblob";//char lenght 255 (2^8 - 1)
        public const string TypeNameBlob = "blob";//char lenght 65535 (2^16 - 1)
        public const string TypeNameMediumBlob = "mediumblob";//char lenght 16777215 (2^24 - 1)
        public const string TypeNameLongBlob = "longblob";//char lenght 4294967295 or 4G (2^32 - 1)
        public const string TypeNameBit = "bit";
        public const string TypeNameSet = "set";//char a lot of value max 64
        public const string TypeNameEnum = "enum";//char one value
        public const string TypeNameBinary = "binary";
        public const string TypeNameVarBinary = "varbinary";
        public const string TypeNameGeometry = "geometry";
        public const string TypeNameChar = "char";
        public const string TypeNameVarChar = "varchar";

        public const string TypeNameXml = "xml";

    }
}
