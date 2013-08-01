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
            //{ DatabaseObjectType.SqlTableValuedFunction, "TF"},
            //{ DatabaseObjectType.ClrTableValuedFunction, "FT"},
            //{ DatabaseObjectType.SqlScalarFunction, "FN" },
            //{ DatabaseObjectType.ClrScalarFunction, "FS" },
            //{ DatabaseObjectType.SqlStoredProcedure, "P" },
            //{ DatabaseObjectType.ClrStoredProcedure, "PC" },
        };

    }
}
