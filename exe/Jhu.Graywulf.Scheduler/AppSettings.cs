using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    public static class AppSettings
    {
        private static string GetValue(string key)
        {
            return (string)((NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Scheduler"))[key];
        }

        public static TimeSpan PollingInterval
        {
            get { return TimeSpan.Parse(GetValue("PollingInterval")); }
        }

        public static TimeSpan AppDomainIdle
        {
            get { return TimeSpan.Parse(GetValue("AppDomainIdle")); }
        }

        public static TimeSpan AppDomainShutdownTimeout
        {
            get { return TimeSpan.Parse(GetValue("AppDomainShutdownTimeout")); }
        }

        public static TimeSpan CancelTimeout
        {
            get { return TimeSpan.Parse(GetValue("CancelTimeout")); }
        }

        public static TimeSpan PersistTimeout
        {
            get { return TimeSpan.Parse(GetValue("PersistTimeout")); }
        }

        // --

        public static string PersistenceConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Activities.Persistence"].ConnectionString; }
        }

        public static void RunSanityCheck()
        {
            // Test persistence service connection
            using (SqlConnection cn = new SqlConnection(PersistenceConnectionString))
            {
                cn.Open();
            }

            // Test well-formated variables
            TimeSpan ts;
            ts = PollingInterval;
            ts = AppDomainIdle;

            // Test cluster registry settings
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                EntityFactory ef = new EntityFactory(context);
                ef.LoadEntity<Registry.Cluster>(Registry.AppSettings.ClusterName);
            }
        }
    }
}
