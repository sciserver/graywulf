using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    public class RegistryConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propConnectionString = new ConfigurationProperty(
            "connectionString", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propWriteCommandTimeout = new ConfigurationProperty(
            "writeCommandTimeout", typeof(int), 2, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propReadOnlyCommandTimeout = new ConfigurationProperty(
            "readOnlyCommandTimeout", typeof(int), 5, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propClusterName = new ConfigurationProperty(
            "clusterName", typeof(string), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propDomainName = new ConfigurationProperty(
            "domainName", typeof(string), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propFederationName = new ConfigurationProperty(
            "federationName", typeof(string), null, ConfigurationPropertyOptions.None);

        static RegistryConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propConnectionString);
            properties.Add(propClusterName);
            properties.Add(propDomainName);
            properties.Add(propFederationName);
        }

        #endregion

        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get { return (string)base[propConnectionString]; }
            set { base[propConnectionString] = value; }
        }

        [ConfigurationProperty("writeCommandTimeout")]
        public int WriteCommandTimeout
        {
            get { return (int)base[propWriteCommandTimeout]; }
            set { base[propWriteCommandTimeout] = value; }
        }

        [ConfigurationProperty("readOnlyCommandTimeout")]
        public int ReadOnlyCommandTimeout
        {
            get { return (int)base[propReadOnlyCommandTimeout]; }
            set { base[propReadOnlyCommandTimeout] = value; }
        }

        [ConfigurationProperty("clusterName")]
        public string ClusterName
        {
            get { return (string)base[propClusterName]; }
            set { base[propClusterName] = value; }
        }

        [ConfigurationProperty("domainName")]
        public string DomainName
        {
            get { return (string)base[propDomainName]; }
            set { base[propDomainName] = value; }
        }

        [ConfigurationProperty("federationName")]
        public string FederationName
        {
            get { return (string)base[propFederationName]; }
            set { base[propFederationName] = value; }
        }
    }
}
