using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Registry
{
    public static class AppSettings
    {
        private static string GetValue(string key)
        {
            var section = (NameValueCollection)ConfigurationManager.GetSection("Jhu.Graywulf/Registry");
            if (section != null)
            {
                return (string)section[key];
            }
            else
            {
                return null;
            }
        }

        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Jhu.Graywulf.Registry"].ConnectionString; }
        }

        public static string ClusterName
        {
            get { return GetValue("ClusterName"); }
        }

        public static string DomainName
        {
            get { return GetValue("DomainName"); }
        }

        public static string FederationName
        {
            get { return GetValue("FederationName"); }
        }
    }
}
