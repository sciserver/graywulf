using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Security
{
    static class KeystoneSettings
    {
        private const string groupName = "Jhu.Graywulf/Web/Security/Keystone";

        public static Uri Uri
        {
            get { return new Uri(Jhu.Graywulf.Util.ConfigurationReader.GetValue(groupName, "Uri")); }
        }

        public static string AdminTenant
        {
            get { return Jhu.Graywulf.Util.ConfigurationReader.GetValue(groupName, "AdminTenant"); }
        }

        public static string AdminUser
        {
            get { return Jhu.Graywulf.Util.ConfigurationReader.GetValue(groupName, "AdminUser"); }
        }

        public static string AdminPassword
        {
            get { return Jhu.Graywulf.Util.ConfigurationReader.GetValue(groupName, "AdminPassword"); }
        }
    }
}
