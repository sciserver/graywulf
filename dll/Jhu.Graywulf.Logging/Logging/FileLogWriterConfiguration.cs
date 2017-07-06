using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class FileLogWriterConfiguration : LogWriterConfigurationBase
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propPath = new ConfigurationProperty(
            "path", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        static FileLogWriterConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propPath);
        }

        #endregion
        #region Properties

        [ConfigurationProperty("path", DefaultValue = null)]
        public string Path
        {
            get { return (string)base[propPath]; }
            set { base[propPath] = value; }
        }

        #endregion

        protected override LogWriterBase OnCreateLogWriter()
        {
            var writer =  new FileLogWriter();

            writer.Path = this.Path;

            return writer;
        }
    }
}
