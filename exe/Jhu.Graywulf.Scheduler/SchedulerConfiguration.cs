using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propPollingInterval = new ConfigurationProperty(
            "pollingInterval", typeof(TimeSpan), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propAppDomainIdle = new ConfigurationProperty(
            "appDomainIdle", typeof(TimeSpan), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propAppDomainShutdownTimeout = new ConfigurationProperty(
            "appDomainShutdownTimeout", typeof(TimeSpan), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propCancelTimeout = new ConfigurationProperty(
            "cancelTimeout", typeof(TimeSpan), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propPersistTimeout = new ConfigurationProperty(
            "persistTimeout", typeof(TimeSpan), null, ConfigurationPropertyOptions.IsRequired);

        static SchedulerConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propPollingInterval);
            properties.Add(propAppDomainIdle);
            properties.Add(propAppDomainShutdownTimeout);
            properties.Add(propCancelTimeout);
            properties.Add(propPersistTimeout);
        }

        #endregion

        [ConfigurationProperty("pollingInterval")]
        public TimeSpan PollingInterval
        {
            get { return (TimeSpan)base[propPollingInterval]; }
            set { base[propPollingInterval] = value; }
        }

        [ConfigurationProperty("appDomainIdle")]
        public TimeSpan AppDomainIdle
        {
            get { return (TimeSpan)base[propAppDomainIdle]; }
            set { base[propAppDomainIdle] = value; }
        }

        [ConfigurationProperty("appDomainShutdownTimeout")]
        public TimeSpan AppDomainShutdownTimeout
        {
            get { return (TimeSpan)base[propAppDomainShutdownTimeout]; }
            set { base[propAppDomainShutdownTimeout] = value; }
        }

        [ConfigurationProperty("cancelTimeout")]
        public TimeSpan CancelTimeout
        {
            get { return (TimeSpan)base[propCancelTimeout]; }
            set { base[propCancelTimeout] = value; }
        }

        [ConfigurationProperty("persistTimeout")]
        public TimeSpan PersistTimeout
        {
            get { return (TimeSpan)base[propPersistTimeout]; }
            set { base[propPersistTimeout] = value; }
        }

        // --

        public string PersistenceConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Activities.Persistence"].ConnectionString; }
        }

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
                ef.LoadEntity<Registry.Cluster>(Registry.AppSettings.ClusterName);
            }
        }
    }
}
