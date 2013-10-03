using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema.PostgreSql
{
    class Constants
    {
        public const string PostgreSqlProviderName = "Npgsql";

        public static readonly Map<DatabaseObjectType, string> PostgreSqlObjectTypeIds = new Map<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "FUNCTION" },


            //Type of the table: BASE TABLE for a persistent base table (the normal table type), 
            //VIEW for a view, FOREIGN TABLE for a foreign table, or LOCAL TEMPORARY for a temporary table
            
            
            
            //{DatabaseObjectType.ScalarFunction,"FUNCTION"},
            //{DatabaseObjectType.StoredProcedure,"PROCEDURE"},
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
        public static readonly Dictionary<DatabaseObjectType, string> PostgreSqlObjectTypeNames = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.Table, "BASE TABLE" },
            { DatabaseObjectType.View, "VIEW" },
            { DatabaseObjectType.StoredProcedure, "FUNCTION" },
            //{ DatabaseObjectType.StoredProcedure, "ROUTINE" },
            //{ DatabaseObjectType.ScalarFunction, "ROUTINE" },
            
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
