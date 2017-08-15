using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("pollingInterval")]
        [DefaultValue("00:00:05")]
        public TimeSpan PollingInterval
        {
            get { return (TimeSpan)base["pollingInterval"]; }
            set { base["pollingInterval"] = value; }
        }

        [ConfigurationProperty("appDomainIdle")]
        [DefaultValue("00:20:00")]
        public TimeSpan AppDomainIdle
        {
            get { return (TimeSpan)base["appDomainIdle"]; }
            set { base["appDomainIdle"] = value; }
        }
        
        [ConfigurationProperty("cancelTimeout")]
        [DefaultValue("00:02:00")]
        public TimeSpan CancelTimeout
        {
            get { return (TimeSpan)base["cancelTimeout"]; }
            set { base["cancelTimeout"] = value; }
        }

        [ConfigurationProperty("persistenceConnectionString")]
        public string PersistenceConnectionString
        {
            get { return (string)base["persistenceConnectionString"]; }
            set { base["persistenceConnectionString"] = value; }
        }

        [ConfigurationProperty("persistTimeout")]
        [DefaultValue("00:02:00")]
        public TimeSpan PersistTimeout
        {
            get { return (TimeSpan)base["persistTimeout"]; }
            set { base["persistTimeout"] = value; }
        }

        [ConfigurationProperty("endpoint")]
        public TcpEndpointConfiguration Endpoint
        {
            get { return (TcpEndpointConfiguration)base["endpoint"]; }
            set { base["endpoint"] = value; }
        }

        // --

        public void RunSanityCheck()
        {
            // Test persistence service connection
            using (var cn = new SqlConnection(PersistenceConnectionString))
            {
                cn.Open();
            }

            // Test well-formated variables
            TimeSpan ts;
            ts = PollingInterval;
            ts = AppDomainIdle;

            // Test cluster registry settings
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                ef.LoadEntity<Registry.Cluster>(Registry.ContextManager.Configuration.ClusterName);
            }
        }
    }
}
