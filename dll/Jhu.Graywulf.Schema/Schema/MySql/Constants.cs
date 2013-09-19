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

    }
}
