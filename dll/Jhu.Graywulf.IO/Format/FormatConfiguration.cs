using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Format
{
    public class FormatConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propFileFormatFactory = new ConfigurationProperty(
            "fileFormatFactory", typeof(string), null, ConfigurationPropertyOptions.None);

        static FormatConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propFileFormatFactory);
        }

        #endregion

        [ConfigurationProperty("fileFormatFactory")]
        public string FileFormatFactory
        {
            get { return (string)base[propFileFormatFactory]; }
            set { base[propFileFormatFactory] = value; }
        }
    }
}
