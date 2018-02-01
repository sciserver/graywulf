using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.IO.Tasks
{
    public static class SqlBulkCopyExtension
    {
        private const String rowsCopiedFieldName = "_rowsCopied";

        private static FieldInfo rowsCopiedField = null;

        public static int RecordsAffected(this SqlBulkCopy bulkCopy)
        {
            if (rowsCopiedField == null)
            {
                rowsCopiedField = typeof(SqlBulkCopy).GetField(rowsCopiedFieldName, BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            }

            return (int)rowsCopiedField.GetValue(bulkCopy);
        }
    }
}
