using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Security
{
    public abstract class AuthenticationConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propIsEnabled = new ConfigurationProperty(
            "enabled", typeof(bool), false, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propAuthorityName = new ConfigurationProperty(
            "authorityName", typeof(string), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propAuthorityUri = new ConfigurationProperty(
            "authorityUri", typeof(Uri), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propIsMasterAuthority = new ConfigurationProperty(
            "isMasterAuthority", typeof(bool), null, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propDisplayName = new ConfigurationProperty(
            "displayName", typeof(string), null, ConfigurationPropertyOptions.None);

        static AuthenticationConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propIsEnabled);
            properties.Add(propAuthorityName);
            properties.Add(propAuthorityUri);
            properties.Add(propIsMasterAuthority);
            properties.Add(propDisplayName);
        }

        #endregion
        [ConfigurationProperty("enabled", DefaultValue=false)]
        public bool IsEnabled
        {
            get { return (bool)base[propIsEnabled]; }
            set { base[propIsEnabled] = value; }
        }

        [ConfigurationProperty("authorityName")]
        public string AuthorityName
        {
            get { return (string)base[propAuthorityName]; }
            set { base[propAuthorityName] = value; }
        }

        [ConfigurationProperty("authorityUri")]
        public Uri AuthorityUri
        {
            get { return (Uri)base[propAuthorityUri]; }
            set { base[propAuthorityUri] = value; }
        }

        [ConfigurationProperty("isMasterAuthority")]
        public bool IsMasterAuthority
        {
            get { return (bool)base[propIsMasterAuthority]; }
            set { base[propIsMasterAuthority] = value; }
        }

        [ConfigurationProperty("displayName")]
        public string DisplayName
        {
            get { return (string)base[propDisplayName]; }
            set { base[propDisplayName] = value; }
        }
    }
}
