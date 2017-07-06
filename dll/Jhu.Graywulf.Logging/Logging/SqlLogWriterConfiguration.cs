using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class SqlLogWriterConfiguration : LogWriterConfigurationBase
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propConnectionString = new ConfigurationProperty(
            "connectionString", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty propSkipExceptions = new ConfigurationProperty(
            "skipException", typeof(bool), false, ConfigurationPropertyOptions.None);

        static SqlLogWriterConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propConnectionString);
            properties.Add(propSkipExceptions);
        }

        #endregion
        #region Properties

        [ConfigurationProperty("connectionString", DefaultValue = null)]
        public string ConnectionString
        {
            get { return (string)base[propConnectionString]; }
            set { base[propConnectionString] = value; }
        }

        [ConfigurationProperty("skipException", DefaultValue = false)]
        public bool SkipExceptions
        {
            get { return (bool)base[propSkipExceptions]; }
            set { base[propSkipExceptions] = value; }
        }

        #endregion

        protected override LogWriterBase OnCreateLogWriter()
        {
            var writer = new SqlLogWriter();

            writer.ConnectionString = this.ConnectionString;
            writer.SkipExceptions = this.SkipExceptions;

            return writer;
        }
    }
}
