using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.RemoteService
{
    public class RemoteServiceConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propTcpPort = new ConfigurationProperty(
            "tcpPort", typeof(int), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propUserGroup = new ConfigurationProperty(
            "userGroup", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        static RemoteServiceConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propTcpPort);
            properties.Add(propUserGroup);
        }

        #endregion
        #region Properties

        public int TcpPort
        {
            get { return (int)base[propTcpPort]; }
            set { base[propTcpPort] = value; }
        }

        public string UserGroup
        {
            get { return (string)base[propUserGroup]; }
            set { base[propUserGroup] = value; }
        }

        #endregion
    }
}
