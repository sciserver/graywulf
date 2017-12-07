using System;
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

        private static readonly ConfigurationProperty propIsAsync = new ConfigurationProperty(
            "isAsync", typeof(bool), false, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propAsyncQueueSize = new ConfigurationProperty(
            "asyncQueueSize", typeof(int), Constants.DefaultLogWriterAsyncQueueSize, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propAsyncTimeout = new ConfigurationProperty(
            "asyncTimeout", typeof(int), Constants.DefaultLogWriterAsyncTimeout, ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propFailOnError = new ConfigurationProperty(
            "failOnError", typeof(bool), true, ConfigurationPropertyOptions.None);

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
            properties.Add(propIsAsync);
            properties.Add(propAsyncQueueSize);
            properties.Add(propAsyncTimeout);
            properties.Add(propFailOnError);
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

        [ConfigurationProperty("isAsync", DefaultValue = false)]
        public bool IsAsync
        {
            get { return (bool)base[propIsAsync]; }
            set { base[propIsAsync] = value; }
        }

        [ConfigurationProperty("asyncQueueSize", DefaultValue = Constants.DefaultLogWriterAsyncQueueSize)]
        public int AsyncQueueSize
        {
            get { return (int)base[propAsyncQueueSize]; }
            set { base[propAsyncQueueSize] = value; }
        }

        [ConfigurationProperty("asyncTimeout", DefaultValue = Constants.DefaultLogWriterAsyncTimeout)]
        public int AsyncTimeout
        {
            get { return (int)base[propAsyncTimeout]; }
            set { base[propAsyncTimeout] = value; }
        }

        [ConfigurationProperty("failOnError", DefaultValue = true)]
        public bool FailOnError
        {
            get { return (bool)base[propFailOnError]; }
            set { base[propFailOnError] = value; }
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

            writer.IsAsync = this.IsAsync;
            writer.AsyncQueueSize = this.AsyncQueueSize;
            writer.AsyncTimeout = this.AsyncTimeout;
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
