using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.ServiceModel
{
    public class TcpEndpointConfiguration : ConfigurationElement
    {
        public class LimitedAccessRole : ConfigurationElement
        {
            [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
            public string Name
            {
                get { return ((string)(base["name"])); }
                set { base["name"] = value; }
            }

            [ConfigurationProperty("users", DefaultValue = null, IsRequired = false)]
            public string Users
            {
                get { return ((string)(base["users"])); }
                set { base["users"] = value; }
            }

            [ConfigurationProperty("groups", DefaultValue = null, IsRequired = false)]
            public string Groups
            {
                get { return ((string)(base["groups"])); }
                set { base["groups"] = value; }
            }
        }

        [ConfigurationCollection(typeof(LimitedAccessRole))]
        public class LimitedAccessRoleCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new LimitedAccessRole();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((LimitedAccessRole)(element)).Name;
            }

            public LimitedAccessRole this[int idx]
            {
                get
                {
                    return (LimitedAccessRole)BaseGet(idx);
                }
            }

            new public LimitedAccessRole this[string key]
            {
                get
                {
                    return (LimitedAccessRole)BaseGet(key);
                }
            }
        }

        [ConfigurationProperty("tcpPort")]
        public int TcpPort
        {
            get { return (int)base["tcpPort"]; }
            set { base["tcpPort"] = value; }
        }

        [ConfigurationProperty("servicePrincipalName")]
        public string ServicePrincipalName
        {
            get { return (string)base["servicePrincipalName"]; }
            set { base["servicePrincipalName"] = value; }
        }

        [ConfigurationProperty("accessRoles")]
        public LimitedAccessRoleCollection AccessRoles
        {
            get { return ((LimitedAccessRoleCollection)(base["accessRoles"])); }
        }
    }
}
