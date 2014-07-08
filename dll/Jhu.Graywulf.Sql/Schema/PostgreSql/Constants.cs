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
            { DatabaseObjectType.ScalarFunction, "FUNCTION" },
            { DatabaseObjectType.TableValuedFunction, "FUNCTION" },
            { DatabaseObjectType.StoredProcedure, "FUNCTION" },
        };

        public const string TypeNameOidVector = "oidvector";
        public const string TypeNameUnknown = "unknown";
        public const string TypeNameRefCursor = "refcursor";
        public const string TypeNameChar = "char";
        public const string TypeNameBpChar = "bpchar";
        public const string TypeNameVarChar = "varchar";
        public const string TypeNameText = "text";
        public const string TypeNameName = "name";
        public const string TypeNameBytea = "bytea";
        public const string TypeNameBit = "bit";
        public const string TypeNameVarBit = "varbit";
        public const string TypeNameBoolean = "bool";
        public const string TypeNameInt16 = "int2";
        public const string TypeNameInt32 = "int4";
        public const string TypeNameInt64 = "int8";
        public const string TypeNameOid = "oid";
        public const string TypeNameReal = "float4";
        public const string TypeNameDouble = "float8";
        public const string TypeNameNumeric = "numeric";
        public const string TypeNameInet = "inet";
        public const string TypeNameMacaddr = "macaddr";
        public const string TypeNameMoney = "money";
        public const string TypeNamePoint = "point";
        public const string TypeNameLine = "line";
        public const string TypeNameLseg = "lseg";
        public const string TypeNamePath = "path";
        public const string TypeNameBox = "box";
        public const string TypeNameCircle = "circle";
        public const string TypeNamePolygon = "polygon";
        public const string TypeNameUuid = "uuid";
        public const string TypeNameXml = "xml";
        public const string TypeNameInterval = "interval";
        public const string TypeNameDate = "date";
        public const string TypeNameTime = "time";
        public const string TypeNameTimeWithTimeZone = "timetz";
        public const string TypeNameTimestamp = "timestamp";
        public const string TypeNameTimestampWithTimeZone = "timestamptz";
        public const string TypeNameAbsTime = "abstime";
        public const string TypeNameTsRange = "tsrange";
        public const string TypeNameTstzRange = "tstzrange";
        public const string TypeNameDateRange = "daterange";
        public const string TypeNameNumRange = "numrange";
        public const string TypeNameInt4Range = "int4range";
        public const string TypeNameInt8Range = "int8range";
        public const string TypeNameJson = "json";
        public const string TypeNameTsVector = "tsvector";
        public const string TypeNameCidr = "cidr";
        public const string TypeNameCString = "cstring";
    }
}