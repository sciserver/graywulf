using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Schema
{
    public static class Constants
    {

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
