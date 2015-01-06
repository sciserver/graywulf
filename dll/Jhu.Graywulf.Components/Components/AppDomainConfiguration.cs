using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Components
{
    public class AppDomainConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propAssemblyPath = new ConfigurationProperty(
            "assemblyPath", typeof(string), null, ConfigurationPropertyOptions.None);

        static AppDomainConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propAssemblyPath);
        }

        #endregion
        #region Properties

        [ConfigurationProperty("assemblyPath")]
        public string AssemblyPath
        {
            get { return (string)base[propAssemblyPath]; }
            set { base[propAssemblyPath] = value; }
        }

        #endregion
        #region Constructors and initializers

        public AppDomainConfiguration()
        {
        }

        #endregion
    }
}
