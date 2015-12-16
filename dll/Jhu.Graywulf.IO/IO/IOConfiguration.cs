using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.IO
{
    public class IOConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propStreamFactory = new ConfigurationProperty(
            "streamFactory", typeof(string), null, ConfigurationPropertyOptions.None);

        static IOConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propStreamFactory);
        }

        #endregion

        [ConfigurationProperty("streamFactory")]
        public string StreamFactory
        {
            get { return (string)base[propStreamFactory]; }
            set { base[propStreamFactory] = value; }
        }
    }
}
