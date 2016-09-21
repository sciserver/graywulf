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
            { typeof(DataType), DatabaseObjectType.DataType },
            { typeof(Table), DatabaseObjectType.Table },
            { typeof(View), DatabaseObjectType.View },
            { typeof(TableValuedFunction), DatabaseObjectType.TableValuedFunction },
            { typeof(ScalarFunction), DatabaseObjectType.ScalarFunction },
            { typeof(StoredProcedure), DatabaseObjectType.StoredProcedure },
        };

        public static readonly Dictionary<DatabaseObjectType, DatabaseObjectType> SimpleDatabaseObjectTypes = new Dictionary<DatabaseObjectType, DatabaseObjectType>()
        {
            { DatabaseObjectType.DataType, DatabaseObjectType.DataType },
            { DatabaseObjectType.Table, DatabaseObjectType.Table },
            { DatabaseObjectType.View, DatabaseObjectType.View },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.SqlInlineTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.ScalarFunction,DatabaseObjectType.ScalarFunction},
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectType.ScalarFunction },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectType.ScalarFunction },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectType.StoredProcedure },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectType.StoredProcedure },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectType.StoredProcedure },
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
            { DatabaseObjectType.DataType, DatabaseObjectNames.DataType_Singular },
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
            { DatabaseObjectType.DataType, DatabaseObjectNames.DataType_Plural },
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
