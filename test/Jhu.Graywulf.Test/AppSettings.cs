using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Test
{
    public static class AppSettings
    {
        public static string IOTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.IO.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string SqlServerSchemaTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.SqlServer.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string MySqlSchemaTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.MySql.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string PostgreSqlSchemaTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.PostgreSql.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }
    }
}
