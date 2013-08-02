using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema.SqlServer
{
    public static class Constants
    {
        public const string SqlServerProviderName = "System.Data.SqlClient";

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
        public const string MetaExample = "meta.example";
        public const string MetaContent = "meta.content";
        public const string MetaUnit = "meta.unit";
    }
}
