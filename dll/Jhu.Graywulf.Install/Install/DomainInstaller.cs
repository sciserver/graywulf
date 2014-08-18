using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class DomainInstaller : ContextObject
    {
        private Domain domain;

        public DomainInstaller(Domain domain)
            : base(domain.Context)
        {
            this.domain = domain;
        }

        public void GenerateDefaultSettings()
        {
            domain.IdentityProvider = typeof(Jhu.Graywulf.Web.Security.GraywulfIdentityProvider).AssemblyQualifiedName;
            domain.AuthenticatorFactory = typeof(Jhu.Graywulf.Web.Security.AuthenticatorFactory).AssemblyQualifiedName;

            GenerateWebConfig();
        }

        private void GenerateWebConfig()
        {
            // Load web.config template from resource
            var webConfig = new XmlDocument();
            webConfig.LoadXml(Scripts.web_config);

            // Generate a machine key
            var mknode = (XmlElement)webConfig.SelectSingleNode("/configuration/system.web/machineKey");

            mknode.SetAttribute("validationKey", GenerateKey(64));
            mknode.SetAttribute("decryptionKey", GenerateKey(32));

            var p = new Parameter()
            {
                Name = "web.config",
                XmlValue = webConfig.InnerXml
            };
            domain.Settings.Add(p);
        }

        public void GenerateDefaultChildren()
        {
            // Create standard user group
            UserGroup ug = new UserGroup(domain);
            ug.Name = Constants.StandardUserGroupName;
            ug.Save();

            domain.StandardUserGroup = ug;
            domain.Save();
        }

        private string GenerateKey(int numBytes)
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[numBytes];

            rng.GetBytes(buff);
            
            return BytesToHexString(buff);
        }

        private string BytesToHexString(byte[] bytes)
        {
            var hexString = new StringBuilder(64);

            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }
            
            return hexString.ToString();
        }
    }
}
