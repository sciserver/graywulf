using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Test
{
    public static class AppSettings
    {
        public static string WebAuthPath
        {
            get
            {
                return ConfigurationManager.AppSettings["Jhu.Graywulf.Web.Auth.Path"] ?? "/auth";
            }
        }

        public static string WebUIPath
        {
            get
            {
                return ConfigurationManager.AppSettings["Jhu.Graywulf.Web.UI.Path"] ?? "/gwui";
            }
        }

        public static string IOTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.IO.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string RegistryTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Registry.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string LoggingTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Logging.Test"];
                return cs != null ? cs.ConnectionString : null;
            }
        }

        public static string JobPersistenceTestConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Activities.Persistence.Test"];
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
