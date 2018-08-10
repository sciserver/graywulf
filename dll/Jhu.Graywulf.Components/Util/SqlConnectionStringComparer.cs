using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Util
{
    public static class SqlConnectionStringComparer
    {
        public static bool IsSameDataSource(string cstr1, string cstr2)
        {
            return IsSameDataSource(new SqlConnectionStringBuilder(cstr1), new SqlConnectionStringBuilder(cstr2));
        }

        public static bool IsSameDataSource(SqlConnectionStringBuilder cstr1, string cstr2)
        {
            return IsSameDataSource(cstr1, new SqlConnectionStringBuilder(cstr2));
        }

        public static bool IsSameDataSource(string cstr1, SqlConnectionStringBuilder cstr2)
        {
            return IsSameDataSource(new SqlConnectionStringBuilder(cstr1), cstr2);
        }

        public static bool IsSameDataSource(SqlConnectionStringBuilder cstr1, SqlConnectionStringBuilder cstr2)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(cstr1.DataSource, cstr2.DataSource) == 0;
        }
    }
}
