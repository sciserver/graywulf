using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Web.Hosting;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI
{
    internal class WebConfigHelper : MarshalByRefObject
    {
        public static bool Configure()
        {
            var binPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "bin");
            var webConfigPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "web.config");

            var ads = new AppDomainSetup()
            {
                ApplicationBase = binPath,
                ConfigurationFile = webConfigPath,
                ShadowCopyFiles = "true"
            };

            var app = AppDomain.CreateDomain("WebConfigHelperAppDomain", null, ads);

            var wch = (WebConfigHelper)app.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(WebConfigHelper).FullName);

            var res = wch.Configure(webConfigPath);

            AppDomain.Unload(app);

            return res;
        }

        public bool Configure(string webConfigPath)
        {
            using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit))
            {
                var domain = registryContext.Domain;

                return MergeWebConfig(domain, webConfigPath);

                //System.Configuration.ConfigurationManager.RefreshSection("system.web/machineKey");
            }
        }

        /// <summary>
        /// Merges web.config with settings in the registry
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="webConfigPath"></param>
        public bool MergeWebConfig(Domain domain, string webConfigPath)
        {
            // Load web.config from the local web
            var webConfig = new XmlDocument();
            webConfig.Load(webConfigPath);

            // Load web.config from domain settings
            XmlDocument domainConfig = null;

            if (domain.Settings.ContainsKey("web.config"))
            {
                var xml = domain.Settings["web.config"].XmlValue;
                if (!String.IsNullOrWhiteSpace(xml))
                {
                    domainConfig = new XmlDocument();
                    domainConfig.LoadXml(xml);
                }
            }

            if (domainConfig != null)
            {
                var oldConfig = webConfig.InnerXml;

                // TODO replace repeating nodes...
                Util.ConfigXmlMerger.Merge(webConfig, domainConfig);

                // Only update web.config if change is made
                if (webConfig.InnerXml != oldConfig)
                {
                    // Overwrite web.config
                    try
                    {
                        webConfig.Save(webConfigPath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Cannot write web.config. Change file permissions.", ex);
                    }
                }
            }

            return false;
        }
    }
}
