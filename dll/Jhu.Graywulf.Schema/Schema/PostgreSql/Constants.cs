using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema.PostgreSql
{
    public static class Constants
    {
        public const string PostgreSqlProviderName = "Npgsql";

        //Type of the table: BASE TABLE for a persistent base table (the normal table type), 
        //VIEW for a view, FOREIGN TABLE for a foreign table, or LOCAL TEMPORARY for a temporary table
        public static readonly Map<DatabaseObjectType, string> PostgreSqlObjectTypeIds = new Map<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "FUNCTION" },
        };
        public static readonly Dictionary<DatabaseObjectType, string> PostgreSqlObjectTypeNames = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "FUNCTION" },
        };

        public const string TypeNameUnknown = "unknown";
        public const string TypeNameSmallInt = "smallint";
        public const string TypeNameInt = "integer";
        public const string TypeNameBigInt = "bigint";
        public const string TypeNameNumeric = "numeric";
        public const string TypeNameReal = "real";
        public const string TypeNameDoublePrecision = "double precision";   // *** removed underscore
        public const string TypeNameMoney = "money";
        public const string TypeNameVarChar = "character varying";
        public const string TypeNameChar = "character";
        public const string TypeNameText = "text";
        public const string TypeNameBytea = "bytea";
        public const string TypeNameTimestamp = "timestamp without time zone";
        public const string TypeNameTimestampWithTimeZone = "timestamp with time zone";
        public const string TypeNameDate = "date";
        public const string TypeNameTime = "time without time zone";
        public const string TypeNameTimeWithTimeZone = "time with time zone";
        public const string TypeNameInterval = "interval";
        public const string TypeNameBoolean = "boolean";
        public const string TypeNamePoint = "point";
        public const string TypeNameLine = "line";
        public const string TypeNameLseg = "lseg";
        public const string TypeNameBox = "box";
        public const string TypeNamePath = "path";
        public const string TypeNamePolygon = "polygon";
        public const string TypeNameCircle = "circle";
        public const string TypeNameCidr = "cidr";
        public const string TypeNameInet = "inet";
        public const string TypeNameMacaddr = "macaddr";
        public const string TypeNameBit = "bit";
        public const string TypeNameBitVarying = "bit varying";
        public const string TypeNameTsvector = "tsvector";
        public const string TypeNameUuid = "uuid";
        public const string TypeNameXml = "xml";
        public const string TypeNameJson = "json";
        public const string TypeNameArray = "ARRAY";
        public const string TypeNameInt4Range = "int4range";
        public const string TypeNameInt8Range = "int8range";
        public const string TypeNameNumRange = "numrange";
        public const string TypeNameTsRange = "tsrange";
        public const string TypeNameTstzRange = "tstzrange";
        public const string TypeNameDateRange = "daterange";
        public const string TypeNameOid = "oid";
    }
}
