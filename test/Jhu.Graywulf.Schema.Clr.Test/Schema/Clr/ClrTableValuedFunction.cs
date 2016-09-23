using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Schema.Clr
{
    public static partial class ClrObjects
    {
        [SqlFunction(
            FillRowMethodName = "ClrTableValuedFunction_Fill",
            TableDefinition = "ID int")]
        public static IEnumerable ClrTableValuedFunction(SqlInt32 parameter)
        {
            return new int[] { 0 };
        }

        public static void ClrTableValuedFunction_Fill(object values, out int a)
        {
            a = (int)values;
        }
    }
}
