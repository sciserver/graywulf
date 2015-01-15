using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Security
{
    public class FormsAuthenticationConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propBaseUri = new ConfigurationProperty(
            "baseUri", typeof(Uri), new Uri("/auth", UriKind.Relative), ConfigurationPropertyOptions.None);

        static FormsAuthenticationConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propBaseUri);
        }

        #endregion
        [ConfigurationProperty("baseUri")]
        public Uri BaseUri
        {
            get { return (Uri)base[propBaseUri]; }
            set { base[propBaseUri] = value; }
        }
    }
}
