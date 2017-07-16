﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public abstract class LogWriterConfigurationBase : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propIsEnabled = new ConfigurationProperty(
            "enabled", typeof(bool), false, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propSourceMask = new ConfigurationProperty(
            "sourceMask", typeof(string), "*", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propSeverityMask = new ConfigurationProperty(
            "severityMask", typeof(string), "*", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propStatusMask = new ConfigurationProperty(
            "statusMask", typeof(string), "*", ConfigurationPropertyOptions.None);

        static LogWriterConfigurationBase()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propIsEnabled);
            properties.Add(propSourceMask);
            properties.Add(propSeverityMask);
            properties.Add(propStatusMask);
        }

        #endregion
        #region Properties

        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool IsEnabled
        {
            get { return (bool)base[propIsEnabled]; }
            set { base[propIsEnabled] = value; }
        }

        [ConfigurationProperty("sourceMask", DefaultValue = "*")]
        public string SourceMask
        {
            get { return (string)base[propSourceMask]; }
            set { base[propSourceMask] = value; }
        }

        [ConfigurationProperty("severityMask", DefaultValue = "*")]
        public string SeverityMask
        {
            get { return (string)base[propSeverityMask]; }
            set { base[propSeverityMask] = value; }
        }

        [ConfigurationProperty("statusMask", DefaultValue = "*")]
        public string StatusMask
        {
            get { return (string)base[propStatusMask]; }
            set { base[propStatusMask] = value; }
        }

        #endregion

        public LogWriterBase CreateLogWriter()
        {
            var writer = OnCreateLogWriter();

            writer.SourceMask = ParseMask<EventSource>(SourceMask);
            writer.SeverityMask = ParseMask<EventSeverity>(SeverityMask);
            writer.StatusMask = ParseMask<ExecutionStatus>(StatusMask);

            return writer;
        }

        protected abstract LogWriterBase OnCreateLogWriter();
        
        private T ParseMask<T>(string value)
            where T : struct
        {
            T res = default(T);

            if (value == "*")
            {
                return (T)Enum.ToObject(typeof(T), 0x7FFFFFFF);
            }
            else if (Enum.TryParse<T>(value, out res))
            {
                return res;
            }
            else
            {
                return (T)Enum.ToObject(typeof(T), 0);
            }
        }
    }
}