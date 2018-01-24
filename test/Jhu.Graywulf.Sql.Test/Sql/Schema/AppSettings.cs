using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Sql.Schema
{
    public static class AppSettings
    {
        public static string SqlServerConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.SqlServer.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string MySqlConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.MySql.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string PostgreSqlConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Schema.PostgreSql.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }
    }
}
